# Bloodlines Unity Continuity Prompt (v3)

Version: 3
Last Updated: 2026-04-17
Supersedes: v2 (pre-graphics-fidelity-clarification)
Canonical location: `D:\ProjectsHome\Bloodlines\03_PROMPTS\BLOODLINES_UNITY_CONTINUITY_PROMPT_v3.md`

Paste the body below into a new session. Do not modify it mid-paste. Do not edit it for brevity. The prompt is a specification, not a draft.

This prompt is dual-agent: it applies to both Claude Code and Codex sessions. Passages that differ by agent are explicitly labeled. Everything else is shared discipline.

Material change in v3: the Graphics Fidelity Target is now explicitly clamped to Generals Zero Hour (2003) / Warcraft III (2002) era polish. AAA-fidelity instincts (PBR, HDRP, ray tracing, AAA animation) are out of scope. The product is a deep strategy RTS where success is driven by strategy depth, balance, replayability, and multiple viable playstyles across the canonical victory paths in `data/victory-conditions.json`; graphics serve readability and faction legibility, not visual spectacle. The authoritative owner direction for this clamp lives at `governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md` and amends the prior full-canon direction at `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`.

---

Continuing Bloodlines Unity development. This is a resume, not a fresh start. Do not re-explain the project. Do not re-debate the approach. Do not re-litigate scope. The vision is locked.

## AGENT IDENTIFICATION (first action)

Identify which agent you are.

- **If you are Claude Code:** read `D:\ProjectsHome\Bloodlines\CLAUDE.md` and the ambient `.claude/rules/*.md` as you would normally. Your lane claims use the `claude/unity-*` branch prefix. Your wrapper-lock `-Session` identifier uses `claude-<lane>-<date>`.
- **If you are Codex:** read `D:\ProjectsHome\Bloodlines\AGENTS.md` first for canonical root and preservation rules, then this prompt. Your lane claims use the `codex/unity-*` branch prefix. Your wrapper-lock `-Session` identifier uses `codex-<lane>-<date>`.
- **Both agents** share the same contract, migration plan, validation gate, and Definition of Done. Neither agent may claim a lane currently owned by the other per `docs\unity\CONCURRENT_SESSION_CONTRACT.md`. If two agents are running concurrently, each claims exactly one lane on its own prefix and commits only to that branch.

## VISION RECONFIRMATION, NON-NEGOTIABLE

Bloodlines is the full, canonical, no-compromise realization of the complete design bible, built on Unity 6.3 LTS DOTS/ECS. The feel target is Command and Conquer Generals Zero Hour fused with Warcraft III. "Scaled up massively" refers to scope and depth (more factions, more units, more systems, more mechanical depth), not visual fidelity. Every system, every house, every unit, every mechanic in the canon ships. No MVP. No phased release. No descoping of gameplay. No "later" for gameplay systems. The full gameplay depth is the deliverable. Duration and session count are not constraints.

The product is a strategy RTS. Success is driven by strategic depth, system interaction, and replayability, not by graphical fidelity. Graphics exist to serve gameplay legibility, faction distinction, and RTS-zoom readability. See the Graphics Fidelity Target section for the specific fidelity ceiling.

If any instinct surfaces during this session to propose gameplay reduction, phasing, placeholder-gameplay-shipping, or "smaller gameplay first," suppress it and continue the full-scope gameplay implementation. If any instinct surfaces to push graphics beyond the Zero Hour / Warcraft III fidelity ceiling, also suppress it.

## GRAPHICS FIDELITY TARGET (non-negotiable ceiling)

Fidelity ceiling: Command and Conquer Generals Zero Hour (2003) and Warcraft III (2002). That is the upper bound, not the lower bound. Matching it is the goal; exceeding it is out of scope.

What this means concretely:

