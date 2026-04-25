# CODEX CONTINUATION PROMPT
# Reuse this prompt to keep pushing Codex forward on Bloodlines Unity work
# Paste this at the start of each new Codex session

---

You are continuing work on Bloodlines, a grand dynastic civilizational RTS game in Unity 6.3 LTS with DOTS/ECS.

## MANDATORY READ ORDER

Before doing anything else, read these files in this exact order:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
3. `D:\ProjectsHome\Bloodlines\docs\unity\CONCURRENT_SESSION_CONTRACT.md` (full file)
4. `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-20.md`

The handoff tells you exactly what was completed last session and what to do next. The concurrent session contract tells you what lanes are active, what files you own, and what files you must not touch. The directive gives you the full priority stack.

## RULES

- You own the `fortification-siege-imminent-engagement` lane and may claim new lanes per the contract
- Do NOT touch files owned by the `ai-strategic-layer` lane (AI*, Dynasty*, Narrative*, Pact*, Marriage*, Captured* under Code/AI/)
- Every sub-slice must pass the full Unity Validation Gate before committing
- Write a per-slice handoff, update the contract revision, update continuity surfaces
- Use `src/game/core/simulation.js` and `src/game/core/ai.js` as the authoritative behavioral specification (read-only, never edit)
- Follow Unity 6.3 DOTS/ECS idioms: ISystem, SystemAPI, EntityCommandBuffer, NativeArray, Burst where possible
- Create a new branch per sub-slice: `codex/unity-<lane>-<description>`
- Push to origin after validation. Merge to master if clean.

## EXECUTION

1. Read the handoff to find your exact next action
2. Read the browser reference code for the system you are porting
3. Implement the ECS sub-slice
4. Write the smoke validator
5. Run all validation gates
6. Write the handoff and update continuity
7. Push and merge
8. Immediately proceed to the next sub-slice from the priority stack in the directive

Do not stop after one sub-slice. Continue through the priority stack until you run out of session capacity.

## PRIORITY STACK (from the directive)

1. Close the fortification lane (sealing-cost balance or breach-depth telemetry)
2. Port marriages, lesser houses, minor houses (5 sub-slices)
3. Port covert operations (espionage, sabotage, assassination, counter-intel)
4. Port scout raids and logistics interdiction

## VERIFICATION COMMANDS

```powershell
cd D:\ProjectsHome\Bloodlines
dotnet build unity/Assembly-CSharp.csproj -nologo
dotnet build unity/Assembly-CSharp-Editor.csproj -nologo
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1
```

All must pass before any commit or push.

Begin now. Read the handoff, find your next action, and execute.
