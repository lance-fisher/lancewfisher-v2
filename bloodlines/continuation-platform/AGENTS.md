# AGENTS.md: Bloodlines Continuation Platform

This module lives inside the canonical Bloodlines root and inherits the root
Bloodlines governance surfaces.

Read order for work in this module:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
6. `D:\ProjectsHome\Bloodlines\continuation-platform\README.md`
7. `D:\ProjectsHome\Bloodlines\continuation-platform\docs\system_design.md`

Local rules for this module:

- The platform is Bloodlines-only. Never widen scan scope outside
  `D:\ProjectsHome\Bloodlines`.
- Preserve the project root. Do not move Bloodlines assets into a second root.
- Treat `continuation-platform\state\` as generated data.
- Treat `continuation-platform\config\` and `docs\` as operator-facing assets,
  keep changes additive and documented.
- Default runtime is Windows-native Python. WSL is optional, not required.

How to run:

```powershell
cd D:\ProjectsHome\Bloodlines\continuation-platform
python server.py --open
```

Key files:

- `server.py`: local HTTP server and API
- `lib\core.py`: scanning, retrieval, routing, continuity, and write posture
- `static\`: browser UI
- `config\`: scan subset, routing policy, doctrine, tier hashes
- `state\`: generated sqlite database, registries, journal, telemetry, handoff