- **Rendering pipeline:** URP Forward. No HDRP. No ray tracing. No volumetric fog. No screen-space reflections. No temporal anti-aliasing beyond URP's built-in FXAA/SMAA. No expensive post-processing stacks.
- **Shaders:** diffuse + optional simple specular. Faction tint via per-instance color. Simple rim light for silhouette legibility. Selection highlight uniform. That is the full shader surface. No PBR authoring. No subsurface scattering. No anisotropic highlights. No parallax occlusion.
- **Textures:** 256x256 to 512x512 diffuse maps, hand-painted style. Occasional 1024x1024 for hero units or buildings where canonical. No normal maps unless a specific unit canonically requires one. No roughness/metalness/height/ao maps.
- **Meshes:** low-poly, 1,000 to 8,000 triangles per unit, 2,000 to 15,000 per building. Legibility at RTS zoom matters more than detail. Silhouette distinction is the quality metric.
- **Animation:** simple skeletal animation, ~10-30 bones per unit. No motion capture. No facial animation. No fancy IK. Looping cycles for idle/walk/attack/death at minimum. More cycles if canonical requires it (siege deployment, muster, etc).
- **VFX:** basic particle systems. Smoke, fire, projectile trails, impact sparks, blood hits, weather. No GPU particle simulations. No soft particles. No complex particle interactions.
- **Lighting:** single directional sun + simple ambient. Optional point lights for torches and fire at fortifications. No light probes required. Real-time shadows only for the sun, cascaded at two levels. No shadow mapping for point lights.
- **Terrain:** tile-based or simple heightmap terrain with hand-painted texture splatting. No tessellation. No virtual texturing. No terrain displacement.
- **UI:** clean, readable, information-dense. Inspired by Age of Empires, Warcraft III, Zero Hour UI density. Minimap always visible. Unit portraits, production queues, resource readouts, minimap, command card. No animated UI beyond simple hover/select states.

What this rules out:

- No ProBuilder-authored or AAA-asset-authored models.
- No PBR material libraries.
- No commercial texture packs designed for AAA pipelines.
- No Shader Graph networks deeper than 2 or 3 nodes of additive logic beyond what the faction-tint shader already does.
- No attempts to match Total War Warhammer, Age of Empires IV, or Company of Heroes 3 visual fidelity.
- No cinematic cutscenes beyond simple in-engine fly-through style.

Strategy depth is the product. Graphics are a delivery surface for that product. The graphics lane ships infrastructure sized for this ceiling and stops.

## SESSIONS 1 THROUGH 100 DIRECTIVE

All browser-era session work (Sessions 1 through approximately 108, captured as browser simulation code in `src/game/core/*.js`, canon ingestions under `02_SESSION_INGESTIONS/`, and continuation prompts under `03_PROMPTS/CONTINUATION_PROMPT_*.md`) must be analyzed and either ported to Unity ECS or explicitly ruled out with operator acknowledgement. Nothing gets silently dropped.

Treat the browser runtime as an executable specification, not a legacy codebase. Every function cluster in `src/game/core/simulation.js` and `src/game/core/ai.js` is a behavioral contract the Unity implementation must satisfy.

Classification rules for each browser-era system:

- **Already ported.** Verify the Unity implementation is canonically faithful against the browser reference and the canon section. Do not re-port.
- **Not yet ported, compatible.** Implement per the migration plan tier priority. Cite the browser file and function cluster and the canon section in the slice handoff.
- **Not yet ported, incompatible with ECS.** Design a replacement that achieves the same canonical intent. Document the browser behavior being replaced. Mark the old behavior as superseded in the migration plan changelog. Never silently skip.
- **Pure narrative or tooling with no gameplay effect.** Mark as non-migration in the slice handoff and move on.

The browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md` is the authoritative map of what remains.

## RESUME PROTOCOL

1. Read `D:\ProjectsHome\Bloodlines\docs\plans\2026-04-17-browser-to-unity-migration-plan.md` first. This is the authoritative forward-work map.
2. Read `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md` for the current-state summary and the most recent slice entries.
3. Read `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json` for machine-readable current state, especially `concurrent_session_contract.next_unblocked_tier_1_lanes`.
4. Read the latest per-slice Unity handoff under `D:\ProjectsHome\Bloodlines\docs\unity\session-handoffs\` to understand the most recent completed Unity work.
5. Read `D:\ProjectsHome\Bloodlines\docs\unity\CONCURRENT_SESSION_CONTRACT.md`. This is the single source of truth for lane ownership, file-scope boundaries, shared-file rules, forbidden paths, branch discipline, wrapper-lock protocol, and the canonical validation gate.
6. Run `scripts\Invoke-BloodlinesUnityContractStalenessCheck.ps1`. If it returns non-zero, STOP and surface. Do not proceed against a stale contract.
7. If the next forward work is in the graphics lane, also read `docs\unity\VISUAL_ASSET_PIPELINE_2026-04-15.md` and `docs\unity\BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`, and confirm any visual direction stays within the Graphics Fidelity Target ceiling above.
8. Verify `git branch --show-current`. If you are not on `master` and not on the lane branch you intend to advance, switch explicitly.
9. Choose the next slice. Either pick up an in-flight lane (see Active Lanes in the contract) or claim an unblocked Tier 1 candidate (see Next Unblocked Tier 1 Lanes in the contract). If claiming a new lane, bump the contract Revision in the same commit that creates the lane.
10. Proceed.

## BRANCH DISCIPLINE (critical, read before every commit)

Some session environments (observed in practice in Claude Code sessions that ride the continuation platform, due to a shared worktree with a sibling branch) may silently move `HEAD` to a different branch mid-session. Codex CLI sessions have not been observed to do this, but the defensive posture is cheap and correct either way:

- Before every commit, confirm `git branch --show-current` matches the lane branch you intend to advance.
- If `HEAD` has drifted to an unexpected branch, do NOT commit on that branch. Instead: cherry-pick your change onto the correct branch (`git checkout <correct-branch> && git cherry-pick <commit-sha>`), then reset the drifted branch to its origin tip if it was pushed.
- Never push directly to `master`. All work lands on lane branches first. Merges to master are `--no-ff` and human-coordinated by Lance.
- After push, verify the correct branch advanced on origin: `git log --oneline origin/<lane-branch> -3`.

## UNITY PROCESS HYGIENE (read before every wrapper invocation)

Unity batch-mode is fragile when another Unity instance is still running or has a stale lockfile. Symptoms seen in practice: wrapper exits code 1 immediately with the log stopping at `COMMAND LINE ARGUMENTS`; wrapper exits code -1 after timeout without running any validator; log reports "could not find BloodlinesMapBootstrapAuthoring" despite the scene being intact.

Before every wrapper invocation run this preflight:

```powershell
Stop-Process -Name 'Unity','bee_backend','Unity.ILPP.Runner','Unity.ILPP.Trigger','UnityShaderCompiler','UnityPackageManager','UnityAutoQuitter' -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath 'D:\ProjectsHome\Bloodlines\unity\Temp\UnityLockfile' -Force -ErrorAction SilentlyContinue
```

If a wrapper exits 1 with a truncated log, retry once after killing processes. If it exits -1 after running some validator phases, the internal deadline was hit; bump `StartupTimeoutSeconds` in the validator file. If "could not find Authoring" appears despite a clean scene file, the Library cache is corrupt; delete `unity/Library/Bee` and `unity/Library/ScriptAssemblies` and retry.

Never run two wrapper scripts in parallel from the same session. The wrapper lock at `.unity-wrapper-lock` enforces this across sessions; respect it.

## VALIDATION GATE (canonical order)

Run serially. Unity holds a project lock; parallel invocations fail. Each wrapper typically takes 60 to 180 seconds; budget up to 540 seconds per run.

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- must exit 0 with 0 errors.
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- must exit 0 with 0 errors.
3. `scripts\Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` -- must end with `Bootstrap runtime smoke validation passed`. All prior economy + AI proof fields must remain on the success line (`gatherDepositObserved`, `trickleGainObserved`, `starvationObserved`, `loyaltyDeclineObserved`, `capPressureObserved`, `aiActivityObserved`, `aiConstructionObserved`, `stabilitySurplusObserved`, `aiMilitaryOrdersIssued >= 1`, `aiDwellings`, `aiFarms`, `aiWells`, `aiBarracks`, `productionProgressAdvancementVerified`, `constructionProgressAdvancementVerified`).
4. `scripts\Invoke-BloodlinesUnityCombatSmokeValidation.ps1` -- must end with `Combat smoke validation passed`. All eight phases must stay green (melee, projectile, explicit attack, attack-move, target-visibility, group-movement, separation, stance).
5. `scripts\Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1` -- must end with `Graphics runtime validation passed`. Every definition-backed unit and building must have a live mesh + material + faction tint.
6. `scripts\Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` -- both Bootstrap and Gameplay scene shells must validate.
7. Lane-specific smokes for any slices already landed:
   - `scripts\Invoke-BloodlinesUnityConvictionSmokeValidation.ps1` -- 4 phases.
   - `scripts\Invoke-BloodlinesUnityDynastySmokeValidation.ps1` -- 4 phases.
   - `scripts\Invoke-BloodlinesUnityFaithSmokeValidation.ps1` -- 4 phases.
   - Add your own lane smoke to this list when creating a new Tier 1 lane.
8. `node tests/data-validation.mjs` -- must exit 0.
9. `node tests/runtime-bridge.mjs` -- must exit 0.
10. `scripts\Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- must exit 0; confirms the concurrent-session contract is current.

