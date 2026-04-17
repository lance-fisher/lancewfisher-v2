import hashlib
import json
import os
import re
import shutil
import sqlite3
import subprocess
import time
import urllib.request
from collections import Counter, defaultdict
from datetime import datetime, timezone
from difflib import unified_diff
from pathlib import Path, PureWindowsPath
from typing import Any


def utc_now() -> str:
    return datetime.now(timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")


def iso_from_epoch(epoch: float) -> str:
    return datetime.fromtimestamp(epoch, timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def safe_relpath(path: Path, root: Path) -> str:
    resolved = path.resolve()
    root_resolved = root.resolve()
    if root_resolved not in resolved.parents and resolved != root_resolved:
        raise ValueError(f"Path escapes root: {path}")
    return resolved.relative_to(root_resolved).as_posix()


class PlatformError(RuntimeError):
    def __init__(self, message: str, status_code: int = 400, payload: dict[str, Any] | None = None) -> None:
        super().__init__(message)
        self.status_code = status_code
        self.payload = payload or {}


class BloodlinesContinuationCore:
    IGNORE_DIRS = {
        ".git",
        ".venv",
        "__pycache__",
        "node_modules",
        ".pytest_cache",
        "dist",
        "build",
    }

    DISCOVERED_DOCUMENT_EXTENSIONS = {
        ".md",
        ".txt",
        ".json",
        ".yml",
        ".yaml",
    }

    DISCOVERED_CODE_EXTENSIONS = {
        ".py",
        ".js",
        ".ts",
        ".tsx",
        ".html",
        ".css",
        ".cs",
        ".ps1",
        ".bat",
        ".cmd",
    }

    PROTECTED_PROJECT_FILES = {
        "AGENTS.md",
        "CLAUDE.md",
    }

    NOISE_DIRECTORY_NAMES = {
        "library",
        "usersettings",
        "obj",
        "packagecache",
    }

    def __init__(self, platform_root: Path) -> None:
        self.platform_root = platform_root.resolve()
        self.config_dir = self.platform_root / "config"
        self.state_dir = self.platform_root / "state"
        self.static_dir = self.platform_root / "static"
        self.docs_dir = self.platform_root / "docs"
        self.tests_dir = self.platform_root / "tests"

        self.scan_settings = self._load_json(self.config_dir / "scan_settings.json")
        self.routing_policy = self._load_json(self.config_dir / "routing_policy.json")
        self.doctrine_rules = self._load_json(self.config_dir / "doctrine_rules.json")
        self.source_subset = self._load_json(self.config_dir / "source_subset.json")
        self.tier_gate_hashes = self._load_json(self.config_dir / "tier_gate_hashes.json")
        self.command_runner_policy = self._load_json(self.config_dir / "command_runner_policy.json")

        self.bloodlines_root = Path(self.scan_settings["bloodlines_root"]).resolve()
        self.db_path = self.state_dir / "continuation.sqlite3"
        self.source_map_path = self.state_dir / "source_map.json"
        self.registry_path = self.state_dir / "canonical_source_registry.json"
        self.discovered_registry_path = self.state_dir / "discovered_source_registry.json"
        self.change_report_path = self.state_dir / "change_report.json"
        self.model_inventory_path = self.state_dir / "model_inventory.json"
        self.resume_state_path = self.state_dir / "resume_state.json"
        self.telemetry_path = self.state_dir / "telemetry.json"
        self.journal_path = self.state_dir / "operations_journal.jsonl"
        self.checkpoints_path = self.state_dir / "scan_checkpoints.json"
        self.handoff_path = self.state_dir / "handoff_pack_preview.md"
        self.last_agent_result_path = self.state_dir / "last_agent_result.json"
        self.tasks_board_path = self.state_dir / "tasks_board.json"
        self.execution_packet_path = self.state_dir / "execution_packet.json"
        self.agent_console_session_path = self.state_dir / "agent_console_session.json"
        self.agent_console_drafts_path = self.state_dir / "agent_console_drafts.json"

        self.session_state = {
            "started_at": utc_now(),
            "write_posture": "read_only",
            "active_tier": None,
            "project_writes_allowed": False,
            "unlock_reason": "default_locked",
            "last_unlock_at": None,
            "resume_candidate_override": None,
        }

        self._ensure_layout()
        self.fts_enabled = self._init_database()
        self.telemetry = self._load_json(self.telemetry_path, self._default_telemetry())
        self.last_agent_result = self._load_json(self.last_agent_result_path, {})
        self._ensure_generated_files()
        self._update_degraded_modes(active=True, reason_code="write_locked")
        if not self.fts_enabled:
            self._update_degraded_modes(active=True, reason_code="fts_disabled")

    def _ensure_layout(self) -> None:
        for folder in (self.state_dir, self.static_dir, self.docs_dir, self.tests_dir):
            folder.mkdir(parents=True, exist_ok=True)

    def _ensure_generated_files(self) -> None:
        if not self.journal_path.exists():
            self.journal_path.write_text("", encoding="utf-8")
        if not self.agent_console_session_path.exists():
            self._write_json(self.agent_console_session_path, self._default_agent_console_session())
        if not self.agent_console_drafts_path.exists():
            self._write_json(self.agent_console_drafts_path, self._default_agent_console_drafts())

    def _load_json(self, path: Path, default: Any | None = None) -> Any:
        if not path.exists():
            if default is None:
                raise FileNotFoundError(path)
            return default
        with path.open("r", encoding="utf-8") as handle:
            return json.load(handle)

    def _write_json(self, path: Path, payload: Any) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        with path.open("w", encoding="utf-8") as handle:
            json.dump(payload, handle, indent=2)

    def _default_telemetry(self) -> dict[str, Any]:
        return {
            "generated_at": utc_now(),
            "ingestion": {
                "last_scan_time": None,
                "last_scan_duration_seconds": None,
                "files_scanned": 0,
                "subset_documents": 0,
                "changed_documents": 0,
                "last_checkpoint_file": None,
            },
            "retrieval": {
                "last_hit_rate": 0.0,
                "top_result_provenance_distribution": {},
                "recent_queries": [],
            },
            "doctrine": {
                "violation_count": 0,
                "categories": {},
            },
            "routing": {
                "decisions_by_task_type": {},
                "last_assignments": self.routing_policy.get("task_routing", {}),
            },
            "handoff": {
                "candidate_count": 0,
                "oldest_candidate_age_hours": None,
            },
            "writes": {
                "count": 0,
                "approved_count": 0,
                "refused_count": 0,
                "last_reason_code": None,
            },
            "degraded_modes": {
                "count": 0,
                "reasons": {},
                "active": [],
            },
            "recent_tasks": [],
        }

    def _default_agent_console_session(self) -> dict[str, Any]:
        created_at = utc_now()
        return {
            "generated_at": created_at,
            "session_id": "bloodlines-main-console",
            "message_counter": 1,
            "messages": [
                {
                    "id": "assistant-1",
                    "role": "assistant",
                    "kind": "welcome",
                    "content": (
                        "Bloodlines offline agent console is ready. Use natural-language prompts for local model "
                        "reasoning, or issue commands such as /resume, /rescan, /search, /read, /drafts, "
                        "/apply-draft, and /anchor."
                    ),
                    "created_at": created_at,
                    "citations": [
                        "CURRENT_PROJECT_STATE.md",
                        "NEXT_SESSION_HANDOFF.md",
                        "continuation/PROJECT_STATE.json",
                    ],
                    "actions_taken": [
                        "Local continuity state will stay inside the Bloodlines root.",
                        "Project writes remain governed by the current tier lock.",
                    ],
                    "routing": None,
                    "confidence": 1.0,
                    "draft_ids": [],
                    "unresolved_items": [],
                }
            ],
        }

    def _default_agent_console_drafts(self) -> dict[str, Any]:
        return {"generated_at": utc_now(), "drafts": []}

    def _connect(self) -> sqlite3.Connection:
        connection = sqlite3.connect(self.db_path, timeout=30)
        connection.row_factory = sqlite3.Row
        connection.execute("PRAGMA journal_mode=WAL")
        return connection

    def _init_database(self) -> bool:
        fts_enabled = True
        with self._connect() as connection:
            connection.executescript(
                """
                CREATE TABLE IF NOT EXISTS scans (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    started_at TEXT NOT NULL,
                    completed_at TEXT,
                    status TEXT NOT NULL,
                    mode TEXT NOT NULL,
                    files_scanned INTEGER DEFAULT 0,
                    subset_docs_ingested INTEGER DEFAULT 0,
                    notes TEXT
                );

                CREATE TABLE IF NOT EXISTS documents (
                    path TEXT PRIMARY KEY,
                    abs_path TEXT NOT NULL,
                    relative_path TEXT NOT NULL,
                    classification TEXT NOT NULL,
                    authority_score REAL NOT NULL,
                    topic TEXT,
                    notes TEXT,
                    sha256 TEXT NOT NULL,
                    mtime REAL NOT NULL,
                    mtime_iso TEXT NOT NULL,
                    size_bytes INTEGER NOT NULL,
                    source_kind TEXT NOT NULL,
                    artifact_provider TEXT,
                    artifact_kind TEXT,
                    last_scan_id INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS chunks (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    doc_path TEXT NOT NULL,
                    chunk_index INTEGER NOT NULL,
                    heading TEXT,
                    text TEXT NOT NULL,
                    token_estimate INTEGER NOT NULL,
                    FOREIGN KEY(doc_path) REFERENCES documents(path)
                );

                CREATE TABLE IF NOT EXISTS artifacts (
                    path TEXT PRIMARY KEY,
                    abs_path TEXT NOT NULL,
                    provider TEXT NOT NULL,
                    artifact_kind TEXT NOT NULL,
                    sha256 TEXT NOT NULL,
                    mtime REAL NOT NULL,
                    mtime_iso TEXT NOT NULL,
                    size_bytes INTEGER NOT NULL,
                    last_scan_id INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS discovered_documents (
                    path TEXT PRIMARY KEY,
                    abs_path TEXT NOT NULL,
                    classification TEXT NOT NULL,
                    authority_score REAL NOT NULL,
                    sha256 TEXT NOT NULL,
                    mtime REAL NOT NULL,
                    mtime_iso TEXT NOT NULL,
                    size_bytes INTEGER NOT NULL,
                    top_level TEXT NOT NULL,
                    extension TEXT NOT NULL,
                    reason TEXT NOT NULL,
                    excerpt TEXT NOT NULL,
                    is_canonical_subset INTEGER NOT NULL,
                    last_scan_id INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS journal (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    created_at TEXT NOT NULL,
                    event_type TEXT NOT NULL,
                    status TEXT NOT NULL,
                    doctrine_check TEXT NOT NULL,
                    provenance TEXT NOT NULL,
                    summary TEXT NOT NULL,
                    payload_json TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS resume_candidates (
                    candidate_type TEXT PRIMARY KEY,
                    label TEXT NOT NULL,
                    source_path TEXT,
                    source_ref TEXT,
                    event_time TEXT NOT NULL,
                    event_epoch REAL NOT NULL,
                    evidence_json TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS write_events (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    created_at TEXT NOT NULL,
                    target_path TEXT NOT NULL,
                    execution_mode TEXT NOT NULL,
                    approval_state TEXT NOT NULL,
                    reason_code TEXT NOT NULL,
                    source_basis TEXT NOT NULL,
                    details_json TEXT NOT NULL
                );
                """
            )
            try:
                connection.execute(
                    "CREATE VIRTUAL TABLE IF NOT EXISTS chunks_fts USING fts5(doc_path, heading, text)"
                )
            except sqlite3.OperationalError:
                fts_enabled = False
        return fts_enabled

    def bootstrap(self, force: bool = False) -> dict[str, Any]:
        must_scan = force or not self.source_map_path.exists() or not self.registry_path.exists()
        if must_scan:
            self.perform_rescan(mode="manual" if force else "startup")
        return self.get_dashboard_snapshot()

    def perform_rescan(self, mode: str = "manual") -> dict[str, Any]:
        started_at = utc_now()
        start_epoch = time.time()
        checkpoints = self._load_json(self.checkpoints_path, {"files": {}, "last_scan": None})
        previous_source_map = self._load_json(self.source_map_path, {})
        previous_registry = self._load_json(self.registry_path, {"documents": []})
        previous_discovered_registry = self._load_json(
            self.discovered_registry_path,
            {"total_documents": 0, "documents": [], "conflict_clusters": []},
        )

        with self._connect() as connection:
            cursor = connection.execute(
                "INSERT INTO scans (started_at, status, mode) VALUES (?, ?, ?)",
                (started_at, "running", mode),
            )
            scan_id = int(cursor.lastrowid)

        try:
            model_inventory = self.get_model_inventory(force_refresh=True)
            source_map, discovered_candidates = self._build_source_map()
            registry = self._ingest_canonical_subset(scan_id=scan_id, checkpoints=checkpoints)
            discovered_registry = self._build_discovered_registry(
                scan_id=scan_id,
                checkpoints=checkpoints,
                candidates=discovered_candidates,
            )
            change_report = self._build_change_report(
                previous_source_map=previous_source_map,
                previous_registry=previous_registry,
                previous_discovered_registry=previous_discovered_registry,
                source_map=source_map,
                registry=registry,
                discovered_registry=discovered_registry,
            )
            registry["discovered_summary"] = {
                "total_documents": discovered_registry["total_documents"],
                "counts": discovered_registry["counts"],
                "top_documents": discovered_registry["documents"][:120],
                "conflict_clusters": discovered_registry["conflict_clusters"][:40],
            }
            resume_state = self._build_resume_state(source_map)
            tasks_board = self._build_tasks_board()
            execution_packet = self._build_execution_packet(
                tasks_board=tasks_board,
                change_report=change_report,
                source_map=source_map,
                registry=registry,
            )

            completed_at = utc_now()
            duration_seconds = round(time.time() - start_epoch, 3)
            source_map["scan"] = {
                "id": scan_id,
                "mode": mode,
                "started_at": started_at,
                "completed_at": completed_at,
                "duration_seconds": duration_seconds,
            }

            self._write_json(self.source_map_path, source_map)
            self._write_json(self.registry_path, registry)
            self._write_json(self.discovered_registry_path, discovered_registry)
            self._write_json(self.change_report_path, change_report)
            self._write_json(self.resume_state_path, resume_state)
            self._write_json(self.tasks_board_path, tasks_board)
            self._write_json(self.execution_packet_path, execution_packet)
            self._write_json(self.checkpoints_path, checkpoints)

            self.telemetry["generated_at"] = completed_at
            self.telemetry["ingestion"]["last_scan_time"] = completed_at
            self.telemetry["ingestion"]["last_scan_duration_seconds"] = duration_seconds
            self.telemetry["ingestion"]["files_scanned"] = source_map["total_files_scanned"]
            self.telemetry["ingestion"]["subset_documents"] = len(registry["documents"])
            self.telemetry["ingestion"]["changed_documents"] = change_report["summary"]["changed_documents"]
            self.telemetry["ingestion"]["last_checkpoint_file"] = source_map.get("last_checkpoint_file")
            self.telemetry["handoff"]["candidate_count"] = len(
                [item for item in resume_state["candidates"] if item.get("exists")]
            )
            self.telemetry["handoff"]["oldest_candidate_age_hours"] = self._candidate_age_hours(
                resume_state["candidates"]
            )
            self._persist_telemetry()

            with self._connect() as connection:
                connection.execute(
                    """
                    UPDATE scans
                    SET completed_at = ?, status = ?, files_scanned = ?, subset_docs_ingested = ?, notes = ?
                    WHERE id = ?
                    """,
                    (
                        completed_at,
                        "completed",
                        source_map["total_files_scanned"],
                        len(registry["documents"]),
                        (
                            f"Artifacts={len(source_map['frontier_artifacts'])}; "
                            f"Models={len(model_inventory.get('models', []))}; "
                            f"Discovered={discovered_registry['total_documents']}"
                        ),
                        scan_id,
                    ),
                )

            self._journal_event(
                event_type="scan",
                status="completed",
                summary=f"Rescan completed in {duration_seconds}s",
                payload={
                    "scan_id": scan_id,
                    "files_scanned": source_map["total_files_scanned"],
                    "subset_documents": len(registry["documents"]),
                    "frontier_artifacts": len(source_map["frontier_artifacts"]),
                    "discovered_documents": discovered_registry["total_documents"],
                    "changed_documents": change_report["summary"]["changed_documents"],
                },
                provenance=[
                    self.source_map_path.name,
                    self.registry_path.name,
                    self.discovered_registry_path.name,
                    self.change_report_path.name,
                    self.resume_state_path.name,
                    self.tasks_board_path.name,
                    self.execution_packet_path.name,
                ],
            )
            return self.get_dashboard_snapshot()
        except Exception as exc:
            completed_at = utc_now()
            with self._connect() as connection:
                connection.execute(
                    "UPDATE scans SET completed_at = ?, status = ?, notes = ? WHERE id = ?",
                    (completed_at, "failed", str(exc), scan_id),
                )
            self._journal_event(
                event_type="scan",
                status="failed",
                summary="Rescan failed",
                payload={"scan_id": scan_id, "error": str(exc)},
                doctrine_check="fail",
                provenance=[],
            )
            raise

    def _build_source_map(self) -> tuple[dict[str, Any], list[dict[str, Any]]]:
        extension_counts: Counter[str] = Counter()
        top_level_counts: Counter[str] = Counter()
        frontier_artifacts: list[dict[str, Any]] = []
        recent_changes: list[dict[str, Any]] = []
        discovered_candidates: list[dict[str, Any]] = []
        canonical_set = {
            item["path"].replace("\\", "/"): item
            for item in self._current_canonical_subset()
        }
        files_scanned = 0
        last_checkpoint_file = None
        duplicate_buckets: dict[str, list[str]] = defaultdict(list)
        duplicate_counts: Counter[str] = Counter()
        duplicate_extensions = {".md", ".json", ".txt", ".yml", ".yaml"}

        with self._connect() as connection:
            connection.execute("DELETE FROM artifacts")

        for file_path, relative_path in self._iter_scan_files():
            files_scanned += 1
            stats = file_path.stat()
            top_level = relative_path.split("/", 1)[0] if "/" in relative_path else "."
            extension_counts[file_path.suffix.lower() or "[no_ext]"] += 1
            top_level_counts[top_level] += 1
            if not self._is_noise_path(relative_path):
                recent_changes.append(
                    {
                        "path": relative_path,
                        "mtime": iso_from_epoch(stats.st_mtime),
                        "mtime_epoch": stats.st_mtime,
                        "size_bytes": stats.st_size,
                        "classification": canonical_set.get(relative_path, {}).get(
                            "classification", self._heuristic_classification(relative_path)
                        ),
                    }
                )
                recent_changes.sort(key=lambda item: item["mtime_epoch"], reverse=True)
                if len(recent_changes) > 20:
                    recent_changes = recent_changes[:20]

            if self._should_include_discovered_candidate(relative_path, file_path.suffix.lower(), stats.st_size):
                discovered_candidates.append(
                    {
                        "path": relative_path,
                        "abs_path": str(file_path),
                        "mtime": iso_from_epoch(stats.st_mtime),
                        "mtime_epoch": stats.st_mtime,
                        "size_bytes": stats.st_size,
                        "top_level": top_level,
                        "extension": file_path.suffix.lower(),
                    }
                )

            artifact = self._detect_frontier_artifact(relative_path)
            if artifact:
                artifact_entry = {
                    "path": relative_path,
                    "provider": artifact["provider"],
                    "artifact_kind": artifact["artifact_kind"],
                    "mtime": iso_from_epoch(stats.st_mtime),
                    "mtime_epoch": stats.st_mtime,
                    "size_bytes": stats.st_size,
                    "sha256": self._hash_file(file_path),
                }
                frontier_artifacts.append(artifact_entry)
                with self._connect() as connection:
                    connection.execute(
                        """
                        INSERT OR REPLACE INTO artifacts
                        (path, abs_path, provider, artifact_kind, sha256, mtime, mtime_iso, size_bytes, last_scan_id)
                        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
                        """,
                        (
                            relative_path,
                            str(file_path),
                            artifact_entry["provider"],
                            artifact_entry["artifact_kind"],
                            artifact_entry["sha256"],
                            stats.st_mtime,
                            artifact_entry["mtime"],
                            stats.st_size,
                            self._latest_scan_id(),
                        ),
                    )
            suffix = Path(relative_path).suffix.lower()
            if suffix in duplicate_extensions:
                duplicate_name = PureWindowsPath(relative_path).name.lower()
                duplicate_counts[duplicate_name] += 1
                bucket = duplicate_buckets[duplicate_name]
                if len(bucket) < 8:
                    bucket.append(relative_path)
            last_checkpoint_file = relative_path
        frontier_artifacts.sort(key=lambda item: item["mtime_epoch"], reverse=True)
        duplicate_names = [
            {"name": name, "paths": duplicate_buckets[name], "count": count}
            for name, count in duplicate_counts.items()
            if count > 1
        ]
        duplicate_names.sort(key=lambda item: item["count"], reverse=True)

        return ({
            "generated_at": utc_now(),
            "bloodlines_root": str(self.bloodlines_root),
            "scope_policy": self.scan_settings["scan_scope_policy"],
            "total_files_scanned": files_scanned,
            "top_level_counts": dict(top_level_counts),
            "extension_counts": dict(extension_counts),
            "canonical_subset_count": len(canonical_set),
            "frontier_artifacts": frontier_artifacts[:60],
            "recent_changes": recent_changes[:20],
            "potential_conflicts": duplicate_names[:20],
            "known_questions": self._current_known_questions(),
            "last_checkpoint_file": last_checkpoint_file,
        }, discovered_candidates)

    def _should_include_discovered_candidate(self, relative_path: str, extension: str, size_bytes: int) -> bool:
        if size_bytes > 1_500_000:
            return False
        if self._is_noise_path(relative_path):
            return False
        lowered = relative_path.lower()
        if any(
            lowered.startswith(prefix)
            for prefix in (
                "continuation-platform/",
                "artifacts/",
                ".tmp-edge",
                "unity/library/",
                "unity/logs/",
                "unity/userSettings/".lower(),
                "unity/obj/",
                "unity/packages/",
            )
        ):
            return False
        if extension in self.DISCOVERED_DOCUMENT_EXTENSIONS:
            return True
        if extension in self.DISCOVERED_CODE_EXTENSIONS:
            return any(
                lowered.startswith(prefix)
                for prefix in (
                    "src/",
                    "scripts/",
                    "api/",
                    "tests/",
                    "tasks/",
                    "data/",
                    "unity/assets/_bloodlines/code/",
                )
            )
        return False

    def _is_noise_path(self, relative_path: str) -> bool:
        lowered = relative_path.replace("\\", "/").lower()
        if any(f"/{segment}/" in f"/{lowered}/" for segment in self.NOISE_DIRECTORY_NAMES):
            return True
        if lowered.startswith(".tmp-edge") or lowered.startswith("artifacts/edge-headless-profile/"):
            return True
        if lowered.startswith("unity/logs/") or lowered.startswith("unity/my project/library/"):
            return True
        if lowered.endswith((".log", ".tmp", ".bin", ".cm")):
            return True
        return False

    def _iter_scan_files(self):
        for dirpath, dirnames, filenames in os.walk(self.bloodlines_root):
            current = Path(dirpath)
            current_relative = os.path.relpath(dirpath, self.bloodlines_root).replace("\\", "/")
            if current_relative == "continuation-platform" or current_relative.startswith("continuation-platform/"):
                dirnames[:] = []
                continue

            filtered = []
            for name in dirnames:
                if name in self.IGNORE_DIRS:
                    continue
                if name.lower() in self.NOISE_DIRECTORY_NAMES:
                    continue
                if name.lower().startswith(".tmp-edge"):
                    continue
                child_relative = f"{current_relative}/{name}" if current_relative != "." else name
                if child_relative == "continuation-platform" or child_relative.startswith("continuation-platform/"):
                    continue
                filtered.append(name)
            dirnames[:] = filtered

            for filename in filenames:
                relative_path = f"{current_relative}/{filename}" if current_relative != "." else filename
                yield current / filename, relative_path.replace("\\", "/")

    def _heuristic_classification(self, relative_path: str) -> str:
        lowered = relative_path.lower()
        if any(part in lowered for part in ("_archive", "/archive/", "\\archive\\", "/archives/")):
            return "archival"
        if any(part in lowered for part in ("draft", "scratch", "brainstorm", "notes", "export")):
            return "secondary"
        return "secondary"

    def _hash_file(self, path: Path) -> str:
        with path.open("rb") as handle:
            return sha256_bytes(handle.read())

    def _configured_canonical_subset(self) -> list[dict[str, Any]]:
        return [dict(item) for item in self.source_subset.get("canonical_subset", [])]

    def _current_known_questions(self) -> list[str]:
        questions = list(self.source_subset.get("known_questions", []))
        questions.extend(
            [
                "What is the current Unity shipping-lane state?",
                "What is the next grounded Unity execution step?",
                "Which canonical files should be updated after the current slice?",
            ]
        )
        unique: list[str] = []
        seen = set()
        for question in questions:
            if question and question not in seen:
                seen.add(question)
                unique.append(question)
        return unique

    def _latest_matching_relative_path(self, pattern: str) -> str | None:
        matches = sorted(
            self.bloodlines_root.glob(pattern),
            key=lambda item: item.stat().st_mtime if item.exists() else 0,
            reverse=True,
        )
        for match in matches:
            if match.is_file():
                return safe_relpath(match, self.bloodlines_root)
        return None

    def _current_canonical_subset(self) -> list[dict[str, Any]]:
        configured = self._configured_canonical_subset()
        dynamic_topic_paths = {
            "latest_frontier_report": self._latest_matching_relative_path("docs/BLOODLINES_STATE_OF_GAME_REPORT_*.md"),
            "latest_unity_lane_handoff": self._latest_matching_relative_path("docs/unity/session-handoffs/*.md"),
            "project_gap_summary": self._latest_matching_relative_path("reports/*project_completion_handoff*.md"),
            "owner_direction": "governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md",
            "continuation_prompt": self._latest_matching_relative_path("03_PROMPTS/CONTINUATION_PROMPT_*.md"),
            "continuation_platform": "continuation-platform/README.md",
        }

        skip_topics = {topic for topic, path in dynamic_topic_paths.items() if path}
        items = [item for item in configured if item.get("topic") not in skip_topics]

        dynamic_entries = [
            {
                "path": dynamic_topic_paths["owner_direction"],
                "classification": "authoritative",
                "authority_score": 0.985,
                "topic": "owner_direction",
                "notes": "Current non-negotiable owner direction for full-canon Unity delivery.",
            },
            {
                "path": dynamic_topic_paths["continuation_platform"],
                "classification": "secondary",
                "authority_score": 0.88,
                "topic": "continuation_platform",
                "notes": "Local continuation cockpit operating guide.",
            },
        ]

        latest_frontier_report = dynamic_topic_paths["latest_frontier_report"]
        if latest_frontier_report:
            dynamic_entries.append(
                {
                    "path": latest_frontier_report,
                    "classification": "secondary",
                    "authority_score": 0.89,
                    "topic": "latest_frontier_report",
                    "notes": "Latest state-of-game report currently on disk.",
                }
            )

        latest_unity_handoff = dynamic_topic_paths["latest_unity_lane_handoff"]
        if latest_unity_handoff:
            dynamic_entries.append(
                {
                    "path": latest_unity_handoff,
                    "classification": "authoritative",
                    "authority_score": 0.975,
                    "topic": "latest_unity_lane_handoff",
                    "notes": "Latest Unity shipping-lane handoff and verification surface.",
                }
            )

        project_gap_summary = dynamic_topic_paths["project_gap_summary"]
        if project_gap_summary:
            dynamic_entries.append(
                {
                    "path": project_gap_summary,
                    "classification": "secondary",
                    "authority_score": 0.91,
                    "topic": "project_gap_summary",
                    "notes": "Current completed-vs-remaining project-wide status report.",
                }
            )

        continuation_prompt = dynamic_topic_paths["continuation_prompt"]
        if continuation_prompt:
            dynamic_entries.append(
                {
                    "path": continuation_prompt,
                    "classification": "secondary",
                    "authority_score": 0.84,
                    "topic": "continuation_prompt",
                    "notes": "Latest ready-to-paste continuation prompt on disk.",
                }
            )

        by_path = {item["path"].replace("\\", "/"): item for item in items}
        for entry in dynamic_entries:
            relative_path = (entry.get("path") or "").replace("\\", "/")
            absolute_path = (self.bloodlines_root / relative_path).resolve()
            if not relative_path or not absolute_path.exists():
                continue
            by_path[relative_path] = entry

        return sorted(by_path.values(), key=lambda item: (item["authority_score"], item["path"]), reverse=True)

    def _safe_project_target(self, relative_path: str) -> Path:
        normalized = relative_path.replace("\\", "/").strip().lstrip("/")
        if not normalized:
            raise PlatformError("A relative path is required.", status_code=400, payload={"reason_code": "missing_path"})
        target = (self.bloodlines_root / normalized).resolve()
        if self.bloodlines_root not in target.parents and target != self.bloodlines_root:
            raise PlatformError(
                "Target path escaped the Bloodlines root.",
                status_code=400,
                payload={"reason_code": "scope_violation"},
            )
        return target

    def _read_text_file(self, target: Path) -> str:
        if not target.exists():
            return ""
        payload = target.read_bytes()
        if b"\x00" in payload:
            raise PlatformError(
                "Binary project files are not editable through the write workbench.",
                status_code=415,
                payload={"reason_code": "binary_not_supported"},
            )
        return payload.decode("utf-8", errors="ignore")

    def read_project_file(self, relative_path: str) -> dict[str, Any]:
        normalized = relative_path.replace("\\", "/").strip().lstrip("/")
        target = self._safe_project_target(normalized)
        exists = target.exists()
        current_text = self._read_text_file(target) if exists else ""
        current_hash = sha256_bytes(current_text.encode("utf-8")) if exists else None
        return {
            "status": "loaded",
            "relative_path": normalized,
            "exists": exists,
            "required_tier": self._required_tier_for_path(normalized),
            "content": current_text,
            "sha256": current_hash,
            "line_count": len(current_text.splitlines()),
        }

    def preview_project_write(
        self, relative_path: str, content: str, reason: str, source_basis: str
    ) -> dict[str, Any]:
        normalized = relative_path.replace("\\", "/").strip().lstrip("/")
        target = self._safe_project_target(normalized)
        current_text = self._read_text_file(target) if target.exists() else ""
        diff_lines = list(
            unified_diff(
                current_text.splitlines(keepends=True),
                content.splitlines(keepends=True),
                fromfile=f"a/{normalized}",
                tofile=f"b/{normalized}",
                n=3,
            )
        )
        diff_text = "".join(diff_lines)
        added_lines = sum(1 for line in diff_lines if line.startswith("+") and not line.startswith("+++"))
        removed_lines = sum(1 for line in diff_lines if line.startswith("-") and not line.startswith("---"))
        return {
            "status": "preview",
            "relative_path": normalized,
            "exists": target.exists(),
            "required_tier": self._required_tier_for_path(normalized),
            "reason": reason,
            "source_basis": source_basis,
            "changed": current_text != content,
            "current_sha256": sha256_bytes(current_text.encode("utf-8")) if target.exists() else None,
            "proposed_sha256": sha256_bytes(content.encode("utf-8")),
            "current_line_count": len(current_text.splitlines()),
            "proposed_line_count": len(content.splitlines()),
            "added_line_count": added_lines,
            "removed_line_count": removed_lines,
            "diff_preview": diff_text[:12000] if diff_text else "No textual changes detected.",
        }

    def _load_agent_console_session(self) -> dict[str, Any]:
        return self._load_json(self.agent_console_session_path, self._default_agent_console_session())

    def _load_agent_console_drafts(self) -> dict[str, Any]:
        return self._load_json(self.agent_console_drafts_path, self._default_agent_console_drafts())

    def _save_agent_console_session(self, session: dict[str, Any]) -> None:
        session["generated_at"] = utc_now()
        self._write_json(self.agent_console_session_path, session)

    def _save_agent_console_drafts(self, drafts_payload: dict[str, Any]) -> None:
        drafts_payload["generated_at"] = utc_now()
        self._write_json(self.agent_console_drafts_path, drafts_payload)

    def _next_agent_console_message_id(self, session: dict[str, Any], role: str) -> str:
        counter = int(session.get("message_counter", 0)) + 1
        session["message_counter"] = counter
        return f"{role}-{counter}"

    def _append_agent_console_message(
        self,
        session: dict[str, Any],
        role: str,
        kind: str,
        content: str,
        citations: list[str] | None = None,
        actions_taken: list[str] | None = None,
        routing: dict[str, Any] | None = None,
        confidence: float | None = None,
        draft_ids: list[str] | None = None,
        unresolved_items: list[str] | None = None,
    ) -> dict[str, Any]:
        message = {
            "id": self._next_agent_console_message_id(session, role),
            "role": role,
            "kind": kind,
            "content": content,
            "created_at": utc_now(),
            "citations": citations or [],
            "actions_taken": actions_taken or [],
            "routing": routing,
            "confidence": confidence,
            "draft_ids": draft_ids or [],
            "unresolved_items": unresolved_items or [],
        }
        session.setdefault("messages", []).append(message)
        session["messages"] = session["messages"][-60:]
        return message

    def _agent_console_available_commands(self) -> list[dict[str, str]]:
        return [
            {"command": "/help", "description": "Show the local command set."},
            {"command": "/resume", "description": "Run Resume Last Good State inside the console."},
            {"command": "/rescan", "description": "Re-ingest the Bloodlines root and refresh continuity."},
            {"command": "/status", "description": "Show write posture, active anchor, and next execution lane."},
            {"command": "/search <query>", "description": "Search the indexed Bloodlines corpus."},
            {"command": "/read <path>", "description": "Load a project file into the conversation."},
            {"command": "/execution", "description": "Show the current Unity execution packet."},
            {"command": "/anchor <candidate|clear>", "description": "Select local_agent_action, manual_edit, frontier_artifact, or clear."},
            {"command": "/drafts", "description": "List pending governed write drafts."},
            {"command": "/apply-draft <id>", "description": "Apply a pending write draft if the tier gate is unlocked."},
            {"command": "/dismiss-draft <id>", "description": "Remove a pending draft without applying it."},
            {"command": "/clear", "description": "Reset the current local conversation thread."},
        ]

    def get_agent_console_state(self) -> dict[str, Any]:
        session = self._load_agent_console_session()
        drafts = self._load_agent_console_drafts()
        resume_state = self._apply_resume_override(self._load_json(self.resume_state_path, {}))
        execution_packet = self.get_execution_packet()
        change_report = self.get_change_report()
        return {
            "generated_at": utc_now(),
            "session": session,
            "drafts": drafts.get("drafts", []),
            "available_commands": self._agent_console_available_commands(),
            "write_posture": self.get_write_posture(),
            "resume_state": resume_state,
            "status": {
                "anchor_label": (resume_state.get("effective_anchor") or resume_state.get("anchor") or {}).get("label"),
                "selection_required": resume_state.get("selection_required", False),
                "pending_drafts": len(drafts.get("drafts", [])),
                "changed_documents": change_report.get("summary", {}).get("changed_documents", 0),
                "recommended_next_step": execution_packet.get("recommended_next_step"),
            },
            "execution_summary": {
                "execution_lane": execution_packet.get("execution_lane"),
                "scene_target": execution_packet.get("scene_target"),
                "recommended_next_step": execution_packet.get("recommended_next_step"),
                "current_priority": (execution_packet.get("project_work_priority") or [None])[0],
            },
            "suggested_prompts": [
                "Summarize the current Bloodlines continuation priority and cite the governing files.",
                "Read CURRENT_PROJECT_STATE.md and tell me what the next Unity step should be.",
                "Draft an update to NEXT_SESSION_HANDOFF.md based on the latest continuity state.",
                "Search for the current Territorial Governance or Unity shell guidance.",
            ],
        }

    def reset_agent_console(self) -> dict[str, Any]:
        self._save_agent_console_session(self._default_agent_console_session())
        return self.get_agent_console_state()

    def submit_agent_console_message(self, message: str) -> dict[str, Any]:
        normalized = re.sub(r"\r\n?", "\n", message or "").strip()
        if not normalized:
            raise PlatformError("A prompt or command is required.", status_code=400, payload={"reason_code": "missing_prompt"})

        if normalized.lower() == "/clear":
            return self.reset_agent_console()

        session = self._load_agent_console_session()
        self._append_agent_console_message(session, role="user", kind="prompt", content=normalized)
        if normalized.startswith("/"):
            response = self._handle_agent_console_command(normalized)
        else:
            response = self._run_agent_console_prompt(session, normalized)

        self._append_agent_console_message(
            session,
            role="assistant",
            kind=response.get("kind", "assistant"),
            content=response.get("assistant_message", "No response generated."),
            citations=response.get("citations", []),
            actions_taken=response.get("actions_taken", []),
            routing=response.get("routing"),
            confidence=response.get("confidence"),
            draft_ids=response.get("draft_ids", []),
            unresolved_items=response.get("unresolved_items", []),
        )
        self._save_agent_console_session(session)
        self._journal_event(
            event_type="agent_action",
            status="completed",
            summary=f"Agent console handled: {normalized[:96]}",
            payload={
                "prompt": normalized,
                "draft_ids": response.get("draft_ids", []),
                "citations": response.get("citations", []),
                "routing": response.get("routing"),
            },
            provenance=response.get("citations", [])[:12],
        )
        return self.get_agent_console_state()

    def _resolve_project_file_reference(self, reference: str) -> str | None:
        normalized = reference.replace("\\", "/").strip().lstrip("/")
        if not normalized:
            return None
        direct_target = self.bloodlines_root / normalized
        if direct_target.exists():
            return normalized

        basename = PureWindowsPath(normalized).name.lower()
        candidates: list[tuple[int, float, str]] = []
        registry = self._load_json(self.registry_path, {"documents": []})
        discovered = self._load_json(self.discovered_registry_path, {"documents": []})
        for document in registry.get("documents", []):
            path = document.get("path", "")
            if PureWindowsPath(path).name.lower() == basename:
                candidates.append((2, float(document.get("authority_score", 0)), path))
        for document in discovered.get("documents", []):
            path = document.get("path", "")
            if PureWindowsPath(path).name.lower() == basename:
                candidates.append((1, float(document.get("authority_score", 0)), path))
        if not candidates:
            return None
        candidates.sort(key=lambda item: (item[0], item[1]), reverse=True)
        return candidates[0][2]

    def _handle_agent_console_command(self, raw_command: str) -> dict[str, Any]:
        command_line = raw_command.strip()
        command, _, argument = command_line.partition(" ")
        argument = argument.strip()
        command_lower = command.lower()

        if command_lower == "/help":
            lines = ["Available local commands:"]
            lines.extend(f"- {item['command']}: {item['description']}" for item in self._agent_console_available_commands())
            return {
                "kind": "command",
                "assistant_message": "\n".join(lines),
                "citations": ["continuation-platform/docs/system_design.md"],
                "actions_taken": ["Loaded the offline command reference."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/status":
            resume_state = self._apply_resume_override(self._load_json(self.resume_state_path, {}))
            anchor = resume_state.get("effective_anchor") or resume_state.get("anchor") or {}
            execution_packet = self.get_execution_packet()
            write_posture = self.get_write_posture()
            message = "\n".join(
                [
                    f"Write posture: {write_posture.get('write_posture')}",
                    f"Active tier: {write_posture.get('active_tier') or 'locked'}",
                    f"Resume anchor: {anchor.get('label') or 'unresolved'}",
                    f"Selection required: {resume_state.get('selection_required', False)}",
                    f"Execution lane: {execution_packet.get('execution_lane') or 'unresolved'}",
                    f"Next step: {execution_packet.get('recommended_next_step') or 'not generated'}",
                ]
            )
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": ["CURRENT_PROJECT_STATE.md", "NEXT_SESSION_HANDOFF.md", "continuity/PROJECT_STATE.json"],
                "actions_taken": ["Read the current continuity posture and execution packet."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/resume":
            result = self.resume_from_last_good_state()
            if result.get("status") == "requires_anchor_selection":
                candidate_names = [
                    item["candidate_type"]
                    for item in result.get("resume_state", {}).get("candidates", [])
                    if item.get("exists")
                ]
                message = (
                    f"{result.get('message')}\nAvailable candidates: {', '.join(candidate_names) or 'none'}.\n"
                    "Use /anchor <candidate> and run /resume again."
                )
            else:
                message = "\n".join(
                    [
                        result.get("reasoning_summary") or "Resume reasoning was generated.",
                        "",
                        f"Recommended next step: {result.get('recommended_next_step') or 'not generated'}",
                    ]
                )
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": result.get("provenance", []),
                "actions_taken": ["Ran Resume Last Good State inside the offline console."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": result.get("confidence", 1.0),
                "unresolved_items": result.get("unresolved_items", []),
            }

        if command_lower == "/rescan":
            snapshot = self.perform_rescan(mode="agent_console")
            changed = snapshot.get("change_report", {}).get("summary", {}).get("changed_documents", 0)
            files_scanned = snapshot.get("source_map", {}).get("total_files_scanned", 0)
            message = (
                f"Rescan complete.\nFiles scanned: {files_scanned}\nChanged documents: {changed}\n"
                f"Continuity health: {snapshot.get('status', {}).get('continuity_health')}"
            )
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": ["CURRENT_PROJECT_STATE.md", "NEXT_SESSION_HANDOFF.md", "continuity/PROJECT_STATE.json"],
                "actions_taken": ["Re-ingested the Bloodlines root and refreshed continuity artifacts."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/search":
            if not argument:
                raise PlatformError("Usage: /search <query>", status_code=400, payload={"reason_code": "missing_query"})
            results = self.search_chunks(argument, limit=6)
            if not results:
                message = f'No indexed results were found for "{argument}".'
            else:
                lines = [f'Search results for "{argument}":']
                for item in results:
                    lines.append(f"- {item['path']} :: {item.get('heading') or 'Document'}")
                    lines.append(f"  {item['excerpt'][:220]}")
                message = "\n".join(lines)
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": [item["path"] for item in results],
                "actions_taken": [f'Searched the local Bloodlines corpus for "{argument}".'],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/read":
            if not argument:
                raise PlatformError("Usage: /read <path>", status_code=400, payload={"reason_code": "missing_path"})
            resolved_path = self._resolve_project_file_reference(argument)
            if not resolved_path:
                raise PlatformError(
                    "No project file matched that reference.",
                    status_code=404,
                    payload={"reason_code": "file_not_found", "reference": argument},
                )
            loaded = self.read_project_file(resolved_path)
            content = loaded.get("content", "")
            if len(content) > 9000:
                content = f"{content[:9000]}\n\n[truncated after 9000 characters]"
            message = "\n".join(
                [
                    f"File: {loaded.get('relative_path')}",
                    f"Exists: {loaded.get('exists')}",
                    f"Required tier: {loaded.get('required_tier')}",
                    f"Line count: {loaded.get('line_count')}",
                    "",
                    content or "[empty file]",
                ]
            )
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": [loaded.get("relative_path")],
                "actions_taken": [f"Loaded {loaded.get('relative_path')} into the console."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/execution":
            packet = self.get_execution_packet()
            message = "\n".join(
                [
                    f"Execution lane: {packet.get('execution_lane') or 'unresolved'}",
                    f"Scene target: {packet.get('scene_target') or 'unresolved'}",
                    f"Next step: {packet.get('recommended_next_step') or 'not generated'}",
                    "",
                    "Current verified state:",
                    *[f"- {item}" for item in (packet.get("current_verified_state") or [])[:6]],
                ]
            )
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": packet.get("canonical_sources", [])[:8],
                "actions_taken": ["Loaded the current Unity execution packet."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/anchor":
            if not argument:
                raise PlatformError(
                    "Usage: /anchor <local_agent_action|manual_edit|frontier_artifact|clear>",
                    status_code=400,
                    payload={"reason_code": "missing_anchor"},
                )
            target = None if argument.lower() == "clear" else argument
            resume_state = self.set_resume_anchor(target)
            anchor = resume_state.get("effective_anchor") or resume_state.get("anchor") or {}
            return {
                "kind": "command",
                "assistant_message": f"Resume anchor is now {anchor.get('label') or 'cleared'}.",
                "citations": [anchor.get("source_path")] if anchor.get("source_path") else [],
                "actions_taken": [f"Selected resume anchor override: {argument}."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/drafts":
            drafts_payload = self._load_agent_console_drafts()
            drafts = drafts_payload.get("drafts", [])
            if not drafts:
                message = "No pending governed write drafts are staged."
            else:
                lines = ["Pending governed write drafts:"]
                for draft in drafts:
                    preview = draft.get("preview", {})
                    lines.append(
                        f"- {draft['id']} :: {draft['relative_path']} | changed={preview.get('changed')} | required={preview.get('required_tier')}"
                    )
                message = "\n".join(lines)
            return {
                "kind": "command",
                "assistant_message": message,
                "citations": [draft["relative_path"] for draft in drafts],
                "actions_taken": ["Listed pending write drafts."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/apply-draft":
            if not argument:
                raise PlatformError("Usage: /apply-draft <id>", status_code=400, payload={"reason_code": "missing_draft_id"})
            result = self.apply_write_draft(argument)
            return {
                "kind": "command",
                "assistant_message": f"Draft {argument} applied to {result.get('target_path')}.",
                "citations": [result.get("target_path")] if result.get("target_path") else [],
                "actions_taken": ["Applied a governed draft write and refreshed continuity."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        if command_lower == "/dismiss-draft":
            if not argument:
                raise PlatformError("Usage: /dismiss-draft <id>", status_code=400, payload={"reason_code": "missing_draft_id"})
            draft = self.dismiss_write_draft(argument)
            return {
                "kind": "command",
                "assistant_message": f"Draft {argument} was removed from the staging queue.",
                "citations": [draft.get("relative_path")] if draft else [],
                "actions_taken": ["Dismissed a pending write draft."],
                "routing": {"mode": "command", "command": command_lower},
                "confidence": 1.0,
            }

        raise PlatformError(
            "Unknown command. Use /help for the available local commands.",
            status_code=400,
            payload={"reason_code": "unknown_command", "command": command_lower},
        )

    def _build_agent_console_context(self, user_message: str, session: dict[str, Any]) -> dict[str, Any]:
        resume_state = self._apply_resume_override(self._load_json(self.resume_state_path, {}))
        tasks_board = self._load_json(self.tasks_board_path, {"open_tasks": [], "handoff_priority": []})
        execution_packet = self.get_execution_packet()
        change_report = self.get_change_report()
        write_posture = self.get_write_posture()
        message_history = []
        for item in session.get("messages", [])[-10:]:
            message_history.append(
                {
                    "role": item.get("role"),
                    "kind": item.get("kind"),
                    "content": str(item.get("content", ""))[:1200],
                }
            )

        explicit_files = []
        for reference in re.findall(r"[A-Za-z0-9_./\\-]+\.[A-Za-z0-9_]+", user_message):
            resolved = self._resolve_project_file_reference(reference)
            if not resolved or resolved in {item["relative_path"] for item in explicit_files}:
                continue
            loaded = self.read_project_file(resolved)
            content = loaded.get("content", "")
            explicit_files.append(
                {
                    "relative_path": loaded.get("relative_path"),
                    "required_tier": loaded.get("required_tier"),
                    "sha256": loaded.get("sha256"),
                    "content": content[:12000],
                    "truncated": len(content) > 12000,
                }
            )
            if len(explicit_files) >= 3:
                break

        retrieval_results = self.search_chunks(
            f"{user_message} Bloodlines continuity Unity shipping lane", limit=6
        )
        anchor = resume_state.get("effective_anchor") or resume_state.get("anchor") or {}
        return {
            "resume_state": {
                "selection_required": resume_state.get("selection_required", False),
                "anchor": {
                    "candidate_type": anchor.get("candidate_type"),
                    "label": anchor.get("label"),
                    "source_path": anchor.get("source_path"),
                },
            },
            "write_posture": {
                "write_posture": write_posture.get("write_posture"),
                "active_tier": write_posture.get("active_tier"),
            },
            "execution_packet": {
                "execution_lane": execution_packet.get("execution_lane"),
                "scene_target": execution_packet.get("scene_target"),
                "recommended_next_step": execution_packet.get("recommended_next_step"),
                "project_work_priority": (execution_packet.get("project_work_priority") or [])[:4],
                "current_verified_state": (execution_packet.get("current_verified_state") or [])[:5],
            },
            "tasks_board": {
                "project_work_priority": tasks_board.get("project_work_priority", [])[:6],
                "handoff_priority": tasks_board.get("handoff_priority", [])[:6],
                "open_tasks": tasks_board.get("open_tasks", [])[:6],
            },
            "change_report": {
                "summary": change_report.get("summary", {}),
                "high_signal_changes": change_report.get("high_signal_changes", [])[:6],
            },
            "message_history": message_history,
            "explicit_files": explicit_files,
            "retrieval_results": retrieval_results,
        }

    def _select_agent_console_task_type(self, user_message: str, context: dict[str, Any]) -> str:
        lowered = user_message.lower()
        write_signals = ("write", "update", "edit", "draft", "create", "revise", "apply", "change")
        if context.get("explicit_files") or any(token in lowered for token in write_signals):
            return "generation"
        return "design_continuation"

    def _build_agent_console_prompt(
        self,
        user_message: str,
        context: dict[str, Any],
        tool_trace: list[dict[str, Any]],
    ) -> str:
        doctrine = "\n".join(f"- {item['rule']}" for item in self.doctrine_rules.get("rules", []))
        tools = "\n".join(
            [
                "- search_context {query, limit}",
                "- read_file {relative_path}",
                "- get_execution_packet {}",
                "- get_change_report {}",
                "- get_tasks_board {}",
                "- resume {}",
                "- rescan {}",
                "- select_anchor {candidate_type}",
            ]
        )
        explicit_files = "\n\n".join(
            [
                "\n".join(
                    [
                        f"[{item['relative_path']}]",
                        f"required_tier={item['required_tier']}",
                        item["content"] + ("\n[truncated]" if item.get("truncated") else ""),
                    ]
                )
                for item in context.get("explicit_files", [])
            ]
        )
        retrieval = "\n\n".join(
            [
                f"[{item['path']} :: {item.get('heading') or 'Document'}]\n{item['excerpt']}"
                for item in context.get("retrieval_results", [])
            ]
        )
        history = "\n".join(
            [
                f"{item['role']} ({item['kind']}): {item['content']}"
                for item in context.get("message_history", [])
            ]
        )
        trace = json.dumps(tool_trace[-4:], indent=2)
        return f"""
You are the Bloodlines offline agent console operating inside the governed continuation platform.

Non-negotiable doctrine:
{doctrine}

Available local tools:
{tools}

Return strict JSON with these keys:
- mode: "tool" | "final" | "draft_write"
- assistant_message
- confidence
- citations
- actions_taken
- unresolved_items
- tool_name
- arguments
- write_draft

Rules:
- Use at most one tool per response.
- Cite only files that informed the answer.
- If the user asks to change a file, read it first unless it is already present below.
- A draft write must include relative_path, reason, and complete content.
- Do not claim a write was applied unless a tool response explicitly says it was applied.
- Prefer Bloodlines continuity files, owner direction, and canonical Unity execution guidance.

Current write posture:
{json.dumps(context.get("write_posture", {}), indent=2)}

Current resume state:
{json.dumps(context.get("resume_state", {}), indent=2)}

Current execution packet:
{json.dumps(context.get("execution_packet", {}), indent=2)}

Current task spine:
{json.dumps(context.get("tasks_board", {}), indent=2)}

Current change report:
{json.dumps(context.get("change_report", {}), indent=2)}

Conversation history:
{history}

User prompt:
{user_message}

Explicit file context:
{explicit_files or "[none]"}

Retrieved support:
{retrieval or "[none]"}

Tool trace so far:
{trace}
""".strip()

    def _execute_agent_console_tool(self, tool_name: str, arguments: dict[str, Any]) -> dict[str, Any]:
        if tool_name == "search_context":
            query = str(arguments.get("query", "")).strip()
            limit = max(1, min(int(arguments.get("limit", 6)), 8))
            if not query:
                raise PlatformError("search_context requires a query.", status_code=400, payload={"reason_code": "missing_query"})
            results = self.search_chunks(query, limit=limit)
            return {
                "tool_name": tool_name,
                "summary": f'Searched the local corpus for "{query}".',
                "citations": [item["path"] for item in results],
                "payload": {
                    "query": query,
                    "results": [
                        {
                            "path": item["path"],
                            "heading": item.get("heading"),
                            "excerpt": item["excerpt"][:260],
                        }
                        for item in results
                    ],
                },
            }

        if tool_name == "read_file":
            relative_path = self._resolve_project_file_reference(str(arguments.get("relative_path", "")).strip())
            if not relative_path:
                raise PlatformError("read_file could not resolve that path.", status_code=404, payload={"reason_code": "file_not_found"})
            loaded = self.read_project_file(relative_path)
            content = loaded.get("content", "")
            return {
                "tool_name": tool_name,
                "summary": f"Loaded {loaded.get('relative_path')}.",
                "citations": [loaded.get("relative_path")],
                "payload": {
                    "relative_path": loaded.get("relative_path"),
                    "required_tier": loaded.get("required_tier"),
                    "sha256": loaded.get("sha256"),
                    "content": content[:12000],
                    "truncated": len(content) > 12000,
                },
            }

        if tool_name == "get_execution_packet":
            packet = self.get_execution_packet()
            return {
                "tool_name": tool_name,
                "summary": "Loaded the current Unity execution packet.",
                "citations": packet.get("canonical_sources", [])[:8],
                "payload": {
                    "execution_lane": packet.get("execution_lane"),
                    "scene_target": packet.get("scene_target"),
                    "recommended_next_step": packet.get("recommended_next_step"),
                    "project_work_priority": (packet.get("project_work_priority") or [])[:5],
                    "current_verified_state": (packet.get("current_verified_state") or [])[:5],
                },
            }

        if tool_name == "get_change_report":
            report = self.get_change_report()
            return {
                "tool_name": tool_name,
                "summary": "Loaded the current continuity delta.",
                "citations": [item["path"] for item in report.get("high_signal_changes", [])[:8]],
                "payload": {
                    "summary": report.get("summary", {}),
                    "high_signal_changes": report.get("high_signal_changes", [])[:8],
                },
            }

        if tool_name == "get_tasks_board":
            tasks_board = self._load_json(self.tasks_board_path, {"open_tasks": [], "handoff_priority": []})
            return {
                "tool_name": tool_name,
                "summary": "Loaded the current task board.",
                "citations": ["tasks/todo.md", "NEXT_SESSION_HANDOFF.md", "HANDOFF.md"],
                "payload": {
                    "project_work_priority": tasks_board.get("project_work_priority", [])[:6],
                    "handoff_priority": tasks_board.get("handoff_priority", [])[:6],
                    "open_tasks": tasks_board.get("open_tasks", [])[:6],
                },
            }

        if tool_name == "resume":
            result = self.resume_from_last_good_state()
            return {
                "tool_name": tool_name,
                "summary": "Ran Resume Last Good State.",
                "citations": result.get("provenance", []),
                "payload": {
                    "status": result.get("status"),
                    "reasoning_summary": result.get("reasoning_summary"),
                    "recommended_next_step": result.get("recommended_next_step"),
                    "unresolved_items": result.get("unresolved_items", []),
                },
            }

        if tool_name == "rescan":
            snapshot = self.perform_rescan(mode="agent_console")
            return {
                "tool_name": tool_name,
                "summary": "Rescanned the Bloodlines root.",
                "citations": ["CURRENT_PROJECT_STATE.md", "NEXT_SESSION_HANDOFF.md", "continuity/PROJECT_STATE.json"],
                "payload": {
                    "continuity_health": snapshot.get("status", {}).get("continuity_health"),
                    "files_scanned": snapshot.get("source_map", {}).get("total_files_scanned", 0),
                    "changed_documents": snapshot.get("change_report", {}).get("summary", {}).get("changed_documents", 0),
                },
            }

        if tool_name == "select_anchor":
            candidate_type = arguments.get("candidate_type")
            resume_state = self.set_resume_anchor(candidate_type)
            anchor = resume_state.get("effective_anchor") or resume_state.get("anchor") or {}
            return {
                "tool_name": tool_name,
                "summary": f"Selected resume anchor {candidate_type}.",
                "citations": [anchor.get("source_path")] if anchor.get("source_path") else [],
                "payload": {"anchor": anchor},
            }

        raise PlatformError(
            "Requested tool is not available in the offline console.",
            status_code=400,
            payload={"reason_code": "unknown_tool", "tool_name": tool_name},
        )

    def _create_agent_console_draft(
        self,
        relative_path: str,
        reason: str,
        content: str,
        citations: list[str],
    ) -> dict[str, Any]:
        resolved_path = self._resolve_project_file_reference(relative_path) or relative_path.replace("\\", "/").strip().lstrip("/")
        preview = self.preview_project_write(
            relative_path=resolved_path,
            content=content,
            reason=reason,
            source_basis="agent_console",
        )
        draft_id = f"draft-{int(time.time() * 1000)}"
        draft = {
            "id": draft_id,
            "relative_path": preview["relative_path"],
            "reason": reason,
            "created_at": utc_now(),
            "source_basis": "agent_console",
            "expected_sha256": preview.get("current_sha256"),
            "content": content,
            "preview": preview,
            "citations": citations[:12],
        }
        drafts_payload = self._load_agent_console_drafts()
        drafts = [item for item in drafts_payload.get("drafts", []) if item.get("id") != draft_id]
        drafts.insert(0, draft)
        drafts_payload["drafts"] = drafts[:12]
        self._save_agent_console_drafts(drafts_payload)
        return draft

    def apply_write_draft(self, draft_id: str) -> dict[str, Any]:
        drafts_payload = self._load_agent_console_drafts()
        drafts = drafts_payload.get("drafts", [])
        target_draft = next((item for item in drafts if item.get("id") == draft_id), None)
        if not target_draft:
            raise PlatformError("Draft was not found.", status_code=404, payload={"reason_code": "draft_not_found"})
        result = self.project_write_probe(
            relative_path=target_draft["relative_path"],
            content=target_draft["content"],
            reason=target_draft["reason"],
            source_basis=target_draft.get("source_basis", "agent_console"),
            expected_sha256=target_draft.get("expected_sha256"),
        )
        drafts_payload["drafts"] = [item for item in drafts if item.get("id") != draft_id]
        self._save_agent_console_drafts(drafts_payload)
        self.perform_rescan(mode="agent_console_apply")
        return result

    def dismiss_write_draft(self, draft_id: str) -> dict[str, Any] | None:
        drafts_payload = self._load_agent_console_drafts()
        drafts = drafts_payload.get("drafts", [])
        target_draft = next((item for item in drafts if item.get("id") == draft_id), None)
        if not target_draft:
            raise PlatformError("Draft was not found.", status_code=404, payload={"reason_code": "draft_not_found"})
        drafts_payload["drafts"] = [item for item in drafts if item.get("id") != draft_id]
        self._save_agent_console_drafts(drafts_payload)
        return target_draft

    def _fallback_agent_console_message(self, context: dict[str, Any]) -> str:
        execution = context.get("execution_packet", {})
        anchor = context.get("resume_state", {}).get("anchor", {})
        priorities = execution.get("project_work_priority") or context.get("tasks_board", {}).get("project_work_priority") or []
        lines = [
            "Current Bloodlines continuation priority remains the Unity shipping lane inside `unity/`.",
            f"Resume anchor: {anchor.get('label') or 'unresolved'}.",
            f"Recommended next step: {execution.get('recommended_next_step') or 'not generated'}.",
        ]
        if priorities:
            lines.append("Current work spine:")
            lines.extend(f"- {item}" for item in priorities[:4])
        return "\n".join(lines)

    def _run_agent_console_prompt(self, session: dict[str, Any], user_message: str) -> dict[str, Any]:
        context = self._build_agent_console_context(user_message, session)
        task_type = self._select_agent_console_task_type(user_message, context)
        routing = self.routing_policy["task_routing"].get(task_type) or self.routing_policy["task_routing"]["generation"]
        model = routing["primary_model"]
        tool_trace: list[dict[str, Any]] = []
        actions_taken: list[str] = []
        started = time.time()
        degraded_reason = None
        prompt_tokens = 0
        output_tokens = 0
        assistant_payload: dict[str, Any] | None = None

        try:
            for _ in range(4):
                prompt = self._build_agent_console_prompt(user_message, context, tool_trace)
                response = self._ollama_request(
                    "/api/generate",
                    method="POST",
                    payload={
                        "model": model,
                        "stream": False,
                        "format": "json",
                        "prompt": prompt,
                        "options": {"temperature": 0.2, "num_predict": 700},
                    },
                    timeout_seconds=180,
                )
                prompt_tokens += int(response.get("prompt_eval_count", 0) or 0)
                output_tokens += int(response.get("eval_count", 0) or 0)
                assistant_payload = self._extract_json_object(response.get("response", ""))
                mode = str(assistant_payload.get("mode", "final")).strip().lower()
                if mode == "tool":
                    tool_name = assistant_payload.get("tool_name")
                    arguments = assistant_payload.get("arguments", {}) or {}
                    tool_result = self._execute_agent_console_tool(tool_name, arguments)
                    tool_trace.append(tool_result)
                    actions_taken.append(tool_result["summary"])
                    continue
                break
            self._update_degraded_modes(active=False, reason_code="ollama_console_fallback")
        except Exception as exc:
            degraded_reason = "ollama_console_fallback"
            self._update_degraded_modes(active=True, reason_code=degraded_reason)
            assistant_payload = {
                "mode": "final",
                "assistant_message": (
                    "The local model did not complete this turn cleanly, so the console is falling back to the current "
                    "continuity spine. Use /resume, /status, /search, or /read for direct local actions while the model "
                    f"recovers.\n\nFailure detail: {exc}"
                ),
                "confidence": 0.34,
                "citations": ["CURRENT_PROJECT_STATE.md", "NEXT_SESSION_HANDOFF.md", "continuity/PROJECT_STATE.json"],
                "actions_taken": actions_taken[:],
                "unresolved_items": ["Local model generation failed for this turn."],
            }

        if str((assistant_payload or {}).get("mode", "final")).strip().lower() == "tool":
            assistant_payload = {
                "mode": "final",
                "assistant_message": (
                    "The local agent gathered additional context but did not produce a final answer within the current "
                    "tool budget. Review the cited files or continue with a narrower follow-up prompt."
                ),
                "confidence": 0.48,
                "citations": [item["path"] for item in context.get("retrieval_results", [])],
                "actions_taken": actions_taken[:],
                "unresolved_items": ["The model exhausted its tool budget before finalizing a response."],
            }

        wall_time = round(time.time() - started, 3)
        self._record_task_metrics(
            task_type=task_type,
            model=model,
            wall_time=wall_time,
            prompt_tokens=prompt_tokens,
            output_tokens=output_tokens,
            degraded_reason=degraded_reason,
            retrieval_hits=len(context.get("retrieval_results", [])),
        )

        citations = self._unique_paths(
            (assistant_payload.get("citations") or [])
            + [item["path"] for item in context.get("retrieval_results", [])]
            + [item["relative_path"] for item in context.get("explicit_files", [])]
        )
        draft_ids: list[str] = []
        if str(assistant_payload.get("mode", "final")).strip().lower() == "draft_write":
            write_draft = assistant_payload.get("write_draft") or {}
            if write_draft.get("relative_path") and write_draft.get("content") is not None:
                draft = self._create_agent_console_draft(
                    relative_path=str(write_draft.get("relative_path")),
                    reason=str(write_draft.get("reason") or "Agent console draft"),
                    content=str(write_draft.get("content")),
                    citations=citations,
                )
                draft_ids.append(draft["id"])
                actions_taken.append(
                    f"Prepared governed write draft {draft['id']} for {draft['relative_path']} (required {draft['preview'].get('required_tier')})."
                )

        actions_taken.extend(assistant_payload.get("actions_taken") or [])
        actions_taken = self._unique_paths(actions_taken)
        assistant_message = str(assistant_payload.get("assistant_message") or "").strip()
        if not assistant_message:
            assistant_message = self._fallback_agent_console_message(context)

        return {
            "kind": "assistant",
            "assistant_message": assistant_message,
            "citations": citations[:16],
            "actions_taken": actions_taken[:12],
            "routing": {
                "mode": "model",
                "task_type": task_type,
                "model": model,
                "fallback_model": routing.get("fallback_model"),
                "degraded_reason": degraded_reason,
                "tool_steps": len(tool_trace),
            },
            "confidence": assistant_payload.get("confidence", 0.62),
            "draft_ids": draft_ids,
            "unresolved_items": assistant_payload.get("unresolved_items", []),
        }

    def _extract_section_lines(self, relative_path: str, heading_pattern: str, limit: int = 12) -> list[str]:
        absolute_path = (self.bloodlines_root / relative_path).resolve()
        if not absolute_path.exists():
            return []
        capture = False
        lines: list[str] = []
        heading_regex = re.compile(heading_pattern)
        for raw_line in absolute_path.read_text(encoding="utf-8", errors="ignore").splitlines():
            stripped = raw_line.strip()
            if heading_regex.match(stripped):
                capture = True
                continue
            if capture and stripped.startswith("## "):
                break
            if capture and stripped:
                lines.append(stripped)
            if len(lines) >= limit:
                break
        return lines

    def _extract_numbered_lines(self, relative_path: str, heading_pattern: str, limit: int = 10) -> list[str]:
        lines = self._extract_section_lines(relative_path, heading_pattern, limit=limit * 3)
        numbered = [line for line in lines if re.match(r"^\d+\.\s", line)]
        return numbered[:limit]

    def _extract_bullet_lines(self, relative_path: str, heading_pattern: str, limit: int = 12) -> list[str]:
        lines = self._extract_section_lines(relative_path, heading_pattern, limit=limit * 4)
        bullets: list[str] = []
        for line in lines:
            if line.startswith("- "):
                bullets.append(line[2:].strip())
            elif re.match(r"^\d+\.\s", line):
                bullets.append(line)
        return bullets[:limit]

    def _build_execution_packet(
        self,
        tasks_board: dict[str, Any],
        change_report: dict[str, Any],
        source_map: dict[str, Any],
        registry: dict[str, Any],
    ) -> dict[str, Any]:
        owner_direction_path = "governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md"
        latest_unity_handoff = self._latest_matching_relative_path("docs/unity/session-handoffs/*.md")
        project_gap_summary = self._latest_matching_relative_path("reports/*project_completion_handoff*.md")
        continuation_prompt = self._latest_matching_relative_path("03_PROMPTS/CONTINUATION_PROMPT_*.md")

        project_work_priority = tasks_board.get("project_work_priority", [])[:10]
        if not project_work_priority and latest_unity_handoff:
            project_work_priority = self._extract_numbered_lines(latest_unity_handoff, r"^## Next Action$", limit=10)

        current_verified_state = []
        if latest_unity_handoff:
            current_verified_state = self._extract_bullet_lines(latest_unity_handoff, r"^## Current Readiness$", limit=10)
        if not current_verified_state:
            current_verified_state = self._extract_bullet_lines(
                "CURRENT_PROJECT_STATE.md",
                r"^## 2026-04-16 Session 107 Unity Construction Progress Observability And UI$",
                limit=10,
            )

        manual_checklist = []
        if latest_unity_handoff:
            manual_checklist = self._extract_bullet_lines(latest_unity_handoff, r"^## Next Action$", limit=16)

        recommended_next_step = (
            project_work_priority[0]
            if project_work_priority
            else "1. In Unity 6.3 LTS, open unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity."
        )

        canonical_sources = [
            "AGENTS.md",
            "README.md",
            "CLAUDE.md",
            owner_direction_path,
            "CURRENT_PROJECT_STATE.md",
            "NEXT_SESSION_HANDOFF.md",
            "continuity/PROJECT_STATE.json",
        ]
        if latest_unity_handoff:
            canonical_sources.append(latest_unity_handoff)
        if project_gap_summary:
            canonical_sources.append(project_gap_summary)
        if continuation_prompt:
            canonical_sources.append(continuation_prompt)

        return {
            "generated_at": utc_now(),
            "status": "ready",
            "execution_lane": "unity_shipping",
            "owner_direction": {
                "path": owner_direction_path,
                "summary": "Full-canon Unity 6.3 DOTS/ECS delivery remains the non-negotiable path.",
            },
            "scene_target": "unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity",
            "recommended_next_step": recommended_next_step,
            "project_work_priority": project_work_priority,
            "current_verified_state": current_verified_state,
            "manual_verification_checklist": manual_checklist,
            "validation_commands": [
                "node tests/data-validation.mjs",
                "node tests/runtime-bridge.mjs",
                "dotnet build unity/Assembly-CSharp.csproj -nologo",
                "dotnet build unity/Assembly-CSharp-Editor.csproj -nologo",
                "powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1",
                "powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1",
            ],
            "governed_write_targets": [
                "CURRENT_PROJECT_STATE.md",
                "NEXT_SESSION_HANDOFF.md",
                "HANDOFF.md",
                "continuity/PROJECT_STATE.json",
                "unity/README.md",
                "unity/Assets/_Bloodlines/Code/README.md",
            ],
            "canonical_sources": canonical_sources,
            "change_summary": change_report.get("summary", {}),
            "frontier_artifact_count": len(source_map.get("frontier_artifacts", [])),
            "authoritative_source_count": len(registry.get("documents", [])),
            "latest_unity_handoff": latest_unity_handoff,
            "latest_project_gap_summary": project_gap_summary,
            "latest_continuation_prompt": continuation_prompt,
        }

    def _ingest_canonical_subset(self, scan_id: int, checkpoints: dict[str, Any]) -> dict[str, Any]:
        configured_docs = self._current_canonical_subset()
        registry_documents: list[dict[str, Any]] = []
        checkpoint_files = checkpoints.setdefault("files", {})

        with self._connect() as connection:
            connection.execute("DELETE FROM documents")
            connection.execute("DELETE FROM chunks")
            if self.fts_enabled:
                connection.execute("DELETE FROM chunks_fts")

        for item in configured_docs:
            relative_path = item["path"].replace("\\", "/")
            absolute_path = (self.bloodlines_root / relative_path).resolve()
            if not absolute_path.exists():
                raise PlatformError(f"Canonical subset file missing: {relative_path}", status_code=500)
            if self.bloodlines_root not in absolute_path.parents and absolute_path != self.bloodlines_root:
                raise PlatformError(f"Subset path escaped Bloodlines root: {relative_path}", status_code=500)

            payload = absolute_path.read_bytes()
            text = payload.decode("utf-8", errors="ignore")
            stats = absolute_path.stat()
            file_hash = sha256_bytes(payload)
            artifact = self._detect_frontier_artifact(relative_path)
            checkpoint_entry = checkpoint_files.get(relative_path, {})
            change_state = "unchanged" if checkpoint_entry.get("sha256") == file_hash else "modified"
            checkpoint_files[relative_path] = {
                "sha256": file_hash,
                "mtime": iso_from_epoch(stats.st_mtime),
                "size_bytes": stats.st_size,
                "status": "completed",
                "last_completed_at": utc_now(),
            }

            document_entry = {
                "path": relative_path,
                "abs_path": str(absolute_path),
                "classification": item["classification"],
                "authority_score": item["authority_score"],
                "topic": item["topic"],
                "notes": item.get("notes", ""),
                "sha256": file_hash,
                "mtime": iso_from_epoch(stats.st_mtime),
                "mtime_epoch": stats.st_mtime,
                "size_bytes": stats.st_size,
                "change_state": change_state,
                "artifact_provider": artifact["provider"] if artifact else None,
                "artifact_kind": artifact["artifact_kind"] if artifact else None,
            }
            registry_documents.append(document_entry)

            with self._connect() as connection:
                connection.execute(
                    """
                    INSERT OR REPLACE INTO documents
                    (path, abs_path, relative_path, classification, authority_score, topic, notes,
                     sha256, mtime, mtime_iso, size_bytes, source_kind, artifact_provider, artifact_kind, last_scan_id)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                    """,
                    (
                        relative_path,
                        str(absolute_path),
                        relative_path,
                        item["classification"],
                        float(item["authority_score"]),
                        item["topic"],
                        item.get("notes", ""),
                        file_hash,
                        stats.st_mtime,
                        iso_from_epoch(stats.st_mtime),
                        stats.st_size,
                        "canonical_subset",
                        document_entry["artifact_provider"],
                        document_entry["artifact_kind"],
                        scan_id,
                    ),
                )

                chunks = self._chunk_text(relative_path, text)
                for chunk in chunks:
                    connection.execute(
                        """
                        INSERT INTO chunks (doc_path, chunk_index, heading, text, token_estimate)
                        VALUES (?, ?, ?, ?, ?)
                        """,
                        (
                            relative_path,
                            chunk["chunk_index"],
                            chunk["heading"],
                            chunk["text"],
                            chunk["token_estimate"],
                        ),
                    )
                    if self.fts_enabled:
                        connection.execute(
                            "INSERT INTO chunks_fts (doc_path, heading, text) VALUES (?, ?, ?)",
                            (relative_path, chunk["heading"], chunk["text"]),
                        )

        registry_documents.sort(key=lambda item: (item["authority_score"], item["mtime_epoch"]), reverse=True)
        return {
            "generated_at": utc_now(),
            "bloodlines_root": str(self.bloodlines_root),
            "subset_policy": self.scan_settings["subset_ingest_policy"],
            "documents": registry_documents,
            "counts": dict(Counter(item["classification"] for item in registry_documents)),
        }

    def _chunk_text(self, relative_path: str, text: str) -> list[dict[str, Any]]:
        target_chars = int(self.scan_settings["target_chunk_chars"])
        max_chars = int(self.scan_settings["max_chunk_chars"])
        overlap_chars = int(self.scan_settings["chunk_overlap_chars"])

        sections: list[tuple[str, str]] = []
        current_heading = "Document"
        current_lines: list[str] = []
        for line in text.splitlines():
            if line.startswith("#"):
                if current_lines:
                    sections.append((current_heading, "\n".join(current_lines).strip()))
                current_heading = line.lstrip("# ").strip() or "Untitled"
                current_lines = []
            else:
                current_lines.append(line)
        if current_lines:
            sections.append((current_heading, "\n".join(current_lines).strip()))
        if not sections:
            sections = [("Document", text.strip())]

        chunks: list[dict[str, Any]] = []
        chunk_index = 0
        for heading, section_text in sections:
            section_text = section_text.strip()
            if not section_text:
                continue
            start = 0
            while start < len(section_text):
                end = min(start + target_chars, len(section_text))
                if end < len(section_text):
                    window = section_text[start : min(len(section_text), start + max_chars)]
                    break_at = max(window.rfind("\n\n"), window.rfind(". "), window.rfind("\n"))
                    if break_at > target_chars // 2:
                        end = start + break_at + 1
                chunk_text = section_text[start:end].strip()
                if chunk_text:
                    chunks.append(
                        {
                            "doc_path": relative_path,
                            "chunk_index": chunk_index,
                            "heading": heading,
                            "text": chunk_text,
                            "token_estimate": max(1, len(chunk_text) // 4),
                        }
                    )
                    chunk_index += 1
                if end >= len(section_text):
                    break
                start = max(0, end - overlap_chars)
        return chunks

    def _detect_frontier_artifact(self, relative_path: str) -> dict[str, str] | None:
        lowered_parts = [part.lower() for part in PureWindowsPath(relative_path).parts]
        filename = lowered_parts[-1]
        provider = None
        if ".claude" in lowered_parts or filename == "claude.md" or "claude" in filename:
            provider = "claude"
        elif ".codex" in lowered_parts or filename == "agents.md" or "codex" in filename:
            provider = "codex"

        if not provider:
            return None

        artifact_kind = "session_artifact"
        if "handoff" in filename:
            artifact_kind = "handoff"
        elif filename in {"claude.md", "agents.md"}:
            artifact_kind = "session_entry"
        elif "summary" in filename or "report" in filename:
            artifact_kind = "summary"
        elif "session" in filename:
            artifact_kind = "session_trace"
        return {"provider": provider, "artifact_kind": artifact_kind}

    def _latest_scan_id(self) -> int:
        with self._connect() as connection:
            row = connection.execute("SELECT COALESCE(MAX(id), 0) AS max_id FROM scans").fetchone()
            return int(row["max_id"])

    def _build_resume_state(self, source_map: dict[str, Any]) -> dict[str, Any]:
        with self._connect() as connection:
            connection.execute("DELETE FROM resume_candidates")

            local_row = connection.execute(
                """
                SELECT created_at, summary, payload_json
                FROM journal
                WHERE event_type IN ('agent_resume', 'agent_action')
                  AND status = 'completed'
                  AND doctrine_check = 'pass'
                ORDER BY created_at DESC
                LIMIT 1
                """
            ).fetchone()
            frontier_row = connection.execute(
                """
                SELECT path, provider, artifact_kind, mtime_iso, mtime
                FROM artifacts
                ORDER BY mtime DESC
                LIMIT 1
                """
            ).fetchone()
            manual_row = connection.execute(
                """
                SELECT path, mtime_iso, mtime, classification
                FROM documents
                WHERE source_kind = 'canonical_subset'
                ORDER BY mtime DESC
                LIMIT 1
                """
            ).fetchone()

        candidates: list[dict[str, Any]] = []
        if local_row:
            local_payload = json.loads(local_row["payload_json"])
            candidates.append(
                self._insert_resume_candidate(
                    "local_agent_action",
                    "Last successful local agent action",
                    local_payload.get("anchor_path"),
                    local_row["summary"],
                    local_row["created_at"],
                    datetime.fromisoformat(local_row["created_at"].replace("Z", "+00:00")).timestamp(),
                    {"summary": local_row["summary"], "payload": local_payload},
                )
            )
        else:
            candidates.append(self._missing_candidate("local_agent_action", "Last successful local agent action"))

        if frontier_row:
            candidates.append(
                self._insert_resume_candidate(
                    "frontier_artifact",
                    f"Latest {frontier_row['provider']} artifact",
                    frontier_row["path"],
                    frontier_row["artifact_kind"],
                    frontier_row["mtime_iso"],
                    float(frontier_row["mtime"]),
                    {
                        "provider": frontier_row["provider"],
                        "artifact_kind": frontier_row["artifact_kind"],
                        "path": frontier_row["path"],
                    },
                )
            )
        else:
            candidates.append(self._missing_candidate("frontier_artifact", "Latest frontier artifact"))

        if manual_row:
            candidates.append(
                self._insert_resume_candidate(
                    "manual_edit",
                    "Latest ingested manual edit",
                    manual_row["path"],
                    manual_row["classification"],
                    manual_row["mtime_iso"],
                    float(manual_row["mtime"]),
                    {"classification": manual_row["classification"], "path": manual_row["path"]},
                )
            )
        else:
            candidates.append(self._missing_candidate("manual_edit", "Latest ingested manual edit"))

        existing_candidates = [item for item in candidates if item.get("exists")]
        existing_candidates.sort(key=lambda item: item["event_epoch"], reverse=True)

        ambiguous = False
        ambiguity_reason = None
        anchor = None
        if existing_candidates:
            anchor = existing_candidates[0]
            if len(existing_candidates) > 1:
                second = existing_candidates[1]
                if anchor["source_path"] != second["source_path"] and abs(anchor["event_epoch"] - second["event_epoch"]) <= 900:
                    ambiguous = True
                    ambiguity_reason = "multiple_recent_candidates"

        return {
            "generated_at": utc_now(),
            "anchor": anchor,
            "ambiguous": ambiguous,
            "ambiguity_reason": ambiguity_reason,
            "candidates": candidates,
            "recent_frontier_artifacts": source_map.get("frontier_artifacts", [])[:8],
        }

    def _apply_resume_override(self, resume_state: dict[str, Any]) -> dict[str, Any]:
        if not resume_state:
            return {}
        override = self.session_state.get("resume_candidate_override")
        candidates = {
            item["candidate_type"]: item for item in resume_state.get("candidates", []) if item.get("exists")
        }
        effective_anchor = resume_state.get("anchor")
        selection_required = resume_state.get("ambiguous", False)
        if override and override in candidates:
            effective_anchor = candidates[override]
            selection_required = False
        return {
            **resume_state,
            "selected_override": override,
            "effective_anchor": effective_anchor,
            "selection_required": selection_required,
        }

    def set_resume_anchor(self, candidate_type: str | None) -> dict[str, Any]:
        resume_state = self._load_json(self.resume_state_path, {})
        if not resume_state:
            raise PlatformError("Resume state is unavailable.", status_code=404)
        if not candidate_type:
            self.session_state["resume_candidate_override"] = None
            return self._apply_resume_override(resume_state)
        candidates = {
            item["candidate_type"]: item for item in resume_state.get("candidates", []) if item.get("exists")
        }
        if candidate_type not in candidates:
            raise PlatformError(
                "Requested resume candidate is not available.",
                status_code=404,
                payload={"candidate_type": candidate_type},
            )
        self.session_state["resume_candidate_override"] = candidate_type
        selected = candidates[candidate_type]
        self._journal_event(
            event_type="anchor_selection",
            status="completed",
            summary=f"Operator selected {candidate_type} as resume anchor",
            payload={"candidate_type": candidate_type, "source_path": selected.get("source_path")},
            provenance=[selected.get("source_path")] if selected.get("source_path") else [],
        )
        return self._apply_resume_override(resume_state)

    def _insert_resume_candidate(
        self,
        candidate_type: str,
        label: str,
        source_path: str | None,
        source_ref: str | None,
        event_time: str,
        event_epoch: float,
        evidence: dict[str, Any],
    ) -> dict[str, Any]:
        with self._connect() as connection:
            connection.execute(
                """
                INSERT OR REPLACE INTO resume_candidates
                (candidate_type, label, source_path, source_ref, event_time, event_epoch, evidence_json)
                VALUES (?, ?, ?, ?, ?, ?, ?)
                """,
                (candidate_type, label, source_path, source_ref, event_time, event_epoch, json.dumps(evidence)),
            )
        return {
            "candidate_type": candidate_type,
            "label": label,
            "source_path": source_path,
            "source_ref": source_ref,
            "event_time": event_time,
            "event_epoch": event_epoch,
            "evidence": evidence,
            "exists": True,
        }

    def _missing_candidate(self, candidate_type: str, label: str) -> dict[str, Any]:
        return {
            "candidate_type": candidate_type,
            "label": label,
            "source_path": None,
            "source_ref": None,
            "event_time": None,
            "event_epoch": 0.0,
            "evidence": {},
            "exists": False,
        }

    def _candidate_age_hours(self, candidates: list[dict[str, Any]]) -> float | None:
        timestamps = [item["event_epoch"] for item in candidates if item.get("exists") and item.get("event_epoch")]
        if not timestamps:
            return None
        oldest = min(timestamps)
        return round((time.time() - oldest) / 3600, 2)

    def _build_discovered_registry(
        self,
        scan_id: int,
        checkpoints: dict[str, Any],
        candidates: list[dict[str, Any]],
    ) -> dict[str, Any]:
        canonical_lookup = {
            item["path"].replace("\\", "/"): item
            for item in self.source_subset.get("canonical_subset", [])
        }
        checkpoint_files = checkpoints.setdefault("files", {})
        discovered_documents: list[dict[str, Any]] = []

        with self._connect() as connection:
            connection.execute("DELETE FROM discovered_documents")

        for candidate in candidates:
            relative_path = candidate["path"]
            absolute_path = Path(candidate["abs_path"])
            try:
                payload = absolute_path.read_bytes()
            except OSError:
                continue
            text = payload.decode("utf-8", errors="ignore")
            file_hash = sha256_bytes(payload)
            canonical_entry = canonical_lookup.get(relative_path)
            authority_score, classification, reason = self._score_discovered_document(
                relative_path=relative_path,
                text=text,
                candidate=candidate,
                canonical_entry=canonical_entry,
            )
            checkpoint_files[relative_path] = {
                "sha256": file_hash,
                "mtime": candidate["mtime"],
                "size_bytes": candidate["size_bytes"],
                "status": "completed",
                "last_completed_at": utc_now(),
            }
            excerpt = re.sub(r"\s+", " ", text[:280]).strip()
            document_entry = {
                "path": relative_path,
                "classification": classification,
                "authority_score": authority_score,
                "sha256": file_hash,
                "mtime": candidate["mtime"],
                "mtime_epoch": candidate["mtime_epoch"],
                "size_bytes": candidate["size_bytes"],
                "top_level": candidate["top_level"],
                "extension": candidate["extension"],
                "reason": reason,
                "excerpt": excerpt,
                "is_canonical_subset": bool(canonical_entry),
            }
            discovered_documents.append(document_entry)
            with self._connect() as connection:
                connection.execute(
                    """
                    INSERT OR REPLACE INTO discovered_documents
                    (path, abs_path, classification, authority_score, sha256, mtime, mtime_iso,
                     size_bytes, top_level, extension, reason, excerpt, is_canonical_subset, last_scan_id)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                    """,
                    (
                        relative_path,
                        str(absolute_path),
                        classification,
                        authority_score,
                        file_hash,
                        candidate["mtime_epoch"],
                        candidate["mtime"],
                        candidate["size_bytes"],
                        candidate["top_level"],
                        candidate["extension"],
                        reason,
                        excerpt,
                        1 if canonical_entry else 0,
                        scan_id,
                    ),
                )

        conflict_clusters = self._build_conflict_clusters(discovered_documents)
        conflict_lookup = {}
        for cluster in conflict_clusters:
            if cluster["state"] == "likely_canonical":
                continue
            for entry in cluster["entries"][1:]:
                conflict_lookup[entry["path"]] = cluster["state"]

        for document in discovered_documents:
            if document["path"] in conflict_lookup and document["classification"] not in {"archival", "ignored"}:
                document["classification"] = "conflicting"
                document["reason"] = f"{document['reason']}; {conflict_lookup[document['path']]}"

        discovered_documents.sort(key=lambda item: (item["authority_score"], item["mtime_epoch"]), reverse=True)
        return {
            "generated_at": utc_now(),
            "total_documents": len(discovered_documents),
            "counts": dict(Counter(item["classification"] for item in discovered_documents)),
            "documents": discovered_documents,
            "conflict_clusters": conflict_clusters,
        }

    def _combined_document_map(
        self,
        registry: dict[str, Any],
        discovered_registry: dict[str, Any],
    ) -> dict[str, dict[str, Any]]:
        combined: dict[str, dict[str, Any]] = {}
        for document in discovered_registry.get("documents", []):
            combined[document["path"]] = {
                "path": document["path"],
                "classification": document.get("classification", "secondary"),
                "authority_score": float(document.get("authority_score", 0)),
                "sha256": document.get("sha256"),
                "mtime": document.get("mtime"),
                "mtime_epoch": document.get("mtime_epoch", 0),
                "size_bytes": document.get("size_bytes", 0),
                "reason": document.get("reason", ""),
                "source": "discovered",
                "topic": document.get("top_level"),
                "artifact_provider": None,
                "artifact_kind": None,
            }
        for document in registry.get("documents", []):
            combined[document["path"]] = {
                **combined.get(document["path"], {}),
                "path": document["path"],
                "classification": document.get("classification", "secondary"),
                "authority_score": float(document.get("authority_score", 0)),
                "sha256": document.get("sha256"),
                "mtime": document.get("mtime"),
                "mtime_epoch": document.get("mtime_epoch", 0),
                "size_bytes": document.get("size_bytes", 0),
                "reason": document.get("notes", "configured canonical source"),
                "source": "canonical",
                "topic": document.get("topic"),
                "artifact_provider": document.get("artifact_provider"),
                "artifact_kind": document.get("artifact_kind"),
            }
        return combined

    def _build_change_report(
        self,
        previous_source_map: dict[str, Any],
        previous_registry: dict[str, Any],
        previous_discovered_registry: dict[str, Any],
        source_map: dict[str, Any],
        registry: dict[str, Any],
        discovered_registry: dict[str, Any],
    ) -> dict[str, Any]:
        previous_documents = self._combined_document_map(previous_registry, previous_discovered_registry)
        current_documents = self._combined_document_map(registry, discovered_registry)
        changed_documents: list[dict[str, Any]] = []

        for path, document in current_documents.items():
            previous = previous_documents.get(path)
            if previous is None:
                change_type = "added"
            elif previous.get("sha256") != document.get("sha256"):
                change_type = "modified"
            elif (
                previous.get("classification") != document.get("classification")
                or previous.get("authority_score") != document.get("authority_score")
            ):
                change_type = "reclassified"
            else:
                continue
            changed_documents.append(
                {
                    "path": path,
                    "change_type": change_type,
                    "classification": document.get("classification"),
                    "authority_score": document.get("authority_score"),
                    "mtime": document.get("mtime"),
                    "mtime_epoch": document.get("mtime_epoch", 0),
                    "reason": document.get("reason", ""),
                    "source": document.get("source"),
                    "topic": document.get("topic"),
                }
            )

        for path, document in previous_documents.items():
            if path in current_documents:
                continue
            if self._is_noise_path(path):
                continue
            changed_documents.append(
                {
                    "path": path,
                    "change_type": "removed",
                    "classification": document.get("classification"),
                    "authority_score": document.get("authority_score"),
                    "mtime": document.get("mtime"),
                    "mtime_epoch": document.get("mtime_epoch", 0),
                    "reason": document.get("reason", ""),
                    "source": document.get("source"),
                    "topic": document.get("topic"),
                }
            )

        previous_frontier = {
            item["path"]: item for item in previous_source_map.get("frontier_artifacts", []) if item.get("path")
        }
        frontier_updates = []
        for artifact in source_map.get("frontier_artifacts", []):
            previous = previous_frontier.get(artifact["path"])
            if previous is None:
                change_type = "added"
            elif previous.get("sha256") != artifact.get("sha256"):
                change_type = "modified"
            else:
                continue
            frontier_updates.append(
                {
                    "path": artifact["path"],
                    "provider": artifact.get("provider"),
                    "artifact_kind": artifact.get("artifact_kind"),
                    "mtime": artifact.get("mtime"),
                    "change_type": change_type,
                }
            )

        if not changed_documents:
            for item in source_map.get("recent_changes", [])[:16]:
                changed_documents.append(
                    {
                        "path": item["path"],
                        "change_type": "recent",
                        "classification": item.get("classification"),
                        "authority_score": current_documents.get(item["path"], {}).get("authority_score", 0),
                        "mtime": item.get("mtime"),
                        "mtime_epoch": item.get("mtime_epoch", 0),
                        "reason": "recently_touched",
                        "source": current_documents.get(item["path"], {}).get("source", "discovered"),
                        "topic": current_documents.get(item["path"], {}).get("topic"),
                    }
                )

        authoritative_updates = [
            item
            for item in changed_documents
            if item["source"] == "canonical"
            or item["classification"] in {"authoritative", "conflicting"}
            or float(item.get("authority_score", 0)) >= 80
        ]
        authoritative_updates.sort(
            key=lambda item: (float(item.get("authority_score", 0)), item.get("mtime_epoch", 0)),
            reverse=True,
        )

        changed_paths = {item["path"] for item in changed_documents}
        conflict_watch = []
        for cluster in discovered_registry.get("conflict_clusters", []):
            meaningful_entries = [
                entry
                for entry in cluster.get("entries", [])
                if entry.get("classification") != "archival" and not self._is_noise_path(entry.get("path", ""))
            ]
            if not meaningful_entries:
                continue
            entry_paths = {entry["path"] for entry in meaningful_entries}
            if cluster.get("state") != "likely_canonical" or changed_paths.intersection(entry_paths):
                conflict_watch.append(
                    {
                        **cluster,
                        "entries": meaningful_entries[:8],
                    }
                )

        hot_spots = Counter()
        for item in changed_documents:
            top_level = item["path"].split("/", 1)[0] if "/" in item["path"] else "."
            hot_spots[top_level] += 1

        changed_documents.sort(
            key=lambda item: (
                float(item.get("authority_score", 0)),
                item.get("mtime_epoch", 0),
                item.get("change_type") == "removed",
            ),
            reverse=True,
        )
        return {
            "generated_at": utc_now(),
            "summary": {
                "changed_documents": len(changed_documents),
                "authoritative_updates": len(authoritative_updates),
                "frontier_session_updates": len(frontier_updates),
                "conflict_watch_count": len(conflict_watch),
                "added": len([item for item in changed_documents if item["change_type"] == "added"]),
                "modified": len([item for item in changed_documents if item["change_type"] == "modified"]),
                "removed": len([item for item in changed_documents if item["change_type"] == "removed"]),
            },
            "high_signal_changes": changed_documents[:24],
            "authoritative_updates": authoritative_updates[:18],
            "frontier_session_updates": frontier_updates[:18],
            "conflict_watch": conflict_watch[:12],
            "hot_spots": [
                {"segment": segment, "changes": count}
                for segment, count in hot_spots.most_common(10)
            ],
        }

    def _score_discovered_document(
        self,
        relative_path: str,
        text: str,
        candidate: dict[str, Any],
        canonical_entry: dict[str, Any] | None,
    ) -> tuple[float, str, str]:
        if canonical_entry:
            return float(canonical_entry["authority_score"]), canonical_entry["classification"], "configured_canonical_subset"

        lowered = relative_path.lower()
        score = 0.2
        reasons = []
        top_level = candidate["top_level"].lower()
        filename = PureWindowsPath(relative_path).name.lower()
        if top_level in {"01_canon", "18_exports", "continuity"}:
            score += 0.38
            reasons.append("canonical_path")
        elif top_level in {"tasks", "docs", "03_prompts", "00_admin", "governance"}:
            score += 0.2
            reasons.append("continuity_support_path")
        elif top_level in {"src", "data"}:
            score += 0.18
            reasons.append("runtime_source_path")
        elif top_level == "unity":
            score += 0.14
            reasons.append("unity_source_path")

        if any(token in filename for token in ("current_project_state", "next_session_handoff", "master", "design_bible", "doctrine", "source_provenance", "definitive_decisions")):
            score += 0.18
            reasons.append("authoritative_name")
        if "state_of_game_report" in filename or "continuation_prompt" in filename:
            score += 0.1
            reasons.append("continuity_report")
        if candidate["extension"] in {".cs", ".js", ".ts", ".tsx", ".py"}:
            score += 0.08
            reasons.append("implementation_file")

        age_hours = max((time.time() - candidate["mtime_epoch"]) / 3600, 0.0)
        if age_hours <= 24:
            score += 0.15
            reasons.append("updated_within_24h")
        elif age_hours <= 72:
            score += 0.1
            reasons.append("updated_within_72h")
        elif age_hours <= 168:
            score += 0.05
            reasons.append("updated_within_week")

        if any(token in lowered for token in ("19_archive", "_archive", "archive_preserved_sources")):
            score -= 0.32
            reasons.append("archive_penalty")
        if any(token in lowered for token in ("draft", "scratch", ".tmp-", "tmp-edge", "test-results")):
            score -= 0.2
            reasons.append("working_state_penalty")
        if "continuation-platform/" in lowered:
            score = 0.0
            return score, "ignored", "platform_internal"

        score = max(0.0, min(round(score, 3), 1.0))
        classification = "secondary"
        if any(token in lowered for token in ("19_archive", "_archive", "archive_preserved_sources")):
            classification = "archival"
        elif score >= 0.82:
            classification = "authoritative"
        elif score < 0.18:
            classification = "ignored"
        return score, classification, ",".join(reasons) or "heuristic_default"

    def _build_conflict_clusters(self, discovered_documents: list[dict[str, Any]]) -> list[dict[str, Any]]:
        clusters: dict[str, list[dict[str, Any]]] = defaultdict(list)
        for document in discovered_documents:
            key = self._normalize_conflict_key(document["path"])
            if not key:
                continue
            clusters[key].append(document)

        output = []
        for key, entries in clusters.items():
            if len(entries) < 2:
                continue
            ranked = sorted(entries, key=lambda item: (item["authority_score"], item["mtime_epoch"]), reverse=True)
            state = "likely_canonical"
            if len(ranked) > 1:
                gap = ranked[0]["authority_score"] - ranked[1]["authority_score"]
                if gap <= 0.05:
                    state = "needs_review"
                elif any(item["classification"] not in {"archival", "ignored"} for item in ranked[1:]):
                    state = "merge_candidate"
            output.append(
                {
                    "cluster_key": key,
                    "state": state,
                    "canonical_candidate": ranked[0]["path"],
                    "entries": [
                        {
                            "path": item["path"],
                            "classification": item["classification"],
                            "authority_score": item["authority_score"],
                            "mtime": item["mtime"],
                        }
                        for item in ranked[:8]
                    ],
                }
            )
        output.sort(key=lambda item: (item["state"] != "needs_review", -len(item["entries"])))
        return output

    def _normalize_conflict_key(self, relative_path: str) -> str:
        stem = Path(relative_path).stem.lower()
        stem = re.sub(r"20\d{2}[-_]\d{2}[-_]\d{2}", "", stem)
        stem = re.sub(r"session[-_ ]?\d+", "", stem)
        stem = re.sub(r"v\d+(\.\d+)?", "", stem)
        stem = re.sub(r"[^a-z0-9]+", " ", stem).strip()
        tokens = [token for token in stem.split() if token not in {"bloodlines", "the", "and"}]
        return " ".join(tokens[:8])

    def _build_tasks_board(self) -> dict[str, Any]:
        todo_path = self.bloodlines_root / "tasks" / "todo.md"
        handoff_path = self.bloodlines_root / "NEXT_SESSION_HANDOFF.md"
        project_handoff_path = self.bloodlines_root / "HANDOFF.md"
        open_tasks = []
        completed_count = 0
        if todo_path.exists():
            heading = "General"
            for line in todo_path.read_text(encoding="utf-8", errors="ignore").splitlines():
                if line.startswith("#"):
                    heading = line.lstrip("# ").strip() or heading
                elif line.startswith("- [ ] "):
                    open_tasks.append({"heading": heading, "task": line[6:].strip()})
                elif line.startswith("- [x] "):
                    completed_count += 1

        handoff_priority = []
        project_work_priority = []
        if handoff_path.exists():
            capture_immediate = False
            immediate_detail = []
            session_sections: list[tuple[int, list[str]]] = []
            capture_project = False
            active_session_number = None
            active_session_lines: list[str] = []
            for line in handoff_path.read_text(encoding="utf-8", errors="ignore").splitlines():
                stripped = line.strip()
                if stripped == "## Immediate Next Action Priority":
                    capture_immediate = True
                    capture_project = False
                    continue
                session_match = re.match(r"^## Session (\d+) Next Action Priority$", stripped)
                if session_match:
                    if active_session_number is not None and active_session_lines:
                        session_sections.append((active_session_number, active_session_lines[:]))
                    active_session_number = int(session_match.group(1))
                    active_session_lines = []
                    capture_project = True
                    capture_immediate = False
                    continue
                if stripped.startswith("## "):
                    if active_session_number is not None and active_session_lines:
                        session_sections.append((active_session_number, active_session_lines[:]))
                        active_session_number = None
                        active_session_lines = []
                    capture_immediate = False
                    capture_project = False
                if capture_immediate and re.match(r"^\d+\.\s", stripped):
                    handoff_priority.append(stripped)
                if capture_immediate and stripped.startswith("- "):
                    immediate_detail.append(stripped[2:].strip())
                if capture_project and re.match(r"^\d+\.\s", stripped):
                    active_session_lines.append(stripped)
            if active_session_number is not None and active_session_lines:
                session_sections.append((active_session_number, active_session_lines[:]))
            if immediate_detail:
                project_work_priority = immediate_detail[:12]
            if session_sections:
                latest_session_number, latest_session_lines = max(session_sections, key=lambda item: item[0])
                if latest_session_number >= 0 and not project_work_priority:
                    project_work_priority = latest_session_lines[:12]
            if not project_work_priority and handoff_priority:
                project_work_priority = handoff_priority[:12]

        if project_handoff_path.exists():
            capture_project_handoff = False
            parsed_project_handoff_lines = []
            for line in project_handoff_path.read_text(encoding="utf-8", errors="ignore").splitlines():
                stripped = line.strip()
                if stripped == "### Next Action":
                    capture_project_handoff = True
                    continue
                if capture_project_handoff and stripped.startswith("#"):
                    break
                if capture_project_handoff and re.match(r"^\d+\.\s", stripped):
                    parsed_project_handoff_lines.append(stripped)
            if parsed_project_handoff_lines and not project_work_priority:
                project_work_priority = parsed_project_handoff_lines[:12]

        recommended = None
        if self.last_agent_result:
            recommended = self.last_agent_result.get("recommended_next_step")

        return {
            "generated_at": utc_now(),
            "open_tasks": open_tasks[:60],
            "open_task_count": len(open_tasks),
            "completed_task_count": completed_count,
            "handoff_priority": handoff_priority[:12],
            "project_work_priority": project_work_priority[:12],
            "recommended_next_step": recommended,
        }

    def get_model_inventory(self, force_refresh: bool = False) -> dict[str, Any]:
        if self.model_inventory_path.exists() and not force_refresh:
            return self._load_json(self.model_inventory_path)

        inventory = {
            "generated_at": utc_now(),
            "ollama_base_url": self.routing_policy.get("ollama_base_url"),
            "status": "offline",
            "models": [],
        }
        try:
            tag_response = self._ollama_request("/api/tags", method="GET")
            models = []
            for item in tag_response.get("models", []):
                model_name = item["name"]
                show_payload = self._ollama_request(
                    "/api/show", method="POST", payload={"model": model_name}
                )
                model_info = show_payload.get("model_info", {})
                models.append(
                    {
                        "name": model_name,
                        "modified_at": item.get("modified_at"),
                        "size": item.get("size"),
                        "digest": item.get("digest"),
                        "family": model_info.get("general.architecture") or model_info.get("family"),
                        "parameter_size": show_payload.get("details", {}).get("parameter_size"),
                        "quantization_level": show_payload.get("details", {}).get("quantization_level"),
                        "context_window": self._extract_model_metric(model_info, "context_length"),
                        "embedding_length": self._extract_model_metric(model_info, "embedding_length"),
                        "capabilities": show_payload.get("capabilities", []),
                        "routing_roles": self._routing_roles_for_model(model_name),
                    }
                )
            inventory["status"] = "online"
            inventory["models"] = models
            self._update_degraded_modes(active=False, reason_code="ollama_unreachable")
        except Exception as exc:
            inventory["error"] = str(exc)
            self._update_degraded_modes(active=True, reason_code="ollama_unreachable")

        self._write_json(self.model_inventory_path, inventory)
        return inventory

    def _extract_model_metric(self, model_info: dict[str, Any], suffix: str) -> Any:
        for key, value in model_info.items():
            if key.endswith(suffix):
                return value
        return None

    def _routing_roles_for_model(self, model_name: str) -> list[str]:
        assignments = []
        for task_type, config in self.routing_policy.get("task_routing", {}).items():
            if config.get("primary_model") == model_name or config.get("fallback_model") == model_name:
                assignments.append(task_type)
        return assignments

    def _ollama_request(
        self,
        path: str,
        method: str = "GET",
        payload: dict[str, Any] | None = None,
        timeout_seconds: int = 45,
    ) -> dict[str, Any]:
        base = self.routing_policy["ollama_base_url"].rstrip("/")
        body = None
        headers = {}
        if payload is not None:
            body = json.dumps(payload).encode("utf-8")
            headers["Content-Type"] = "application/json"
        request = urllib.request.Request(f"{base}{path}", data=body, method=method, headers=headers)
        with urllib.request.urlopen(request, timeout=timeout_seconds) as response:
            return json.loads(response.read().decode("utf-8"))

    def get_dashboard_snapshot(self) -> dict[str, Any]:
        source_map = self._load_json(self.source_map_path, {})
        registry = self._load_json(self.registry_path, {"documents": []})
        discovered_registry = self._load_json(
            self.discovered_registry_path,
            {"total_documents": 0, "counts": {}, "documents": [], "conflict_clusters": []},
        )
        change_report = self.get_change_report()
        resume_state = self._apply_resume_override(self._load_json(self.resume_state_path, {}))
        model_inventory = self.get_model_inventory(force_refresh=False)
        timeline = self.get_timeline(limit=16)
        config_summary = self.get_configuration_summary()
        tasks_board = self._load_json(self.tasks_board_path, {"open_tasks": [], "handoff_priority": []})
        execution_packet = self.get_execution_packet()
        return {
            "generated_at": utc_now(),
            "status": {
                "continuity_health": "attention" if resume_state.get("selection_required") else "ready",
                "last_scan_time": self.telemetry["ingestion"]["last_scan_time"],
                "last_scan_duration_seconds": self.telemetry["ingestion"]["last_scan_duration_seconds"],
                "session_started_at": self.session_state["started_at"],
                "offline_posture": "offline_first",
            },
            "source_map": source_map,
            "registry": registry,
            "discovered_registry": {
                "total_documents": discovered_registry.get("total_documents", 0),
                "counts": discovered_registry.get("counts", {}),
                "documents": discovered_registry.get("documents", [])[:140],
                "conflict_clusters": discovered_registry.get("conflict_clusters", [])[:40],
            },
            "change_report": change_report,
            "resume_state": resume_state,
            "model_inventory": model_inventory,
            "write_posture": self.get_write_posture(),
            "telemetry": self.telemetry,
            "telemetry_drilldown": self.get_telemetry_drilldown(),
            "timeline": timeline,
            "configuration": config_summary,
            "last_agent_result": self.last_agent_result,
            "tasks_board": tasks_board,
            "execution_packet": execution_packet,
            "handoff_preview": self.get_handoff_preview(),
            "handoff_builder": self.get_handoff_builder_state(),
            "environment": self.get_environment_status(),
            "agent_console": self.get_agent_console_state(),
        }

    def get_execution_packet(self) -> dict[str, Any]:
        return self._load_json(self.execution_packet_path, {})

    def get_configuration_summary(self) -> dict[str, Any]:
        return {
            "runtime": self.routing_policy.get("runtime"),
            "scan_settings": self.scan_settings,
            "routing_policy": self.routing_policy.get("task_routing", {}),
            "optional_bridges": self.routing_policy.get("optional_bridges", {}),
            "doctrine_rules": self.doctrine_rules.get("rules", []),
            "fts_enabled": self.fts_enabled,
        }

    def get_write_posture(self) -> dict[str, Any]:
        return {
            **self.session_state,
            "tier_requirements": {
                "tier_3": "In-scope Bloodlines project writes",
                "tier_4": "Governance writes, cross-folder operations, irreversible operations",
            },
        }

    def get_change_report(self) -> dict[str, Any]:
        if self.change_report_path.exists():
            return self._load_json(self.change_report_path)
        source_map = self._load_json(self.source_map_path, {})
        return {
            "generated_at": utc_now(),
            "summary": {
                "changed_documents": len(source_map.get("recent_changes", [])[:16]),
                "authoritative_updates": 0,
                "frontier_session_updates": len(source_map.get("frontier_artifacts", [])[:8]),
                "conflict_watch_count": len(source_map.get("potential_conflicts", [])[:8]),
                "added": 0,
                "modified": 0,
                "removed": 0,
            },
            "high_signal_changes": [
                {
                    "path": item["path"],
                    "change_type": "recent",
                    "classification": item.get("classification"),
                    "authority_score": 0,
                    "mtime": item.get("mtime"),
                    "mtime_epoch": item.get("mtime_epoch", 0),
                    "reason": "recently_touched",
                    "source": "discovered",
                    "topic": item["path"].split("/", 1)[0] if "/" in item["path"] else ".",
                }
                for item in source_map.get("recent_changes", [])[:16]
            ],
            "authoritative_updates": [],
            "frontier_session_updates": source_map.get("frontier_artifacts", [])[:8],
            "conflict_watch": source_map.get("potential_conflicts", [])[:8],
            "hot_spots": [],
        }

    def get_telemetry_drilldown(self) -> dict[str, Any]:
        with self._connect() as connection:
            write_rows = connection.execute(
                """
                SELECT created_at, target_path, execution_mode, approval_state, reason_code, source_basis
                FROM write_events
                ORDER BY id DESC
                LIMIT 12
                """
            ).fetchall()
            scan_rows = connection.execute(
                """
                SELECT started_at, completed_at, status, mode, files_scanned, subset_docs_ingested, notes
                FROM scans
                ORDER BY id DESC
                LIMIT 8
                """
            ).fetchall()
            journal_rows = connection.execute(
                """
                SELECT event_type, COUNT(*) AS count
                FROM journal
                GROUP BY event_type
                ORDER BY count DESC
                """
            ).fetchall()

        return {
            "generated_at": utc_now(),
            "summary": {
                "recent_task_count": len(self.telemetry.get("recent_tasks", [])),
                "active_degraded_modes": len(self.telemetry.get("degraded_modes", {}).get("active", [])),
                "write_refusals": self.telemetry.get("writes", {}).get("refused_count", 0),
                "recent_queries": len(self.telemetry.get("retrieval", {}).get("recent_queries", [])),
            },
            "recent_tasks": self.telemetry.get("recent_tasks", [])[-10:][::-1],
            "retrieval": {
                "last_hit_rate": self.telemetry.get("retrieval", {}).get("last_hit_rate", 0),
                "top_result_provenance_distribution": self.telemetry.get("retrieval", {}).get(
                    "top_result_provenance_distribution", {}
                ),
                "recent_queries": list(reversed(self.telemetry.get("retrieval", {}).get("recent_queries", [])[-10:])),
            },
            "routing": {
                "decisions_by_task_type": self.telemetry.get("routing", {}).get("decisions_by_task_type", {}),
                "last_assignments": self.telemetry.get("routing", {}).get("last_assignments", {}),
            },
            "degraded_modes": self.telemetry.get("degraded_modes", {}),
            "writes": {
                "summary": self.telemetry.get("writes", {}),
                "recent_events": [
                    {
                        "created_at": row["created_at"],
                        "target_path": row["target_path"],
                        "execution_mode": row["execution_mode"],
                        "approval_state": row["approval_state"],
                        "reason_code": row["reason_code"],
                        "source_basis": row["source_basis"],
                    }
                    for row in write_rows
                ],
            },
            "recent_scans": [
                {
                    "started_at": row["started_at"],
                    "completed_at": row["completed_at"],
                    "status": row["status"],
                    "mode": row["mode"],
                    "files_scanned": row["files_scanned"],
                    "subset_docs_ingested": row["subset_docs_ingested"],
                    "notes": row["notes"],
                }
                for row in scan_rows
            ],
            "journal_breakdown": {row["event_type"]: row["count"] for row in journal_rows},
        }

    def get_handoff_builder_state(self) -> dict[str, Any]:
        resume_state = self._load_json(self.resume_state_path, {})
        agent_result = self.last_agent_result or self._load_json(self.last_agent_result_path, {})
        registry = self._load_json(self.registry_path, {"documents": []})
        tasks_board = self._load_json(self.tasks_board_path, {"open_tasks": [], "handoff_priority": []})
        change_report = self.get_change_report()
        doctrine_rules = [rule["rule"] for rule in self.doctrine_rules.get("rules", [])]
        frontier_escalations = []
        if agent_result.get("frontier_handoff_candidate"):
            frontier_escalations.append(
                {
                    "reason": agent_result.get("frontier_handoff_reason") or "local capability ceiling reached",
                    "mode": agent_result.get("recommended_mode"),
                }
            )

        briefing_lines = [
            f"Resume anchor: {resume_state.get('anchor', {}).get('label', 'unresolved')}",
            f"Recommended next step: {agent_result.get('recommended_next_step', 'not generated')}",
            f"Changed files since last baseline: {change_report.get('summary', {}).get('changed_documents', 0)}",
            f"Open task count: {tasks_board.get('open_task_count', 0)}",
            f"Conflict watch count: {change_report.get('summary', {}).get('conflict_watch_count', 0)}",
        ]
        suggested_prompt = "\n".join(
            [
                "Continue Bloodlines from this local continuity handoff.",
                "",
                "Resume anchor:",
                json.dumps(resume_state.get("anchor", {}), indent=2),
                "",
                "Recommended next step:",
                agent_result.get("recommended_next_step", "No recommendation generated yet."),
                "",
                "Reasoning summary:",
                agent_result.get("reasoning_summary", "No agent summary generated yet."),
                "",
                "High-signal changes:",
                *[
                    f"- {item['change_type']}: {item['path']} ({item.get('classification', 'unknown')})"
                    for item in change_report.get("high_signal_changes", [])[:8]
                ],
                "",
                "Open work queue:",
                *[
                    f"- {item}"
                    for item in (
                        tasks_board.get("project_work_priority")
                        or tasks_board.get("handoff_priority")
                        or [task["task"] for task in tasks_board.get("open_tasks", [])[:8]]
                    )[:8]
                ],
                "",
                "Doctrine rules to preserve:",
                *[f"- {rule}" for rule in doctrine_rules[:8]],
            ]
        )
        return {
            "generated_at": utc_now(),
            "anchor": resume_state.get("anchor"),
            "continuity_delta": {
                "summary": change_report.get("summary", {}),
                "high_signal_changes": change_report.get("high_signal_changes", [])[:8],
                "frontier_session_updates": change_report.get("frontier_session_updates", [])[:8],
            },
            "open_work": {
                "project_work_priority": tasks_board.get("project_work_priority", [])[:8],
                "handoff_priority": tasks_board.get("handoff_priority", [])[:8],
                "open_tasks": tasks_board.get("open_tasks", [])[:10],
                "recommended_next_step": agent_result.get("recommended_next_step"),
            },
            "canonical_sources": registry.get("documents", [])[:10],
            "doctrine_rules": doctrine_rules,
            "frontier_escalations": frontier_escalations,
            "briefing_lines": briefing_lines,
            "suggested_prompt": suggested_prompt,
        }

    def get_handoff_preview(self) -> dict[str, Any]:
        if not self.handoff_path.exists():
            return {"path": str(self.handoff_path), "content": ""}
        return {"path": str(self.handoff_path), "content": self.handoff_path.read_text(encoding="utf-8", errors="ignore")}

    def unlock_write_posture(self, phrase: str) -> dict[str, Any]:
        phrase_hash = sha256_bytes(phrase.encode("utf-8"))
        matched_tier = None
        tier_hashes = self.tier_gate_hashes.get("tiers", {})
        for tier_name, expected_hash in tier_hashes.items():
            if phrase_hash == expected_hash:
                matched_tier = tier_name
                break
        if not matched_tier:
            self._record_write_event(
                target_path="[unlock_attempt]",
                execution_mode="unlock",
                approval_state="refused",
                reason_code="tier_insufficient",
                source_basis="operator phrase hash mismatch",
                details={"attempted_hash": phrase_hash[:12]},
            )
            raise PlatformError(
                "Unlock phrase did not match a configured tier gate.",
                status_code=403,
                payload={"reason_code": "tier_insufficient"},
            )

        active_tier = "tier_3" if matched_tier == "tier3" else "tier_4"
        self.session_state["write_posture"] = "approved_write"
        self.session_state["active_tier"] = active_tier
        self.session_state["project_writes_allowed"] = True
        self.session_state["unlock_reason"] = "operator_confirmed"
        self.session_state["last_unlock_at"] = utc_now()
        self._update_degraded_modes(active=False, reason_code="write_locked")
        self._record_write_event(
            target_path="[unlock_success]",
            execution_mode="unlock",
            approval_state="approved",
            reason_code="unlocked",
            source_basis="operator confirmed tier gate",
            details={"active_tier": active_tier},
        )
        return self.get_write_posture()

    def _record_write_event(
        self,
        target_path: str,
        execution_mode: str,
        approval_state: str,
        reason_code: str,
        source_basis: str,
        details: dict[str, Any],
    ) -> None:
        created_at = utc_now()
        with self._connect() as connection:
            connection.execute(
                """
                INSERT INTO write_events
                (created_at, target_path, execution_mode, approval_state, reason_code, source_basis, details_json)
                VALUES (?, ?, ?, ?, ?, ?, ?)
                """,
                (
                    created_at,
                    target_path,
                    execution_mode,
                    approval_state,
                    reason_code,
                    source_basis,
                    json.dumps(details),
                ),
            )
        self.telemetry["writes"]["count"] += 1
        if approval_state == "approved":
            self.telemetry["writes"]["approved_count"] += 1
        else:
            self.telemetry["writes"]["refused_count"] += 1
        self.telemetry["writes"]["last_reason_code"] = reason_code
        self._persist_telemetry()

    def project_write_probe(
        self,
        relative_path: str,
        content: str,
        reason: str,
        source_basis: str,
        expected_sha256: str | None = None,
    ) -> dict[str, Any]:
        normalized = relative_path.replace("\\", "/").strip().lstrip("/")
        target = self._safe_project_target(normalized)
        required_tier = self._required_tier_for_path(relative_path)
        active_tier = self.session_state.get("active_tier")
        if not self.session_state.get("project_writes_allowed") or not self._tier_meets_requirement(active_tier, required_tier):
            self._record_write_event(
                target_path=normalized,
                execution_mode="project_write",
                approval_state="refused",
                reason_code="tier_insufficient",
                source_basis=source_basis,
                details={"required_tier": required_tier, "reason": reason},
            )
            raise PlatformError(
                "Write posture is locked or below the required tier.",
                status_code=403,
                payload={"reason_code": "tier_insufficient", "required_tier": required_tier},
            )

        current_sha256 = self._hash_file(target) if target.exists() else None
        if expected_sha256 is not None and current_sha256 != expected_sha256:
            self._record_write_event(
                target_path=normalized,
                execution_mode="project_write",
                approval_state="refused",
                reason_code="stale_source",
                source_basis=source_basis,
                details={"required_tier": required_tier, "expected_sha256": expected_sha256, "current_sha256": current_sha256},
            )
            raise PlatformError(
                "Project file changed after the preview was generated. Reload the current file before applying a write.",
                status_code=409,
                payload={"reason_code": "stale_source", "current_sha256": current_sha256},
            )

        backup_dir = self.state_dir / "backups" / datetime.now().strftime("%Y%m%d-%H%M%S")
        backup_dir.mkdir(parents=True, exist_ok=True)
        if target.exists():
            backup_path = backup_dir / normalized.replace("/", "__")
            backup_path.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(target, backup_path)
        target.write_text(content, encoding="utf-8")
        written_sha256 = sha256_bytes(content.encode("utf-8"))
        self._record_write_event(
            target_path=normalized,
            execution_mode="project_write",
            approval_state="approved",
            reason_code="write_applied",
            source_basis=source_basis,
            details={
                "required_tier": required_tier,
                "reason": reason,
                "backup_dir": str(backup_dir),
                "previous_sha256": current_sha256,
                "written_sha256": written_sha256,
            },
        )
        return {
            "status": "written",
            "target_path": normalized,
            "required_tier": required_tier,
            "backup_dir": str(backup_dir),
            "written_sha256": written_sha256,
        }

    def _required_tier_for_path(self, relative_path: str) -> str:
        filename = PureWindowsPath(relative_path).name
        if filename in self.PROTECTED_PROJECT_FILES or relative_path.startswith("continuation-platform/"):
            return "tier_4"
        return "tier_3"

    def _tier_meets_requirement(self, active_tier: str | None, required_tier: str) -> bool:
        order = {"tier_3": 3, "tier_4": 4}
        if active_tier not in order or required_tier not in order:
            return False
        return order[active_tier] >= order[required_tier]

    def resume_from_last_good_state(self) -> dict[str, Any]:
        resume_state = self._load_json(self.resume_state_path, {})
        if not resume_state:
            resume_state = self._build_resume_state(self._load_json(self.source_map_path, {}))
            self._write_json(self.resume_state_path, resume_state)
        resume_state = self._apply_resume_override(resume_state)

        if resume_state.get("selection_required"):
            response = {
                "status": "requires_anchor_selection",
                "message": "Multiple recent last-good-state candidates need operator review.",
                "resume_state": resume_state,
            }
            self.last_agent_result = response
            self._write_json(self.last_agent_result_path, response)
            return response

        anchor = resume_state.get("effective_anchor") or resume_state.get("anchor")
        if not anchor:
            raise PlatformError("No valid resume anchor was found.", status_code=404)

        bundle = self._build_context_bundle(anchor)
        agent_result = self._run_resume_agent(anchor, bundle)
        self.last_agent_result = agent_result
        self._write_json(self.last_agent_result_path, agent_result)
        self._write_json(self.tasks_board_path, self._build_tasks_board())
        self._journal_event(
            event_type="agent_resume",
            status="completed",
            summary=f"Resume evaluated from {anchor['candidate_type']}",
            payload={
                "anchor_type": anchor["candidate_type"],
                "anchor_path": anchor.get("source_path"),
                "recommended_mode": agent_result.get("recommended_mode"),
                "recommended_next_step": agent_result.get("recommended_next_step"),
            },
            provenance=agent_result.get("provenance", []),
        )
        return agent_result

    def _build_context_bundle(self, anchor: dict[str, Any]) -> dict[str, Any]:
        registry = self._load_json(self.registry_path, {"documents": []})
        source_map = self._load_json(self.source_map_path, {})
        latest_unity_handoff = self._latest_matching_relative_path("docs/unity/session-handoffs/*.md")
        project_gap_summary = self._latest_matching_relative_path("reports/*project_completion_handoff*.md")
        key_docs = []
        key_doc_paths = [
            "NEXT_SESSION_HANDOFF.md",
            "CURRENT_PROJECT_STATE.md",
            "continuity/PROJECT_STATE.json",
            "18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md",
            "01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md",
            "governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md",
            "tasks/todo.md",
        ]
        if latest_unity_handoff:
            key_doc_paths.append(latest_unity_handoff)
        if project_gap_summary:
            key_doc_paths.append(project_gap_summary)

        for relpath in key_doc_paths:
            excerpt = self._document_excerpt(relpath, max_chars=900)
            if excerpt:
                key_docs.append({"path": relpath, "excerpt": excerpt})

        retrieval_query = " ".join(
            [
                "current authoritative project state",
                "latest unity shipping lane state",
                "owner direction unity canonical",
                "best next Unity action",
                anchor.get("source_path") or "",
                anchor.get("label") or "",
            ]
        ).strip()
        retrieval_results = self.search_chunks(retrieval_query, limit=int(self.scan_settings["max_agent_context_chunks"]))

        return {
            "anchor": anchor,
            "known_questions": self._current_known_questions(),
            "recent_changes": source_map.get("recent_changes", [])[:8],
            "frontier_artifacts": source_map.get("frontier_artifacts", [])[:6],
            "registry_documents": registry.get("documents", [])[:12],
            "key_docs": key_docs,
            "retrieval_results": retrieval_results,
        }

    def _document_excerpt(self, relative_path: str, max_chars: int = 900) -> str | None:
        absolute_path = (self.bloodlines_root / relative_path).resolve()
        if not absolute_path.exists():
            return None
        text = absolute_path.read_text(encoding="utf-8", errors="ignore").strip()
        if not text:
            return None
        return text[:max_chars]

    def search_chunks(self, query: str, limit: int = 8) -> list[dict[str, Any]]:
        if not query.strip():
            return []

        results: list[dict[str, Any]] = []
        if self.fts_enabled:
            sanitized_query = " ".join(term for term in re.split(r"\W+", query.lower()) if term)
            try:
                with self._connect() as connection:
                    rows = connection.execute(
                        """
                        SELECT
                            chunks.doc_path,
                            chunks.chunk_index,
                            chunks.heading,
                            chunks.text,
                            documents.authority_score,
                            bm25(chunks_fts) AS rank
                        FROM chunks_fts
                        JOIN chunks
                          ON chunks_fts.doc_path = chunks.doc_path
                         AND chunks_fts.text = chunks.text
                        JOIN documents
                          ON documents.path = chunks.doc_path
                        WHERE chunks_fts MATCH ?
                        ORDER BY documents.authority_score DESC, rank ASC
                        LIMIT ?
                        """,
                        (sanitized_query, limit),
                    ).fetchall()
                    for row in rows:
                        results.append(
                            {
                                "path": row["doc_path"],
                                "chunk_index": row["chunk_index"],
                                "heading": row["heading"],
                                "excerpt": row["text"][:420],
                                "authority_score": row["authority_score"],
                                "rank": row["rank"],
                            }
                        )
            except sqlite3.OperationalError:
                results = []
        if not results:
            query_terms = [term for term in re.split(r"\W+", query.lower()) if term]
            with self._connect() as connection:
                rows = connection.execute(
                    """
                    SELECT chunks.doc_path, chunks.chunk_index, chunks.heading, chunks.text, documents.authority_score
                    FROM chunks
                    JOIN documents ON documents.path = chunks.doc_path
                    ORDER BY documents.authority_score DESC
                    """
                ).fetchall()
            scored: list[tuple[int, dict[str, Any]]] = []
            for row in rows:
                lowered = row["text"].lower()
                score = sum(lowered.count(term) for term in query_terms)
                if score > 0:
                    scored.append(
                        (
                            score,
                            {
                                "path": row["doc_path"],
                                "chunk_index": row["chunk_index"],
                                "heading": row["heading"],
                                "excerpt": row["text"][:420],
                                "authority_score": row["authority_score"],
                                "rank": -score,
                            },
                        )
                    )
            scored.sort(key=lambda item: (item[0], item[1]["authority_score"]), reverse=True)
            results = [item[1] for item in scored[:limit]]

        hit_rate = round(len(results) / max(limit, 1), 2)
        provenance_distribution = dict(Counter(item["path"] for item in results))
        self.telemetry["retrieval"]["last_hit_rate"] = hit_rate
        self.telemetry["retrieval"]["top_result_provenance_distribution"] = provenance_distribution
        recent_queries = self.telemetry["retrieval"].setdefault("recent_queries", [])
        recent_queries.append({"query": query, "hit_count": len(results), "at": utc_now()})
        self.telemetry["retrieval"]["recent_queries"] = recent_queries[-12:]
        self._persist_telemetry()
        return results

    def _run_resume_agent(self, anchor: dict[str, Any], bundle: dict[str, Any]) -> dict[str, Any]:
        task_type = "generation"
        routing = self.routing_policy["task_routing"][task_type]
        model = routing["primary_model"]
        started = time.time()

        prompt = self._build_resume_prompt(anchor, bundle)
        model_response = None
        degraded_reason = None
        try:
            response = self._ollama_request(
                "/api/generate",
                method="POST",
                payload={
                    "model": model,
                    "stream": False,
                    "format": "json",
                    "prompt": prompt,
                    "options": {"temperature": 0.2, "num_predict": 320},
                },
                timeout_seconds=150,
            )
            model_response = self._extract_json_object(response.get("response", ""))
            if not isinstance(model_response, dict):
                raise ValueError("Model response was not valid JSON")
            self._update_degraded_modes(active=False, reason_code="ollama_resume_fallback")
            prompt_tokens = response.get("prompt_eval_count", 0)
            output_tokens = response.get("eval_count", 0)
            wall_time = round(time.time() - started, 3)
            self._record_task_metrics(
                task_type=task_type,
                model=model,
                wall_time=wall_time,
                prompt_tokens=prompt_tokens,
                output_tokens=output_tokens,
                degraded_reason=None,
                retrieval_hits=len(bundle["retrieval_results"]),
            )
        except Exception as exc:
            degraded_reason = "ollama_resume_fallback"
            self._update_degraded_modes(active=True, reason_code=degraded_reason)
            model_response = self._heuristic_resume_response(anchor, bundle, str(exc))
            wall_time = round(time.time() - started, 3)
            self._record_task_metrics(
                task_type=task_type,
                model=model,
                wall_time=wall_time,
                prompt_tokens=0,
                output_tokens=0,
                degraded_reason=degraded_reason,
                retrieval_hits=len(bundle["retrieval_results"]),
            )

        provenance = self._unique_paths(
            [anchor.get("source_path")]
            + [item["path"] for item in bundle["key_docs"]]
            + [item["path"] for item in bundle["retrieval_results"]]
        )
        tasks_board = self._build_tasks_board()
        continuity_priority = (tasks_board.get("project_work_priority") or [None])[0]
        model_next_step = model_response.get("recommended_next_step")
        recommended_next_step = continuity_priority or model_next_step
        result = {
            "status": "completed",
            "mode": "resume_last_good_state",
            "anchor": anchor,
            "reasoning_summary": model_response.get("reasoning_summary"),
            "recommended_next_step": recommended_next_step,
            "model_recommended_next_step": model_next_step,
            "continuity_override_applied": bool(continuity_priority and continuity_priority != model_next_step),
            "recommended_mode": model_response.get("recommended_mode", "implement"),
            "confidence": model_response.get("confidence", 0.62),
            "unresolved_items": model_response.get("unresolved_items", []),
            "frontier_handoff_candidate": model_response.get("frontier_handoff_candidate", False),
            "frontier_handoff_reason": model_response.get("frontier_handoff_reason"),
            "provenance": provenance,
            "routing": {
                "task_type": task_type,
                "primary_model": model,
                "fallback_model": routing.get("fallback_model"),
                "degraded_reason": degraded_reason,
            },
            "retrieval_results": bundle["retrieval_results"],
            "recent_changes": bundle["recent_changes"],
            "generated_at": utc_now(),
        }
        return result

    def _build_resume_prompt(self, anchor: dict[str, Any], bundle: dict[str, Any]) -> str:
        doctrine = "\n".join(f"- {item['rule']}" for item in self.doctrine_rules.get("rules", []))
        key_docs = "\n\n".join(
            f"[{doc['path']}]\n{doc['excerpt']}" for doc in bundle["key_docs"]
        )
        retrieval = "\n\n".join(
            f"[{item['path']} :: {item['heading']}]\n{item['excerpt']}" for item in bundle["retrieval_results"]
        )
        recent_changes = "\n".join(
            f"- {item['path']} ({item['mtime']})" for item in bundle["recent_changes"]
        )
        prompt = f"""
You are the local Bloodlines continuation agent.

Follow these doctrine rules exactly:
{doctrine}

Return strict JSON with these keys:
- reasoning_summary
- recommended_next_step
- recommended_mode
- confidence
- unresolved_items
- frontier_handoff_candidate
- frontier_handoff_reason

Do not invent facts outside the supplied sources.
If local capability is insufficient, set frontier_handoff_candidate to true.

Resume anchor:
- type: {anchor.get("candidate_type")}
- label: {anchor.get("label")}
- source_path: {anchor.get("source_path")}
- source_ref: {anchor.get("source_ref")}
- event_time: {anchor.get("event_time")}

Recent changes:
{recent_changes}

Key documents:
{key_docs}

Retrieved support:
{retrieval}
"""
        return prompt.strip()

    def _extract_json_object(self, text: str) -> dict[str, Any]:
        text = text.strip()
        if not text:
            raise ValueError("Empty model response")
        try:
            return json.loads(text)
        except json.JSONDecodeError:
            match = re.search(r"\{.*\}", text, re.DOTALL)
            if not match:
                raise
            return json.loads(match.group(0))

    def _heuristic_resume_response(
        self, anchor: dict[str, Any], bundle: dict[str, Any], failure_reason: str
    ) -> dict[str, Any]:
        next_task = self._extract_next_unchecked_task()
        unresolved = []
        if bundle["frontier_artifacts"]:
            unresolved.append("Review the latest frontier artifact for any state drift before writing.")
        if bundle["recent_changes"]:
            unresolved.append("Confirm the latest changed files are still authoritative after the rescan.")
        return {
            "reasoning_summary": (
                f"Resume from {anchor.get('source_path')} because it is the latest valid continuity anchor. "
                "Use the current project state and next-session handoff as the primary execution spine."
            ),
            "recommended_next_step": next_task
            or "Open NEXT_SESSION_HANDOFF.md, confirm the current continuation batch, then continue the next documented work item.",
            "recommended_mode": "resume",
            "confidence": 0.58,
            "unresolved_items": unresolved,
            "frontier_handoff_candidate": False,
            "frontier_handoff_reason": None,
            "fallback_reason": failure_reason,
        }

    def _extract_next_unchecked_task(self) -> str | None:
        todo_path = self.bloodlines_root / "tasks" / "todo.md"
        if not todo_path.exists():
            return None
        text = todo_path.read_text(encoding="utf-8", errors="ignore")
        match = re.search(r"^- \[ \] (.+)$", text, re.MULTILINE)
        return match.group(1).strip() if match else None

    def _record_task_metrics(
        self,
        task_type: str,
        model: str,
        wall_time: float,
        prompt_tokens: int,
        output_tokens: int,
        degraded_reason: str | None,
        retrieval_hits: int,
    ) -> None:
        routing_counts = self.telemetry["routing"].setdefault("decisions_by_task_type", {})
        routing_counts[task_type] = routing_counts.get(task_type, 0) + 1
        recent_tasks = self.telemetry.setdefault("recent_tasks", [])
        recent_tasks.append(
            {
                "task_type": task_type,
                "model": model,
                "wall_time_seconds": wall_time,
                "prompt_tokens": prompt_tokens,
                "output_tokens": output_tokens,
                "retrieval_hits": retrieval_hits,
                "degraded_reason": degraded_reason,
                "at": utc_now(),
            }
        )
        self.telemetry["recent_tasks"] = recent_tasks[-20:]
        self._persist_telemetry()

    def _persist_telemetry(self) -> None:
        self.telemetry["generated_at"] = utc_now()
        self._write_json(self.telemetry_path, self.telemetry)

    def _update_degraded_modes(self, active: bool, reason_code: str) -> None:
        degraded = self.telemetry.setdefault("degraded_modes", {"count": 0, "reasons": {}, "active": []})
        active_list = degraded.setdefault("active", [])
        reasons = degraded.setdefault("reasons", {})
        if active and reason_code not in active_list:
            active_list.append(reason_code)
            degraded["count"] = degraded.get("count", 0) + 1
            reasons[reason_code] = reasons.get(reason_code, 0) + 1
        if not active and reason_code in active_list:
            active_list.remove(reason_code)
        self._persist_telemetry()

    def _unique_paths(self, values: list[str | None]) -> list[str]:
        output = []
        seen = set()
        for value in values:
            if not value or value in seen:
                continue
            seen.add(value)
            output.append(value)
        return output

    def _journal_event(
        self,
        event_type: str,
        status: str,
        summary: str,
        payload: dict[str, Any],
        doctrine_check: str = "pass",
        provenance: list[str] | None = None,
    ) -> None:
        created_at = utc_now()
        provenance = provenance or []
        record = {
            "created_at": created_at,
            "event_type": event_type,
            "status": status,
            "doctrine_check": doctrine_check,
            "summary": summary,
            "payload": payload,
            "provenance": provenance,
        }
        with self._connect() as connection:
            connection.execute(
                """
                INSERT INTO journal (created_at, event_type, status, doctrine_check, provenance, summary, payload_json)
                VALUES (?, ?, ?, ?, ?, ?, ?)
                """,
                (created_at, event_type, status, doctrine_check, json.dumps(provenance), summary, json.dumps(payload)),
            )
        with self.journal_path.open("a", encoding="utf-8") as handle:
            handle.write(json.dumps(record) + "\n")

    def get_timeline(self, limit: int = 20) -> list[dict[str, Any]]:
        with self._connect() as connection:
            rows = connection.execute(
                """
                SELECT created_at, event_type, status, doctrine_check, provenance, summary, payload_json
                FROM journal
                ORDER BY created_at DESC
                LIMIT ?
                """,
                (limit,),
            ).fetchall()
        timeline = []
        for row in rows:
            timeline.append(
                {
                    "created_at": row["created_at"],
                    "event_type": row["event_type"],
                    "status": row["status"],
                    "doctrine_check": row["doctrine_check"],
                    "provenance": json.loads(row["provenance"]),
                    "summary": row["summary"],
                    "payload": json.loads(row["payload_json"]),
                }
            )
        return timeline

    def export_handoff_pack(self) -> dict[str, Any]:
        builder = self.get_handoff_builder_state()
        anchor = builder.get("anchor") or {}
        agent_result = self.last_agent_result or self._load_json(self.last_agent_result_path, {})
        lines = [
            "# Bloodlines Continuation Platform Handoff",
            "",
            f"Generated: {utc_now()}",
            "",
            "## Resume Anchor",
            json.dumps(anchor, indent=2),
            "",
            "## Recommended Next Step",
            builder.get("open_work", {}).get("recommended_next_step") or "No recommendation generated yet.",
            "",
            "## Reasoning Summary",
            agent_result.get("reasoning_summary", "No agent summary generated yet."),
            "",
            "## Continuity Delta",
        ]
        for line in builder.get("briefing_lines", []):
            lines.append(f"- {line}")
        lines.extend(["", "## High-Signal Changes"])
        for item in builder.get("continuity_delta", {}).get("high_signal_changes", []):
            lines.append(
                f"- {item['change_type']}: {item['path']} ({item.get('classification', 'unknown')}, authority={item.get('authority_score', 0)})"
            )
        lines.extend(["", "## Frontier Session Updates"])
        for item in builder.get("continuity_delta", {}).get("frontier_session_updates", []):
            lines.append(
                f"- {item['change_type']}: {item['path']} ({item.get('provider', 'unknown')} / {item.get('artifact_kind', 'artifact')})"
            )
        lines.extend(["", "## Open Work"])
        for item in builder.get("open_work", {}).get("project_work_priority", []):
            lines.append(f"- {item}")
        for task in builder.get("open_work", {}).get("open_tasks", [])[:8]:
            lines.append(f"- {task['task']} [{task['heading']}]")
        lines.extend(["", "## Canonical Sources"])
        for doc in builder.get("canonical_sources", [])[:10]:
            lines.append(f"- {doc['path']} ({doc['classification']}, authority={doc['authority_score']})")
        lines.extend(["", "## Doctrine Rules"])
        for rule in builder.get("doctrine_rules", []):
            lines.append(f"- {rule}")
        lines.extend(["", "## Frontier Escalations"])
        if builder.get("frontier_escalations"):
            for item in builder["frontier_escalations"]:
                lines.append(f"- {item['reason']} ({item.get('mode', 'unspecified')})")
        else:
            lines.append("- None currently queued.")
        lines.extend(["", "## Frontier Re-entry Briefing Prompt", builder.get("suggested_prompt") or ""])
        self.handoff_path.write_text("\n".join(lines) + "\n", encoding="utf-8")
        self._journal_event(
            event_type="handoff_export",
            status="completed",
            summary="Generated handoff pack preview",
            payload={
                "path": str(self.handoff_path),
                "changed_documents": builder.get("continuity_delta", {}).get("summary", {}).get("changed_documents", 0),
                "open_task_count": len(builder.get("open_work", {}).get("open_tasks", [])),
            },
            provenance=[str(self.handoff_path.name)],
        )
        return {"status": "completed", "path": str(self.handoff_path), "preview": "\n".join(lines[:18])}

    def get_environment_status(self) -> dict[str, Any]:
        python_call = subprocess.run(
            ["python", "--version"], capture_output=True, text=True, timeout=5, check=False
        )
        status = {
            "windows_runtime": True,
            "python_version": (python_call.stdout.strip() or python_call.stderr.strip()),
            "wsl": {"available": False, "running_distributions": []},
        }
        try:
            result = subprocess.run(
                ["wsl", "-l", "-v"],
                capture_output=True,
                text=True,
                timeout=8,
                check=False,
            )
            output = result.stdout.strip() or result.stderr.strip()
            if output:
                lines = [line.strip() for line in output.splitlines() if line.strip() and "NAME" not in line.upper()]
                running = []
                for line in lines:
                    if "Running" in line or "Stopped" in line:
                        running.append(line)
                status["wsl"] = {
                    "available": result.returncode == 0,
                    "running_distributions": running,
                }
        except Exception as exc:
            status["wsl"] = {"available": False, "error": str(exc), "running_distributions": []}
        return status
