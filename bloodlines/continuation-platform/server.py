import argparse
import json
import mimetypes
import webbrowser
from http import HTTPStatus
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlparse

from lib.core import BloodlinesContinuationCore, PlatformError


PLATFORM_ROOT = Path(__file__).resolve().parent
CORE = BloodlinesContinuationCore(PLATFORM_ROOT)


class ContinuationRequestHandler(BaseHTTPRequestHandler):
    server_version = "BloodlinesContinuationPlatform/0.2"

    def do_GET(self) -> None:
        parsed = urlparse(self.path)
        if parsed.path in {"/", "/index.html"}:
            self._serve_static("index.html")
            return
        if parsed.path == "/app.js":
            self._serve_static("app.js")
            return
        if parsed.path == "/styles.css":
            self._serve_static("styles.css")
            return
        if parsed.path == "/api/bootstrap":
            self._send_json(CORE.bootstrap(force=False))
            return
        if parsed.path == "/api/write-posture":
            self._send_json(CORE.get_write_posture())
            return
        if parsed.path == "/api/telemetry":
            self._send_json(CORE.telemetry)
            return
        if parsed.path == "/api/timeline":
            self._send_json({"events": CORE.get_timeline(limit=24)})
            return
        if parsed.path == "/api/discovered-registry":
            self._send_json(CORE._load_json(CORE.discovered_registry_path, {"documents": [], "conflict_clusters": []}))
            return
        if parsed.path == "/api/change-report":
            self._send_json(CORE.get_change_report())
            return
        if parsed.path == "/api/tasks-board":
            self._send_json(CORE._load_json(CORE.tasks_board_path, {"open_tasks": [], "handoff_priority": []}))
            return
        if parsed.path == "/api/execution-packet":
            self._send_json(CORE.get_execution_packet())
            return
        if parsed.path == "/api/agent-console":
            self._send_json(CORE.get_agent_console_state())
            return
        if parsed.path == "/api/telemetry-drilldown":
            self._send_json(CORE.get_telemetry_drilldown())
            return
        if parsed.path == "/api/handoff-preview":
            self._send_json(CORE.get_handoff_preview())
            return
        if parsed.path == "/api/handoff-builder":
            self._send_json(CORE.get_handoff_builder_state())
            return
        self.send_error(HTTPStatus.NOT_FOUND, "Not found")

    def do_POST(self) -> None:
        parsed = urlparse(self.path)
        payload = self._read_json_body()
        try:
            if parsed.path == "/api/rescan":
                self._send_json(CORE.bootstrap(force=True))
                return
            if parsed.path == "/api/agent/resume":
                self._send_json(CORE.resume_from_last_good_state())
                return
            if parsed.path == "/api/agent-console/message":
                self._send_json(CORE.submit_agent_console_message(payload.get("message", "")))
                return
            if parsed.path == "/api/agent-console/reset":
                self._send_json(CORE.reset_agent_console())
                return
            if parsed.path == "/api/agent-console/apply-draft":
                result = CORE.apply_write_draft(payload.get("draft_id", ""))
                self._send_json({"result": result, "agent_console": CORE.get_agent_console_state()})
                return
            if parsed.path == "/api/agent-console/dismiss-draft":
                result = CORE.dismiss_write_draft(payload.get("draft_id", ""))
                self._send_json({"result": result, "agent_console": CORE.get_agent_console_state()})
                return
            if parsed.path == "/api/select-anchor":
                self._send_json(CORE.set_resume_anchor(payload.get("candidate_type")))
                return
            if parsed.path == "/api/unlock":
                phrase = payload.get("phrase", "")
                self._send_json(CORE.unlock_write_posture(phrase))
                return
            if parsed.path == "/api/export-handoff":
                self._send_json(CORE.export_handoff_pack())
                return
            if parsed.path == "/api/project-file":
                self._send_json(CORE.read_project_file(payload.get("relative_path", "")))
                return
            if parsed.path == "/api/project-write/preview":
                self._send_json(
                    CORE.preview_project_write(
                        relative_path=payload.get("relative_path", ""),
                        content=payload.get("content", ""),
                        reason=payload.get("reason", ""),
                        source_basis=payload.get("source_basis", "api_request"),
                    )
                )
                return
            if parsed.path == "/api/project-write":
                result = CORE.project_write_probe(
                    relative_path=payload.get("relative_path", ""),
                    content=payload.get("content", ""),
                    reason=payload.get("reason", ""),
                    source_basis=payload.get("source_basis", "api_request"),
                    expected_sha256=payload.get("expected_sha256"),
                )
                self._send_json(result)
                return
            self.send_error(HTTPStatus.NOT_FOUND, "Not found")
        except PlatformError as exc:
            self._send_json({"error": str(exc), **exc.payload}, status=exc.status_code)
        except Exception as exc:
            self._send_json({"error": str(exc)}, status=500)

    def _serve_static(self, filename: str) -> None:
        static_path = PLATFORM_ROOT / "static" / filename
        if not static_path.exists():
            self.send_error(HTTPStatus.NOT_FOUND, "Not found")
            return
        content_type = mimetypes.guess_type(str(static_path))[0] or "text/plain"
        payload = static_path.read_bytes()
        self.send_response(HTTPStatus.OK)
        self.send_header("Content-Type", content_type)
        self.send_header("Content-Length", str(len(payload)))
        self.end_headers()
        self.wfile.write(payload)

    def _read_json_body(self) -> dict:
        length = int(self.headers.get("Content-Length", "0"))
        if length <= 0:
            return {}
        body = self.rfile.read(length)
        if not body:
            return {}
        return json.loads(body.decode("utf-8"))

    def _send_json(self, payload: dict, status: int = 200) -> None:
        encoded = json.dumps(payload).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(encoded)))
        self.end_headers()
        self.wfile.write(encoded)

    def log_message(self, format: str, *args) -> None:
        return


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Bloodlines Continuation Platform")
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--port", type=int, default=8067)
    parser.add_argument("--open", action="store_true", help="Open the UI in the default browser.")
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    CORE.bootstrap(force=False)
    server = ThreadingHTTPServer((args.host, args.port), ContinuationRequestHandler)
    url = f"http://{args.host}:{args.port}"
    print(f"Bloodlines Continuation Platform running at {url}")
    if args.open:
        webbrowser.open(url)
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        server.server_close()


if __name__ == "__main__":
    main()