All ten must be green before merge to master.

## SLICE COMPLETION CHECKLIST (run in order at end of slice)

1. Local working tree clean except intended files.
2. All ten validation gates green.
3. Per-slice handoff written at `docs\unity\session-handoffs\YYYY-MM-DD-unity-<lane>-<slice>.md`. Must include: Goal, Browser Reference (file + function cluster + line), Canon Reference (doc + section), Work Completed, Scope Discipline (what you deliberately did NOT do), Verification (exact pass lines from wrappers), Next Action.
4. `NEXT_SESSION_HANDOFF.md` updated with a section for this slice. Append-only.
5. `continuity\PROJECT_STATE.json` updated: `latest_unity_session_handoff` points at your handoff; add a per-lane `latest_unity_<lane>_handoff` pointer; update `concurrent_session_contract.active_lanes` / `paused_lanes` / `retired_lanes` arrays.
6. If lane state changed: update `docs\unity\CONCURRENT_SESSION_CONTRACT.md` -- bump Revision, set Last Updated to today, set Last Updated By to your session identifier (format: `<agent>-<lane>-<date>`), amend the affected lane subsection. Re-run `scripts\Invoke-BloodlinesUnityContractStalenessCheck.ps1`.
7. Commit on the lane branch with a message citing the browser reference and canon reference.
8. Push to the lane branch. Never push to master in a slice commit.

## DO NOT

- Do not re-read the full canon unless a specific system requires it.
- Do not summarize prior sessions.
- Do not propose changes to the overall approach.
- Do not ask whether the full vision is feasible.
- Do not suggest the gameplay scope is too large.
- Do not recommend an MVP pivot.
- Do not push graphics past the Zero Hour / Warcraft III fidelity ceiling. No PBR. No HDRP. No ray tracing. No AAA animation. No normal maps except where canonical. No AAA post-processing stacks.
- Do not add systems to the browser simulation. The browser runtime is frozen as behavioral specification. Only bug fixes and parity-instrumentation explicitly authorized by the operator.
- Do not produce a plan document in place of implementation. The migration plan already exists.
- Do not modify code outside your lane's owned paths as defined in the contract. Shared-file edits must follow the narrow-modification rules.
- Do not rewrite `BloodlinesDebugEntityPresentationBridge`. Narrow fallback additions only.
- Do not push to `master`. Push to the lane branch.
- Do not produce final faction-identity art autonomously. Art direction decisions (within the Zero Hour ceiling) are human-owned.
- Do not proceed against a stale contract.
- Do not run multiple Unity wrappers in parallel. Respect the wrapper lock.
- Do not silently drop browser-era behavior. Port it, rule it out with operator ack, or document the replacement.
- Do not commit to a branch other than the one you intend to advance. Check `git branch --show-current` before every commit.
- Do not claim a lane owned by the other agent. Claude claims `claude/unity-*`; Codex claims `codex/unity-*`.

## DO

