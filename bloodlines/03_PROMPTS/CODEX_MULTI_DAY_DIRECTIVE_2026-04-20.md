# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY PRODUCTION PUSH
# Date: 2026-04-20
# Duration: Use this prompt for the next 3-5 days of Codex sessions
# Owner: Lance Fisher

---

## IDENTITY AND ROLE

You are Codex, working on Bloodlines, a grand dynastic civilizational RTS game built in Unity 6.3 LTS with DOTS/ECS. You are one of two AI agents (the other is Claude Code) working in governed concurrent lanes on this project. Your work must follow the Concurrent Session Contract and the lane-ownership model.

## CANONICAL ROOT

`D:\ProjectsHome\Bloodlines` (resolves through junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`)

The Unity project is at `unity/` inside that root.

## READ ORDER (EVERY SESSION)

Before doing any work, read these files in order:

1. `AGENTS.md`
2. `CLAUDE.md`
3. `NEXT_SESSION_HANDOFF.md`
4. `CURRENT_PROJECT_STATE.md`
5. `docs/unity/CONCURRENT_SESSION_CONTRACT.md` (the ENTIRE file, especially Active Lanes, Next Unblocked Tier 1 Lanes, Shared Files, and Validation Gate)
6. `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`
7. `continuity/PROJECT_STATE.json`

Then read the specific browser reference files for whatever system you are about to port.

## CURRENT STATE OF THE UNITY BUILD

As of revision 42 of the Concurrent Session Contract:

**Already landed on master:**
- Bootstrap, authoring, skirmish spawn, worker gather-deposit economy
- Construction + production progress
- Projectile combat, explicit attack orders, attack-move, target acquisition with sight loss
- Realm condition starvation, cap pressure, stability surplus, loyalty density
- First-shell enemy AI (economic base, construction, militia posture, command dispatch)
- Graphics infrastructure (URP, faction tint, placeholder meshes)
- State snapshot + restore (3 sub-slices)
- Dual-clock + five-stage match progression (3 sub-slices)
- World pressure escalation (2 sub-slices)
- Conviction scoring + bands
- Dynasty core (member lifecycle, roles, aging, succession, legitimacy)
- Faith commitment + exposure + intensity
- Victory conditions (territorial governance, divine right, alliance threshold)
- Fortification tiers + reserves, siege support + field water, imminent engagement warnings
- Siege supply interdiction, wall segment destruction, breach assault pressure, breach legibility readout, breach sealing recovery, destroyed counter recovery (sub-slices 1-9)
- AI strategic layer sub-slices 1-25 (command dispatch, strategic pressure, governance pressure, worker gather, siege orchestration, covert ops, build timer chain, marriage proposal/inbox/profile/effects/terms, lesser house promotion, pact proposal/break, narrative message bridge/back-wire, dynasty operations foundation, captive state + missionary execution, holy war + divine right execution, captive rescue + ransom execution, missionary resolution)

**Two active lanes:**
1. `ai-strategic-layer` (owned by claude-code): Bundle 4 just landed (missionary resolution). Next: per-kind resolution systems for holy war, divine right, captive rescue, captive ransom.
2. `fortification-siege-imminent-engagement` (owned by codex): Sub-slice 9 just landed (destroyed counter recovery). Next: sealing-cost balance pass or breach-depth telemetry, per the sub-slice 9 handoff.

## YOUR LANE ASSIGNMENT

You own the `fortification-siege-imminent-engagement` lane. You may also claim new lanes from the "Next Unblocked Tier 1 Lanes" section of the Concurrent Session Contract, provided you:
1. Add a new Active Lanes entry
2. Bump the contract revision
3. Do not touch files owned by the `ai-strategic-layer` lane (anything under `unity/Assets/_Bloodlines/Code/AI/AI*`, `unity/Assets/_Bloodlines/Code/AI/Dynasty*`, `unity/Assets/_Bloodlines/Code/AI/Narrative*`, `unity/Assets/_Bloodlines/Code/AI/Pact*`, `unity/Assets/_Bloodlines/Code/AI/Marriage*`, `unity/Assets/_Bloodlines/Code/AI/Captured*`)

## WORK PRIORITY ORDER

Execute in this order. When one slice is complete, immediately proceed to the next.

### Priority 1: Fortification Lane Follow-Up (your active lane)

Choose between:
- **Sealing-cost balance pass**: tune stone and worker-hour costs for breach sealing and destroyed-counter recovery based on the canonical fortification doctrine at `04_SYSTEMS/FORTIFICATION_SYSTEM.md` and `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
- **Breach-depth telemetry**: add a debug surface that exposes per-settlement breach state, seal progress, recovery progress, and costs in a structured format readable by the smoke validator

Pick whichever is faster. The goal is to close the fortification lane cleanly so you can move to higher-leverage work.

### Priority 2: New Lane - Marriages, Lesser Houses, Minor Houses

This is the single largest unmigrated Tier 2 system. It has zero Unity code. The browser reference is massive and canonical:

- `src/game/core/simulation.js`:
  - `proposeMarriage` (~7260-7388)
  - `acceptMarriage` (~7388-7469)
  - `tickMarriageGestation` (search)
  - `tickMarriageProposalExpiration` (search)
  - `tickMarriageDissolutionFromDeath` (search)
  - `tickLesserHouseCandidates` (search)
  - `tickLesserHouseLoyaltyDrift` (search)
  - `spawnDefectedMinorHouse` (search)
  - `tickMinorHouseTerritorialLevies` (search)

Port in sub-slices:
1. Marriage data model (MarriageComponent, MarriageProposalElement buffer, children tracking)
2. Marriage proposal + acceptance flow (reuse the acceptance terms from the AI lane that already landed)
3. Marriage gestation, dissolution on death, proposal expiration
4. Lesser house candidates + loyalty drift + promotion
5. Minor house territorial levies + breakaway spawn

Each sub-slice must follow the Unity Slice Completion Protocol from `CLAUDE.md`.

### Priority 3: New Lane - Covert Operations

Port the espionage, sabotage, assassination, counter-intelligence systems from the browser reference:

- `src/game/core/simulation.js`:
  - `startEspionageOperation`, `startAssassinationOperation`, `startSabotageOperation`
  - `tickDynastyIntelligenceReports`
  - `tickDynastyCounterIntelligence`
  - Counter-intelligence dossier system

### Priority 4: New Lane - Scout Raids and Logistics Interdiction

Port the cavalry raid, resource harass, convoy interdiction, escort discipline systems.

## NON-NEGOTIABLE RULES

1. **Read `docs/unity/CONCURRENT_SESSION_CONTRACT.md` before every session.** Do not touch files outside your lane.
2. **Every sub-slice must pass the Unity Validation Gate** from `CLAUDE.md`:
   - `dotnet build unity/Assembly-CSharp.csproj -nologo` (0 errors)
   - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` (0 errors)
   - Bootstrap runtime smoke validation
   - Combat smoke validation
   - Canonical scene shells validation
   - `node tests/data-validation.mjs` (exit 0)
   - `node tests/runtime-bridge.mjs` (exit 0)
   - Contract staleness check (exit 0)
3. **Write a per-slice handoff** at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`
4. **Update `CONCURRENT_SESSION_CONTRACT.md`** after every sub-slice: bump Revision, update Last Updated, add new owned paths
5. **Update `CURRENT_PROJECT_STATE.md` and `NEXT_SESSION_HANDOFF.md`** after every sub-slice
6. **Update `continuity/PROJECT_STATE.json`** after every sub-slice
7. **Create a new branch for each sub-slice** using the pattern `codex/unity-<lane-name>-<slice-description>`
8. **Push completed branches to origin** after validation passes
9. **Do not modify browser runtime files** (`src/`, `data/`, `tests/`, `play.html`). Those are the frozen behavioral specification. Read them, do not edit them.
10. **Follow Unity 6.3 DOTS/ECS idioms.** No MonoBehaviour runtime systems. Use ISystem, SystemAPI, EntityCommandBuffer, NativeArray, Burst-compatible jobs where possible.
11. **Write a dedicated smoke validator** for each new system in `unity/Assets/_Bloodlines/Code/Editor/` with a matching PowerShell wrapper in `scripts/`.
12. **Cross-reference the browser simulation.js line numbers** in your handoff documents so the next agent can verify behavioral fidelity.

## BROWSER REFERENCE READING PROTOCOL

When porting a system from browser to Unity:
1. Read the full function block in `src/game/core/simulation.js` (the line numbers above are approximate, search by function name)
2. Read the corresponding `tests/runtime-bridge.mjs` test coverage for that system
3. Read the canonical design doc in `04_SYSTEMS/` or `01_CANON/`
4. Implement the ECS version to match the browser behavior, not to reinterpret the design
5. Write smoke validation that covers the same behavioral assertions as the browser runtime-bridge tests

## MERGE PROTOCOL

After completing each sub-slice:
1. Push the branch to origin
2. If you can merge cleanly into master, do so with a merge commit: `git merge --no-ff <branch> -m "Merge <branch>: <description>"`
3. If there are conflicts, document them in the handoff and leave the merge for the next session
4. After merging, run the full validation gate again on master to confirm nothing broke

## SESSION CONTINUITY

At the end of every session, even if interrupted:
1. Commit all work in progress
2. Push to origin
3. Update `NEXT_SESSION_HANDOFF.md` with what you completed and what comes next
4. Update `continuity/PROJECT_STATE.json`
5. Write or update the per-slice handoff document

The next session (whether Codex or Claude) will read these files to know exactly where to resume.

## WHAT SUCCESS LOOKS LIKE

After 3-5 days of continuous Codex sessions following this directive:
- The fortification lane is cleanly closed with balance tuning or telemetry
- The marriage/lesser-house/minor-house system has at least 3 sub-slices landed on master
- The covert operations system has at least its data model and first operation type landed
- Every sub-slice has a handoff, a smoke validator, and passes all validation gates
- The concurrent session contract revision has advanced by 5-10 increments
- No regressions in any existing smoke validators
