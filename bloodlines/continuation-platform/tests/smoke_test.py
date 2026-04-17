import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
sys.path.insert(0, str(ROOT))

from lib.core import BloodlinesContinuationCore, PlatformError


def main() -> None:
    core = BloodlinesContinuationCore(ROOT)
    bootstrap = core.bootstrap(force=True)
    resume = core.resume_from_last_good_state()
    if resume.get("status") == "requires_anchor_selection":
        candidates = [
            item["candidate_type"]
            for item in resume.get("resume_state", {}).get("candidates", [])
            if item.get("exists")
        ]
        preferred = next(
            (item for item in ("local_agent_action", "manual_edit", "frontier_artifact") if item in candidates),
            None,
        )
        if not preferred:
            raise RuntimeError("Expected at least one selectable resume candidate")
        core.set_resume_anchor(preferred)
        resume = core.resume_from_last_good_state()
    discovered = core._load_json(core.discovered_registry_path)
    tasks_board = core._load_json(core.tasks_board_path)
    change_report = core.get_change_report()
    telemetry = core.get_telemetry_drilldown()
    handoff_builder = core.get_handoff_builder_state()
    execution_packet = core.get_execution_packet()
    agent_console = core.get_agent_console_state()

    print("BOOTSTRAP_OK")
    print(json.dumps({"scan": bootstrap["status"], "anchor": bootstrap["resume_state"].get("anchor")}, indent=2))
    print("RESUME_OK")
    print(json.dumps({"status": resume.get("status"), "next_step": resume.get("recommended_next_step")}, indent=2))

    if discovered.get("total_documents", 0) <= 0:
        raise RuntimeError("Expected discovered registry documents")
    print("DISCOVERED_REGISTRY_OK")

    for question in core.source_subset.get("known_questions", []):
        hits = core.search_chunks(question, limit=3)
        if not hits:
            raise RuntimeError(f"Expected retrieval hits for question: {question}")
    print("RETRIEVAL_OK")

    if not tasks_board.get("open_tasks"):
        raise RuntimeError("Expected open tasks in tasks board")
    print("TASKS_BOARD_OK")

    if change_report.get("summary", {}).get("changed_documents", 0) < 0:
        raise RuntimeError("Expected change report summary")
    print("CHANGE_REPORT_OK")

    if not telemetry.get("recent_scans"):
        raise RuntimeError("Expected telemetry scan history")
    print("TELEMETRY_OK")

    if not handoff_builder.get("suggested_prompt"):
        raise RuntimeError("Expected handoff builder prompt")
    print("HANDOFF_BUILDER_OK")

    if execution_packet.get("execution_lane") != "unity_shipping":
        raise RuntimeError("Expected unity shipping execution packet")
    if not execution_packet.get("recommended_next_step"):
        raise RuntimeError("Expected execution packet next step")
    print("EXECUTION_PACKET_OK")

    if not agent_console.get("available_commands"):
        raise RuntimeError("Expected command deck commands")
    if not agent_console.get("session", {}).get("messages"):
        raise RuntimeError("Expected initial console welcome message")
    print("AGENT_CONSOLE_STATE_OK")

    console_help = core.submit_agent_console_message("/help")
    if console_help["session"]["messages"][-1]["kind"] != "command":
        raise RuntimeError("Expected /help to append a command response")
    print("AGENT_CONSOLE_HELP_OK")

    console_search = core.submit_agent_console_message("/search Unity shipping lane")
    if not console_search["session"]["messages"][-1].get("citations"):
        raise RuntimeError("Expected /search to cite local files")
    print("AGENT_CONSOLE_SEARCH_OK")

    console_read = core.submit_agent_console_message("/read CURRENT_PROJECT_STATE.md")
    if "CURRENT_PROJECT_STATE.md" not in console_read["session"]["messages"][-1]["content"]:
        raise RuntimeError("Expected /read to load CURRENT_PROJECT_STATE.md")
    print("AGENT_CONSOLE_READ_OK")

    loaded = core.read_project_file("CURRENT_PROJECT_STATE.md")
    if not loaded.get("exists") or "CURRENT_PROJECT_STATE" not in loaded.get("relative_path", ""):
        raise RuntimeError("Expected project file load to return the current state file")
    print("PROJECT_FILE_LOAD_OK")

    preview = core.preview_project_write(
        relative_path="test-results/continuation-platform-write-smoke.txt",
        content="continuation-platform smoke write\n",
        reason="smoke preview",
        source_basis="tests/smoke_test.py",
    )
    if not preview.get("changed"):
        raise RuntimeError("Expected write preview to detect a change")
    if preview.get("required_tier") != "tier_3":
        raise RuntimeError("Expected test-results write to require tier_3")
    print("WRITE_PREVIEW_OK")

    try:
        core.project_write_probe(
            relative_path="CURRENT_PROJECT_STATE.md",
            content="write gate validation",
            reason="smoke test",
            source_basis="tests/smoke_test.py",
        )
        raise RuntimeError("Expected write gate refusal")
    except PlatformError as exc:
        if exc.payload.get("reason_code") != "tier_insufficient":
            raise
        print("WRITE_GATE_OK")

    core.session_state["write_posture"] = "approved_write"
    core.session_state["active_tier"] = "tier_3"
    core.session_state["project_writes_allowed"] = True
    core.session_state["unlock_reason"] = "test_override"

    applied = core.project_write_probe(
        relative_path="test-results/continuation-platform-write-smoke.txt",
        content="continuation-platform smoke write\n",
        reason="smoke apply",
        source_basis="tests/smoke_test.py",
        expected_sha256=preview.get("current_sha256"),
    )
    if applied.get("status") != "written":
        raise RuntimeError("Expected governed project write to succeed under unlocked test posture")

    reloaded = core.read_project_file("test-results/continuation-platform-write-smoke.txt")
    if reloaded.get("content", "").replace("\r\n", "\n") != "continuation-platform smoke write\n":
        raise RuntimeError("Expected governed project write to update the target file")
    print("WRITE_APPLY_OK")

    handoff = core.export_handoff_pack()
    if handoff.get("status") != "completed":
        raise RuntimeError("Expected handoff export completion")
    print("HANDOFF_PREVIEW_OK")


if __name__ == "__main__":
    main()