- Pick up the next unblocked Unity task per the migration plan tier order.
- Confirm the task is inside a lane you are authorized to advance. If the lane is unclaimed, claim it by adding it to Active Lanes in the contract and bumping Revision.
- Implement at full canonical gameplay depth matching the bible and the browser behavior. Cite both in the slice handoff.
- Extend runtime-smoke validation to prove the behavior. Every new system gets a governed validator with at least three phases: baseline, intended transition, edge case.
- Keep presentation work sized to the Graphics Fidelity Target. Placeholder silhouettes stay low-poly. Shader logic stays minimal. Material authoring stays simple.
- Run the full canonical validation gate. All ten gates must be green.
- Use `scripts\Invoke-BloodlinesUnityWrapperWithLock.ps1` with a `-Session` identifier matching your agent and lane for every Unity batch invocation. Respect `.unity-wrapper-lock`.
- Stay on the correct lane branch. Rebase from `origin/master` before push. Push only to the lane branch.
- Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity\PROJECT_STATE.json`, and write a per-slice handoff.
- If lane state changed: update the contract (bump Revision, set Last Updated, set Last Updated By, amend affected lane, re-run the staleness check).
- Before every commit: verify `git branch --show-current` matches the intended lane. If it does not, cherry-pick onto the correct branch.
- Before every Unity wrapper invocation: kill stale Unity processes, delete the UnityLockfile if stale, respect the wrapper lock.

## CANON AND REFERENCE AUTHORITY

- Canon: `18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md` (539 KB).
- Subsystem depth: `04_SYSTEMS\`, `07_FAITHS\`, `08_MECHANICS\`, `10_UNITS\`, `11_MATCHFLOW\`, `12_UI_UX\`, `13_AUDIO_VISUAL\`.
- Behavioral reference: `src\game\core\simulation.js` (14,868 LOC, read-only spec; do not extend). `src\game\core\ai.js` (3,141 LOC). Every slice handoff cites the exact file and function cluster.
- Session ingestions: `02_SESSION_INGESTIONS\` -- most are rolled into canon; the 2026-04-15 match-structure-time-system-multiplayer-doctrine trio is doctrine-locked and canonical.
- Data source of truth: `data\*.json` synced into Unity via `Code\Editor\JsonContentImporter.cs`.
- Target runtime: `unity\Assets\_Bloodlines\` only. The `Bloodlines\` URP stub is not a work target.
- Migration authority: `docs\plans\2026-04-17-browser-to-unity-migration-plan.md`.
- Lane ownership authority: `docs\unity\CONCURRENT_SESSION_CONTRACT.md`. If this prompt and the contract disagree, the contract wins.
- Graphics lane authority: `docs\unity\VISUAL_ASSET_PIPELINE_2026-04-15.md` and `docs\unity\BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`, interpreted through the Graphics Fidelity Target above.
- Agent canonical entry for Codex: `D:\ProjectsHome\Bloodlines\AGENTS.md`.
- Agent canonical entry for Claude Code: `D:\ProjectsHome\Bloodlines\CLAUDE.md`.

## GRAPHICS AND PRESENTATION DIRECTION

The graphics lane infrastructure first slice landed on master. It is now classified as retired in the contract with follow-up polish items available. The direction is locked and does not need to be rediscussed. All direction below is interpreted through the Graphics Fidelity Target ceiling (Zero Hour / Warcraft III era polish).

Rendering path: URP Forward with Shader Graph shaders under `unity\Assets\_Bloodlines\Shaders\`. Entities.Graphics batch rendering with MaterialMeshInfo, RenderFilterSettings, and a per-instance `FactionTintComponent` float4 driven from `FactionComponent`. Faction-color shader with base albedo (optionally hand-painted diffuse texture), faction tint, silhouette rim for RTS-zoom legibility, and a selection highlight uniform. `FactionTintPalette` static table resolves faction to color. No HDRP. No ray tracing. No volumetric effects.

Visual definition layer: `UnitVisualDefinition` and `BuildingVisualDefinition` are additive ScriptableObjects keyed to `UnitDefinition` and `BuildingDefinition` by id. Never merge into the existing definitions. Binding systems in `unity\Assets\_Bloodlines\Code\Rendering\` attach visuals at unit and building spawn. `BloodlinesDebugEntityPresentationBridge` co-renders with the visual bridge; fallback to primitives remains required for entities without a visual definition.

Placeholders: procedural low-poly meshes generated at editor time via `BloodlinesPlaceholderMeshGenerator`. Every canonical unit and building has a distinct silhouette under `unity\Assets\_Bloodlines\Art\Placeholders\`. Placeholders stay within polygon budgets sized for the fidelity ceiling (1k-8k tris per unit, 2k-15k per building). Placeholders are temporary scaffolding, not a shipping state, but the final shipping art is also bounded by the same polygon budget.

Graphics lane boundary: in scope are URP Forward shader, visual-definition ScriptableObjects, low-poly placeholder meshes, Entities.Graphics binding systems, faction tint palette, graphics runtime validation wrapper, simple skeletal animation hookup, basic particle VFX for combat hits and projectile trails, faction-distinct hand-painted diffuse texture slots. Out of scope autonomously: final faction-identity art (art direction is human-owned, even within the Zero Hour ceiling), any PBR or normal-map authoring, any AAA animation pipeline, motion capture integration, portrait art, terrain art beyond simple heightmap + splat, any lighting polish beyond single-sun + ambient + cascaded shadow map.

## CONCURRENT LANE DISCIPLINE

Lane ownership, file-scope exclusivity, shared-file rules, forbidden paths, branch discipline, wrapper-lock discipline, and the canonical validation gate are all defined in `docs\unity\CONCURRENT_SESSION_CONTRACT.md`. Follow the contract. If the contract is stale, stop.

Two or more agents may run concurrently. Each agent claims exactly one lane, works the slice to completion in isolation, validates, and hands back to master. Before committing, each agent verifies its HEAD matches its claimed lane branch.

## WHAT YOU ARE CONTINUING TOWARD (Definition of Done for Bloodlines)

Bloodlines is complete when Unity ships all of the following. Each gameplay item has a governed validator proving behavior. Presentation items are sized to the Graphics Fidelity Target.

Houses and factions:
- All nine founding houses realized with distinct identity (Ironmark, Stonehelm, Goldgrave, Hartvale, Westland, Whitehall, Oldcrest, Highborne, Trueborn), each with faction color, unique units where canonical, unique buildings where canonical.
- Tribal faction kind and Neutral faction kind distinct from kingdom.

Units and buildings:
- All canonical units per house and per role (worker, villager, militia, swordsman, bowman, axeman, light cavalry, scout rider, verdant warden, commander, tribal raider, siege engineer, supply wagon, battering ram, siege tower, trebuchet, ballista, mantlet, plus house-specific variants).
- All canonical buildings per house (command hall, dwelling, farm, well, barracks, stable, siege workshop, supply camp, wayshrine, market, storehouse, granary, keep tier 1 through max, watch tower, gatehouse, wall segment, tribal camp, plus house-specific variants).

Gameplay systems (full depth; no MVP):
- All four ancient faiths (Old Light, Blood Dominion, The Order, The Wild) with intensity tiers L1 through L5 and Light/Dark doctrine divergence.
- Full conviction ledger with four buckets, five bands, and band-effect multipliers wired into loyalty, population, capture, and stabilization.
- Full dynasty cascade: members, aging, succession, commanders, marriage, cadet houses, minor-house emergence, legitimacy, loyalty pressure, fallen ledger, captives.
- Full operations and covert play: espionage, assassination, sabotage (four subtypes), counter-intelligence, dossiers.
- Full diplomacy and pact system: non-aggression pacts, alliances, acceptance drag, hostility graph.
- Full Trueborn City arc with Rise and recognition paths.
- Full world-pressure escalation with Great Reckoning and Trueborn rise trigger.
- Full naval warfare with all six vessel classes including embark/disembark.
- Full five-stage match structure (Founding, Rising, Challenge, War and Turning of Tides, Reckoning) with three-phase overlay and multi-speed time.
- Full declared-time strategic layer and dual-clock declaration seam.
- Full directive and attitude modifier systems.
- Full continental world architecture and terrain system.
- Full political event catalog.
- Full realm condition legibility (famine, water crisis, cap pressure, stability surplus, loyalty bands, logistics, world pressure).
- Full fortification tier system with layered defense and imminent-engagement warnings.
- Full siege doctrine with engineer corps, supply wagons, field water discipline, convoy escort coverage.
- Full scout raid, resource harass, logistics interdiction.
- Full state snapshot and restore.
- Full AI depth matching and exceeding the browser reference.

Presentation (bounded by the Graphics Fidelity Target):
- Zero Hour / Warcraft III era art: low-poly meshes with hand-painted diffuse textures, simple skeletal animation cycles, URP Forward rendering, faction-distinct colors via shader tint, clean and readable UI.
- All nine founding houses visually distinct at RTS zoom through silhouette, color, and simple material variation.
- Basic combat VFX: projectile trails, impact particles, death effects, blood hit flashes.
- Audio: Wwise integration for unit voice-overs, music, ambient, combat SFX. Fidelity matches the visual ceiling (appropriate for early-2000s RTS, not AAA sound design).
- UX: onboarding via HUD tooltips and panel labels, lobby, settings. Information density over visual spectacle. No interactive tutorial mode and no story campaign per `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`.

Multiplayer:
- Netcode for Entities multiplayer unless explicitly ruled out by the operator.

Each gameplay item above must have a Unity implementation, a governed validator, and a browser-reference + canon citation in its handoff. Each presentation item must stay within the Graphics Fidelity Target. Until all items above are true, Bloodlines is not done.

## TROUBLESHOOTING (common failures and their fixes)

- **Unity exits 1 immediately, log truncated at COMMAND LINE ARGUMENTS.** Stale Unity process or lockfile holds the project. Kill Unity processes, delete `unity/Temp/UnityLockfile`, retry.
- **Unity exits -1 after some phases pass.** Internal validator deadline hit. Bump `StartupTimeoutSeconds` in the affected validator file.
- **Unity build reports "could not find BloodlinesMapBootstrapAuthoring" despite scene being intact.** Library cache is stale against the current asmdef. Delete `unity/Library/Bee` and `unity/Library/ScriptAssemblies`, retry.
- **Assembly-CSharp.csproj build reports missing `.editorconfig` or `.nuget.g.targets`.** Unity regenerates these during its own build. The csproj files are gitignored. This failure mode is a dotnet-only artifact and does not affect Unity batch validation.
- **Git HEAD keeps switching to a sibling branch.** Observed in Claude Code sessions riding the continuation platform. Before every commit verify `git branch --show-current`; if drifted, cherry-pick your commit onto the correct branch. Codex CLI sessions are not known to exhibit this, but still verify.
- **Merge conflict in NEXT_SESSION_HANDOFF.md or PROJECT_STATE.json.** These are append-only end-of-slice artifacts. Resolve by keeping both sides additively. Never drop another lane's entry.
- **Contract staleness check fails.** Some lane was modified or a new handoff landed with a date newer than the contract's Last Updated. Bump Revision, set Last Updated to today, set Last Updated By to your session identifier, amend affected lane subsections, re-run the check.
- **`dotnet build` hangs or freezes.** Kill all `dotnet.exe`, `Unity.exe`, `bee_backend.exe` processes.
- **Log file cannot be deleted.** Another Unity process is writing to it. Kill Unity processes first.
- **Agent proposes PBR or AAA visual work.** Surface the Graphics Fidelity Target. Decline the proposal. Keep the slice within Zero Hour / Warcraft III ceiling.

## STATE OF PLAY (as of 2026-04-17)

Master is at commit `7cc72be5` (plus any state-document updates after). Green on all ten validation gates.

Landed Tier 1 slices:
- Conviction scoring + bands + 4-phase governed validator (merged via `7f8de3c`).
- Dynasty core: 8-member template set + aging + heir succession + interregnum + 4-phase validator (merged via `1aa6ade`).
- Faith commitment + exposure threshold + 5-tier intensity resolution + 4-phase validator (merged via `9036d91`).

Other recent merges:
- Group movement + combat stances + unit separation + recent impact + wrapper hardening (merged via `d9e58fc`; all 8 combat phases green).
- Target acquisition throttling + sight-loss cleanup (merged via `dc00fff`).
- Attack orders + attack-move commands (merged via `5167a0b`).
- Graphics infrastructure foundation: URP + faction tint + placeholder meshes + governed validator (merged via `548d780`).
- AI barracks construction + observability (merged via `3101e98`).

Remaining Tier 1 slices (any order, any agent):
- State snapshot + restore.
- Fortification + siege + imminent-engagement.
- Dual-clock + 5-stage match progression.
- Enemy AI strategic layer port from `src/game/core/ai.js`.

Graphics lane follow-up items (available to claim as a new lane, bounded by the Graphics Fidelity Target):
- Serialized toggle on `BloodlinesDebugEntityPresentationBridge` to skip primitives where visual bridge has a proxy.
- `FactionTintStrength` serialized field on the visual bridge.
- `SelectedTag` driver for selection highlight uniform.
- Per-class silhouette refinement (staves, tabards, capes) within polygon budget.

Now proceed with the next unblocked forward work.

---

## PROMPT MAINTENANCE NOTES (not part of the pasted prompt body)

Update this prompt when:

1. The migration plan changes priority order or adds/removes a Tier 1 slice.
2. A validation gate step is added, removed, or reordered.
3. A new shared file or forbidden path is added to the contract.
4. A persistent infrastructure pattern emerges that future sessions need to know about.
5. The Definition of Done changes because a new canonical subsystem is added or deprecated.
6. The Graphics Fidelity Target changes.

Do NOT update for:

- Individual slice completions. Those go in `NEXT_SESSION_HANDOFF.md` and per-slice handoffs.
- Bug fixes within a validator. Those are slice-level.
- Operator preference changes that are already captured in `CLAUDE.md`.

When updating, bump the Version header at the top and set Last Updated to today.
