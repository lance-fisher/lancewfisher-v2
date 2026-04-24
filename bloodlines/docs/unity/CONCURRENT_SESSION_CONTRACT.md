# Bloodlines Unity Concurrent Session Contract

## Contract Metadata

- Revision: 124
- Last Updated: 2026-04-23
- Last Updated By: claude-world-trade-routes-2026-04-23
- Supersedes: revision 118 (Records the player succession influence landing on canonical `master` and clears the branch-in-flight marker.)


## Purpose

This document is the single source of truth for Unity lane ownership, file-scope boundaries, shared-file rules, and forbidden paths across all concurrent Claude and Codex sessions working on the Bloodlines Unity project. Prompts and session handoffs do not hardcode lane data; they point here. Any session that needs to know what it may touch, what branches are in flight, or what the canonical validation gate is must read this document first. The contract is maintained across revisions so each bump constitutes an auditable record of how the concurrent session structure has evolved.

## Active Lanes

### Lane: economy-and-ai

- Status: retired
- Branch Prefix: `claude/unity-food-water-economy`, `claude/unity-enemy-ai-economic-base`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Economy/**`
  - `unity/Assets/_Bloodlines/Code/AI/**`
  - `unity/Assets/_Bloodlines/Code/Components/FactionLoyaltyComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs`
- Owned Scripts:
  - None dedicated. The bootstrap runtime smoke (`scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`) serves as the verification surface for this lane's work; it is a shared file governed by narrow-modification rules in the Shared Files section.
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-building-resource-trickle-and-concurrent-session-contract.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-starvation-response-and-famine-water-crisis.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-faction-loyalty-and-famine-water-crisis-delta.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-cap-pressure-response.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-stability-surplus-loyalty-restoration.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-loyalty-density-hud-readout.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-economic-base.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-construction-pass.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-militia-posture.md`
- Current Branch In Flight: none (merged into master via `aed6969` on 2026-04-17, Sessions 120-124)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-enemy-ai-militia-posture.md`

### Lane: combat-and-projectile

- Status: retired
- Branch Prefix: `codex/unity-combat-foundation`, `codex/unity-projectile-combat`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-combat-foundation.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`
- Current Branch In Flight: none (`codex/unity-combat-foundation` merged at `a8dd553`; `codex/unity-projectile-combat` merged at `2ee8096`)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-projectile-combat.md`

### Lane: graphics-infrastructure

- Status: retired (first slice merged via `548d780`; follow-up polish items unstaffed, available to claim as new lane)
- Branch Prefix: `claude/unity-graphics-infrastructure`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Rendering/**`
  - `unity/Assets/_Bloodlines/Shaders/**`
  - `unity/Assets/_Bloodlines/Code/Components/FactionTintComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Definitions/BuildingVisualDefinition.cs`
  - `unity/Assets/_Bloodlines/Code/Definitions/UnitVisualDefinition.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlaceholderMeshGenerator.cs`
- Owned Scripts:
  - None yet defined. A dedicated graphics validation wrapper should be added as part of the first substantive implementation commit on this lane.
- Lane Authority Documents:
  - `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
  - `docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
  - `docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`
- Current Branch In Flight: `claude/unity-graphics-infrastructure`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-governance-contract-source-of-truth.md`

### Lane: state-snapshot-restore

- Status: retired (all 3 sub-slices merged into master on 2026-04-17)
- Branch Prefix: `claude/unity-save-load-snapshot`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/SaveLoad/**`
  - `unity/Assets/_Bloodlines/Code/Components/SnapshotVersionComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SaveLoad.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSaveLoadSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnitySaveLoadSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- ProbeSnapshotIntegrity probe added after stabilitySurplus phase (sub-slice 3). snapshotIntegrityChecked=True in pass line.
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-save-load-snapshot.md`
- Browser Reference: `src/game/core/simulation.js` `exportStateSnapshot` (13822), `restoreStateSnapshot` (13989)
- Current Branch In Flight: none (merged into master 2026-04-17)

### Lane: combat-attack-move

- Status: retired (merged into master via `5167a0b`)
- Branch Prefix: `codex/unity-attack-orders-attack-move`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**` (extends the combat-and-projectile foundation)
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/CODEX_NEXT_PROMPT_ATTACK_MOVE.md`
  - `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`
- Current Branch In Flight: `codex/unity-attack-move`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`

### Lane: combat-acquisition-and-sight

- Status: retired (merged into master via `dc00fff`)
- Branch Prefix: `codex/unity-target-acquisition-los`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`
- Current Branch In Flight: none
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`

### Lane: ai-barracks-observability

- Status: retired (merged into master via `3101e98`)
- Branch Prefix: `claude/unity-ai-expansion-and-faith`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/AI/AIEconomyControllerComponent.cs` (additive fields: `TargetBarracksCount`, `ControlledBarracksCountCached`)
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs` (additive `TryDebugGetAIBuildingCounts`)
- Current Branch In Flight: none

### Lane: conviction-scoring

- Status: retired (merged into master via `7f8de3c`)
- Branch Prefix: `claude/unity-conviction-scoring`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Conviction/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Conviction.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesConvictionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-conviction-scoring-and-bands.md`
- Current Branch In Flight: none
- Follow-Up (pending operator design call): wire `ConvictionBandEffects.LoyaltyProtectionMultiplier` into loyalty decline, `PopulationGrowthMultiplier` into population growth, `CaptureMultiplier` into capture speed. Browser never applied these multipliers, so wiring is a design decision not an automatic port.

### Lane: conviction-band-wiring

- Status: retired (validated landing branch `codex/overnight-land-conviction-2026-04-22`; ready to advance canonical `master`)
- Branch Prefix: `codex/unity-conviction-band-wiring`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code\Economy\StarvationResponseSystem.cs`
  - `unity/Assets/_Bloodlines/Code\Economy\CapPressureResponseSystem.cs`
  - `unity/Assets/_Bloodlines/Code\Combat\AttackResolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code\Combat\ProjectileImpactSystem.cs`
  - `unity/Assets/_Bloodlines/Code\Combat\DeathResolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code\Combat\PendingCommanderCaptureComponent.cs`
  - `unity/Assets/_Bloodlines/Code\Editor\BloodlinesConvictionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-conviction-band-wiring.md`
- Browser Reference:
  - `src/game/core/simulation.js` `CONVICTION_BAND_EFFECTS` (~1849), `getProtectedLoyaltyDelta` (~4511), `tickPopulationGrowth` (~7938), and territorial `captureMultiplier` consumption (~8134)
- Current Branch In Flight: none
- Last Slice State:
  - `StarvationResponseSystem` now applies conviction-aware protection to negative loyalty deltas and famine population decline using the canonical multiplier table.
  - `CapPressureResponseSystem` now applies the same conviction loyalty protection to negative cap-pressure loyalty loss.
  - `AttackResolutionSystem`, `ProjectileImpactSystem`, and `DeathResolutionSystem` now consume the canonical conviction `CaptureMultiplier` on the narrower commander-only seam: lethal commander defeats can become captives, write `CapturedMemberElement`, and mark the matching dynasty member `Captured`.
  - `BloodlinesConvictionSmokeValidation` now proves starvation protection, cap-pressure protection, and deterministic commander capture in dedicated ECS validation worlds.
- Immediate Next Action:
  - return to the lower-priority HUD backlog next, with fortification legibility or victory-distance readout the cleanest Codex pickup after the player pact landing

### Lane: dynasty-core

- Status: retired (merged into master via `1aa6ade`)
- Branch Prefix: `claude/unity-dynasty-core`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/**`
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastySmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityDynastySmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-dynasty-core.md`
- Current Branch In Flight: none
- Follow-Up (next dynasty slice): wire `DynastyBootstrap.AttachDynasty` into `SkirmishBootstrapSystem`; extend `BloodlinesBootstrapRuntimeSmokeValidation` to assert 8 members per playable faction.

### Lane: dynasty-renown-prestige

- Status: active
- Branch Prefix: `codex/unity-dynasty-renown-prestige`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyRenownComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyRenownAccumulationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyRenown.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyRenownSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityDynastyRenownSmokeValidation.ps1`
- Shared-File Narrow Edits Planned:
  - `unity/Assembly-CSharp.csproj` -- add compile includes for the renown runtime files only if the local generated project file does not already pick them up
  - `unity/Assembly-CSharp-Editor.csproj` -- add compile includes for the dedicated renown validator only if the local generated project file does not already pick it up
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyStateComponent.cs` -- reuse dynasty legitimacy/interregnum state without reopening the retired dynasty-core lane
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- resolve the current ruling member for succession-aware prestige changes
  - `unity/Assets/_Bloodlines/Code/GameTime/DualClockComponent.cs` -- read `InWorldDays` for deterministic whole-day scoring cadence
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- read held and loyal territory counts for territorial prestige advantage
  - `unity/Assets/_Bloodlines/Code/Faith/FaithStateComponent.cs` -- read committed-faith intensity and level for covenant prestige contribution
  - `unity/Assets/_Bloodlines/Code/Victory/VictoryStateComponent.cs` -- read live win state and victory momentum
  - `unity/Assets/_Bloodlines/Code/Victory/VictoryConditionEvaluationSystem.cs` -- reuse canonical thresholds for territorial governance and divine-right progress
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-dynasty-renown-prestige.md`
- Browser Reference:
  - `src/game/core/simulation.js` per-member renown state only; no dynasty-level prestige ledger exists in browser runtime, so this lane lifts the design-bible dynasty prestige surface onto Unity ECS state
- Current Branch In Flight: none (landed on canonical `master` during the 2026-04-22 overnight build advance)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-22-unity-dynasty-renown-prestige.md`

### Lane: dynasty-succession-crisis

- Status: retired (validated rerun landed onto canonical `master` in this session)
- Branch Prefix: `codex/unity-dynasty-succession-crisis`, `codex/unity-dynasty-succession-crisis-rerun`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisEvaluationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisRecoverySystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnitySuccessionCrisisSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs` -- multiply existing control-point trickle by the crisis `ResourceTrickleFactor` only; no unrelated economy rewrites
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs` -- additive `TryDebugGetSuccessionCrisis` readout only
  - `unity/Assembly-CSharp.csproj` -- succession-crisis runtime entries retained and stale analyzer roots canonicalized back to `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- succession-crisis validator entry retained and stale analyzer roots canonicalized back to `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- read dynasty legitimacy/interregnum/member roster state without reopening the retired dynasty-core lane
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve faction ids for loyalty and economy application
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- apply opening loyalty shock and daily crisis loyalty drain to owned control points
  - `unity/Assets/_Bloodlines/Code/Components/ConvictionComponent.cs` -- scale recovery speed by the canonical conviction band ladder
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- tick crisis effects on whole in-world days
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-dynasty-succession-crisis.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-dynasty-succession-crisis-rerun.md`
- Browser Reference:
  - `src/game/core/simulation.js` `SUCCESSION_CRISIS_SEVERITY_PROFILES`, `buildSuccessionCrisisTriggerProfile`, `startSuccessionCrisis`, `tickDynastyPoliticalEvents`, `getSuccessionCrisisTerms`, `consolidateSuccessionCrisis`
- Current Branch In Flight: none (rerun branch validated and merged onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-dynasty-succession-crisis-rerun.md`
- Last Slice State:
  - the succession-crisis watch/evaluation/recovery seam is now replayed on the current master line without dragging in the stale historical branch's unrelated diffs
  - dedicated succession crisis smoke plus the full 10-step governed validation chain reran green after canonicalizing stale generated analyzer roots back to `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - fresh automation worktrees may still need a local `unity/Library` junction to `D:\ProjectsHome\Bloodlines\unity\Library` before the governed `dotnet build` gate can resolve `Library\ScriptAssemblies`
  - next Codex pickup should move to Priority 7 `faith-verdant-warden`

### Lane: dynasty-political-events

- Status: active
- Branch Prefix: `codex/unity-dynasty-political-events`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyPoliticalEventsSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityDynastyPoliticalEventsSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerDivineRightDeclarationSystem.cs` -- block only when `DivineRightFailedCooldown` is active for the source faction
  - `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` -- multiply outgoing damage by `DynastyPoliticalEventAggregateComponent.AttackMultiplier` only
  - `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingSystem.cs` -- multiply existing building trickle output by `DynastyPoliticalEventAggregateComponent.ResourceTrickleFactor` only
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs` -- multiply existing control-point trickle by the political-event resource factor only
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs` -- multiply existing loyalty stabilization by `DynastyPoliticalEventAggregateComponent.StabilizationMultiplier` only
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs` -- additive `TryDebugGetPoliticalEvents(...)` readout only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the dynasty political event runtime files, plus the already-validated succession-crisis runtime files still missing from the generated project
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile includes for the dynasty political event editor validator, plus the already-validated succession-crisis validator file still missing from the generated project
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- read active divine-right operations without touching the AI lane
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationDivineRightComponent.cs` -- read resolve windows and operation type metadata only
  - `unity/Assets/_Bloodlines/Code/Faith/FaithStateComponent.cs` -- read committed covenant, intensity, and level thresholds for divine-right failure
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve faction-root entities by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- project fixed-length cooldowns into in-world-day expiry timestamps
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-dynasty-political-events.md`
- Browser Reference:
  - `src/game/core/simulation.js` `getDynastyPoliticalEventState`, `tickDynastyPoliticalEvents`, `getFactionPoliticalEventEffects`, `failDivineRightDeclaration`, and the covenant/divine-right cooldown constants block
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-22-unity-dynasty-political-events.md`
- Last Slice State:
  - faction roots now hold timed dynasty political event buffers and a computed aggregate multiplier snapshot
  - failed divine-right upkeep now writes a timed `DivineRightFailedCooldown` political event that blocks immediate redeclaration
  - resource trickle, attack damage, and control-point stabilization now consume the aggregate multipliers through narrow shared-file seams
  - `BloodlinesDynastyPoliticalEventsSmokeValidation` proves aggregate application, cooldown creation, and expiry reset
  - the follow-on covenant-test slice has now landed and consumes `CovenantTestCooldown` as a real failure output rather than a placeholder

### Lane: faith-commitment

- Status: retired (merged into master via `9036d91`)
- Branch Prefix: `claude/unity-faith-commitment`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFaithSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-faith-commitment-and-intensity.md`
- Current Branch In Flight: none
- Follow-Up (later faith slices): sacred-site exposure walker, structure intensity regen, wayshrine amplification, covenant tests, holy wars, divine right, conviction event on commit.

### Lane: faith-covenant-test

- Status: active (validated implementation landed on canonical `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-faith-covenant-test`, `codex/unity-player-covenant-test-dispatch`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/CovenantTestStateComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestRequestComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestDispatchStateComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestDispatchSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/CovenantTestQualificationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/CovenantTestResolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCovenantTestSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovenantTestDispatchSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCovenantTestSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerCovenantTestDispatchSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs` -- additive covenant-test trigger/readout and test-only intensity setter helpers only
  - `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerDivineRightDeclarationSystem.cs` -- player divine-right declaration now requires `CovenantTestStateComponent.TestPhase == Complete`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs` -- validator seeding only so the pre-existing player divine-right smoke remains focused on its intended seam
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the covenant-test runtime files and canonicalized Unity.Entities analyzer paths
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the covenant-test validator and canonicalized Unity.Entities analyzer paths
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs` -- additive covenant-test dispatch queue/readout helpers while preserving the older direct trigger path for legacy smoke coverage
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for `PlayerCovenantTestDispatchStateComponent.cs` and `PlayerCovenantTestDispatchSystem.cs`; stale analyzer paths were rewritten back to canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for `BloodlinesPlayerCovenantTestDispatchSmokeValidation.cs`; stale analyzer paths were rewritten back to canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventComponent.cs` -- consume the landed political-event cooldown buffer shape only
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventSystem.cs` -- reuse the existing timed cooldown seam without reopening that lane
  - `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/PlayerFaithDeclarationUtility.cs` -- reuse faction-root lookup and kingdom gate helpers
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/ResourceStockpileComponent.cs` -- read/write covenant-test resource costs only
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read/write covenant-test population costs only
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyStateComponent.cs` -- apply covenant-test legitimacy gain/loss only
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- project the 180-day hold and retry cooldown onto in-world time
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-faith-covenant-test.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-covenant-test-dispatch.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-covenant-test-dispatch-landing.md`
- Browser Reference:
  - `src/game/core/simulation.js` constants `COVENANT_TEST_INTENSITY_THRESHOLD`, `COVENANT_TEST_DURATION_IN_WORLD_DAYS`, `COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS`, the covenant-test cost block near lines 134-141, `performCovenantTestAction`, and `ensureFaithCovenantTestCompletionFromLegacyState`
- Current Branch In Flight: none (merged onto canonical `master` via `de39a803` on 2026-04-23)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-player-covenant-test-dispatch-landing.md`
- Last Slice State:
  - `PlayerCovenantTestDispatchStateComponent` and `PlayerCovenantTestDispatchSystem` now live on canonical `master`, so the player faction root exposes real covenant-test availability, affordability, cost, queued, and pending-request state before the existing resolution seam consumes the request
  - `BloodlinesDebugCommandSurface.Faith` now exposes `TryDebugQueueCovenantTestDispatch(...)` and `TryDebugGetCovenantTestDispatchState(...)`, and the older direct trigger path remains intact for the legacy covenant-test validator
  - `BloodlinesPlayerCovenantTestDispatchSmokeValidation` plus `scripts/Invoke-BloodlinesUnityPlayerCovenantTestDispatchSmokeValidation.ps1` now prove availability/cost display, queued request emission, resolution consumption, and unaffordable rite blocking on the merged master-compatible line
  - the merged result reran the full governed 10-gate chain plus the dedicated player covenant-test dispatch smoke green, and `unity/Assembly-CSharp*.csproj` analyzer roots were re-canonicalized back to `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - next Codex pickup should move to Priority 19 `contested-territory-pressure`

### Lane: territory-governor-specialization

- Status: active
- Branch Prefix: `codex/unity-territory-governor-specialization`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/TerritoryGovernance/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Governance.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGovernorSpecializationSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityGovernorSpecializationSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs` -- additive governor trickle multiplier only
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs` -- additive governor stabilization, capture resistance, and loyalty-protection hooks only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs` -- additive settlement-governor reserve heal and muster tempo scaling only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the territory-governance runtime files plus governance debug partial
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the governor specialization validator
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- read owned territory state and settlement-class specialization keys only
  - `unity/Assets/_Bloodlines/Code/Components/SettlementComponent.cs` -- read settlement class and fortification tier only
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- resolve eligible governance members only
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve owning faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- gate assignment refresh to in-world whole days
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs` -- scale reserve cadence on governed keeps only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-territory-governor-specialization.md`
- Browser Reference:
  - `src/game/core/simulation.js` `DEFAULT_GOVERNOR_SPECIALIZATION`, `GOVERNOR_SPECIALIZATION_PROFILES`, `getGovernorSpecializationIdForSettlementClass`, `getGovernanceSeatMembers`, `getGovernanceSeatMemberScore`, `syncGovernorAssignments`, and `getGovernorProfileForControlPoint`
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-22-unity-territory-governor-specialization.md`
- Last Slice State:
  - control points and settlements now receive daily governor seat assignments from eligible dynasty governance members using browser-aligned role priority plus specialization scoring
  - governed control points now project trickle, stabilization, capture resistance, and loyalty protection; governed keeps now accelerate reserve healing and muster cadence
  - `BloodlinesDebugCommandSurface.Governance` and `BloodlinesGovernorSpecializationSmokeValidation` prove assignment/readout, live consumer gains, and ungoverned cleanup
  - next Codex pickup should move to Priority 5 `combat-commander-aura`

### Lane: combat-commander-aura

- Status: active
- Branch Prefix: `codex/unity-combat-commander-aura`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/CommanderAuraComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Combat/CommanderAuraCanon.cs`
  - `unity/Assets/_Bloodlines/Code/Combat/CommanderAuraSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.CommanderAura.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCommanderAuraSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCommanderAuraSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` -- additive commander aura attack-cadence hook only
  - `unity/Assets/_Bloodlines/Code/Combat/CombatStanceResolutionSystem.cs` -- additive commander aura morale-retreat resistance hook only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for commander aura runtime and debug files only
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the commander aura validator only
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Combat/CommanderComponent.cs` -- read commander member identity and renown only
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve same-faction recipients and faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/ConvictionComponent.cs` -- read the current conviction band for aura scaling only
  - `unity/Assets/_Bloodlines/Code/Faith/FaithStateComponent.cs` -- read selected covenant and doctrine path for doctrine aura bonuses only
  - `unity/Assets/_Bloodlines/Code/Components/PositionComponent.cs` -- resolve commander-to-unit distance only
  - `unity/Assets/_Bloodlines/Code/Combat/CombatStatsComponent.cs` -- apply and restore runtime attack and sight deltas only
  - `unity/Assets/_Bloodlines/Code/Pathing/MovementStatsComponent.cs` -- apply and restore runtime speed deltas only
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- remove aura state when commanders or recipients are dead only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-22-unity-combat-commander-aura.md`
- Browser Reference:
  - `src/game/core/simulation.js` `COMMANDER_BASE_AURA_RADIUS`, `getCommanderAuraProfile`, `CONVICTION_BAND_EFFECTS`, and `getConvictionBandEffects`
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-22-unity-combat-commander-aura.md`
- Last Slice State:
  - commanders now resolve doctrine-aware, conviction-scaled aura profiles and apply the strongest nearby same-faction aura to friendly units each simulation frame
  - in-range allies now receive live attack, sight, speed, and morale-retreat buffs that cleanly restore when they leave range or the commander dies
  - `BloodlinesDebugCommandSurface.CommanderAura` and `BloodlinesCommanderAuraSmokeValidation` prove readout, in-range buffing, out-of-range non-application, and death cleanup
  - next Codex pickup should move to Priority 6 `fortification-postures`

### Lane: fortification-postures

- Status: active
- Branch Prefix: `codex/unity-fortification-postures`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementPostureComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Fortification/PlayerImminentEngagementPostureRequestComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementPostureSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.Posture.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementPostureSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityImminentEngagementPostureSmokeValidation.ps1`
- Coordinated Same-Agent Edits On Fortification-Owned Paths:
  - `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementWarningSystem.cs` -- preserve player-selected posture ids during active warning windows while AI settlements still auto-resolve posture choice
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs` -- consume posture heal, muster, frontline-target, and retreat-threshold hooks only
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` -- additive frontline attack bonus seam only
  - `unity/Assets/_Bloodlines/Code/Combat/CombatStanceResolutionSystem.cs` -- additive retreat-threshold seam only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for fortification posture runtime and debug files only
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the fortification posture validator only
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/ImminentEngagementComponent.cs` -- reuse the existing settlement-side warning window state only
  - `unity/Assets/_Bloodlines/Code/Fortification/ImminentEngagementCanon.cs` -- reuse the browser-aligned posture tuning table only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveAssignmentComponent.cs` -- resolve frontline defenders and reserve duty state only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationSettlementLinkComponent.cs` -- resolve fortification-linked defenders back to their settlement anchor only
  - `unity/Assets/_Bloodlines/Code/Components/SettlementComponent.cs` -- resolve settlement ids and keep classes only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-fortification-postures.md`
- Browser Reference:
  - `src/game/core/simulation.js` `IMMINENT_ENGAGEMENT_POSTURES` (~271-308) and `tickImminentEngagementWarnings` (~11742-11873)
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-fortification-postures.md`
- Last Slice State:
  - active imminent-engagement windows now materialize brace / steady / counterstroke posture state for player and AI settlements
  - reserve healing, muster tempo, frontline attack bonus, and retreat thresholds now consume live posture data through narrow additive hooks only
  - `BloodlinesDebugCommandSurface.Fortification.Posture` and `BloodlinesImminentEngagementPostureSmokeValidation` prove debug selection, one-shot player request application, damage/heal deltas, and cleanup after threat resolution
  - next Codex pickup should move to Priority 7 `faith-verdant-warden`

### Lane: faith-verdant-warden

- Status: active
- Branch Prefix: `codex/unity-faith-verdant-warden`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/VerdantWardenComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/VerdantWardenSupportSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.VerdantWarden.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVerdantWardenSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityVerdantWardenSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs` -- additive Verdant Warden stabilization and loyalty-protection hooks only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs` -- additive Verdant Warden reserve-heal, reserve-muster, and desired-frontline hooks only
  - `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` -- additive Verdant Warden frontline defender attack seam only
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- additive cached Verdant Warden control-point coverage fields only
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs` -- additive cached Verdant Warden fortification coverage fields only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the Verdant Warden runtime and debug files only
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the Verdant Warden validator only
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/UnitTypeComponent.cs` -- identify `verdant_warden` units and keep the browser support seam keyed to unit type only
  - `unity/Assets/_Bloodlines/Code/Components/PositionComponent.cs` -- resolve live support radius against control points and fortified settlements only
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- ignore dead wardens and dead linked defenders only
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve same-faction support ownership only
  - `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs` -- reuse tile size when translating control-point support radius into world-space distance only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveAssignmentComponent.cs` -- resolve engaged or mustering frontline defenders only
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationSettlementLinkComponent.cs` -- resolve linked defenders back to their fortified settlement anchor only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-faith-verdant-warden.md`
- Browser Reference:
  - `src/game/core/simulation.js` `isVerdantWardenUnit`, `getVerdantWardenSupportProfile`, `getVerdantWardenZoneSupportProfile`, and `DEFAULT_VERDANT_WARDEN_SUPPORT`
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-faith-verdant-warden.md`
- Last Slice State:
  - same-faction living Verdant Warden units now auto-materialize capped control-point and fortification support coverage each simulation tick
  - control-point stabilization / loyalty protection, reserve healing / muster tempo, and frontline defender attack now consume cached Verdant Warden support through narrow additive hooks only
  - `BloodlinesDebugCommandSurface.Faith.VerdantWarden` and `BloodlinesVerdantWardenSmokeValidation` prove single-stack coverage, debug readout, reserve-heal uplift, and the canonical three-stack cap on stored multipliers and frontline damage
  - next Codex pickup should move to Priority 8 `faith-exposure-walker`

### Lane: faith-exposure-walker

- Status: active
- Branch Prefix: `codex/unity-faith-exposure-walker`, `codex/unity-faith-exposure-walker-sacred-site`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/FaithExposureWalkerComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/FaithExposureWalkerSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.ExposureWalker.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithExposureWalkerSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFaithExposureWalkerSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs` -- additive sacred-site seed buffer element only
  - `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs` -- additive sacred-site seed ingestion and covenant-id resolution only
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs` -- additive sacred-site seed baking and covenant-id resolution only
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- additive sacred-site entity spawn helper and seed buffer consumption only
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the exposure walker runtime and debug files only
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the exposure walker validator only
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve faction roots and same-faction sacred-site/unit ownership only
  - `unity/Assets/_Bloodlines/Code/Components/FactionKindComponent.cs` -- restrict exposure gain to kingdom factions only
  - `unity/Assets/_Bloodlines/Code/Components/BuildingTypeComponent.cs` -- resolve wayshrine / covenant hall / grand sanctuary multiplier profiles from canonical type ids only
  - `unity/Assets/_Bloodlines/Code/Components/ConstructionComponents.cs` -- ignore under-construction faith structures when projecting multipliers
  - `unity/Assets/_Bloodlines/Code/Components/PositionComponent.cs` -- resolve sacred-site and faith-structure world radii only
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- ignore dead units and dead structures only
  - `unity/Assets/_Bloodlines/Code/Definitions/MapDefinition.cs` -- read `sacredSites` authoring data only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-faith-exposure-walker.md`
- Browser Reference:
  - `src/game/core/simulation.js` `updateFaithExposure` (~8174) and `getWayshrineExposureMultiplierAt` (~8246)
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-faith-exposure-walker.md`
- Last Slice State:
  - sacred sites now bootstrap as ECS entities with covenant id, world-space radius, and exposure rate carried directly from `MapDefinition.sacredSites`
  - kingdom factions with living units inside a sacred-site radius now gain browser-aligned exposure through `FaithScoring.RecordExposure`, which also discovers the faith on first exposure
  - completed wayshrines, covenant halls, and grand sanctuaries now auto-tag into the multiplier seam and amplify same-faction sacred-site exposure up to the canonical `4.0x` / four-contributor cap
  - `BloodlinesDebugCommandSurface.Faith.ExposureWalker` and `BloodlinesFaithExposureWalkerSmokeValidation` prove base gain, wayshrine amplification, under-construction ignore, kingdom-only filtering, and the stacked multiplier cap
  - next Codex pickup should move to Priority 9 `faith-structure-regen`

### Lane: faith-structure-regen

- Status: active
- Branch Prefix: `codex/unity-faith-structure-regen`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Faith/FaithStructureRegenComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Faith/FaithStructureRegenSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFaithStructureRegenSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFaithStructureRegenSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for the faith structure regen runtime files only
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for the dedicated faith structure regen validator only
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- read committed faith and mutate the existing intensity field only
  - `unity/Assets/_Bloodlines/Code/Components/BuildingTypeComponent.cs` -- resolve canonical faith-building ids only
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- ignore destroyed faith structures only
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- batch regen on whole in-world days while preserving browser per-second scaling
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-faith-structure-regen.md`
- Browser Reference:
  - `src/game/core/simulation.js` `getCompletedFaithBuildings`, `getFaithStructureIntensityRegenRate`, and `updateFaithStructureIntensity`
- Current Branch In Flight: none (validated implementation landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-faith-structure-regen.md`
- Last Slice State:
  - completed `wayshrine`, `covenant_hall`, `grand_sanctuary`, and `apex_covenant` buildings now contribute their browser `faithIntensityRegenBonus` values to committed faction intensity
  - the total regen rate now clamps at the canonical `1.4` per-second ceiling and applies on whole in-world days through `DualClockComponent`
  - `FaithStructureRegenComponent` caches current rate, counted structure total, and last applied delta so the seam is directly inspectable in ECS validation worlds
  - `BloodlinesFaithStructureRegenSmokeValidation` proves richer faith infrastructure outpaces a single shrine and the cap holds regardless of building count
  - next Codex pickup should move to Priority 10 `player-captive-ransom-trickle`

### Lane: combat-group-movement-and-stances

- Status: retired (merged into master via `d9e58fc`; all 8 combat phases green)
- Branch Prefix: `codex/unity-group-movement-and-stances`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/**` (group movement, combat stances, unit separation, recent impact recovery)
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Stances.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Movement.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` (phases 1-8: melee, projectile, explicit attack, attack-move, target-visibility, group-movement, separation, stance)
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-group-movement-and-stances.md`
- Current Branch In Flight: none

### Lane: dual-clock-match-progression

- Status: retired (all 3 sub-slices merged into master on 2026-04-17)
- Branch Prefix: `claude/unity-match-progression`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Time/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityMatchProgressionSmokeValidation.ps1`
- Shared-File Narrow Edits Planned:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- match-progression probe to be added after save-load integrity phase (sub-slice 2 or 3)
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- seed DualClockComponent + MatchProgressionComponent singletons (sub-slice 1)
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-dual-clock-match-progression.md` (written at slice close)
- Browser Reference:
  - `src/game/core/simulation.js` `tickDualClock` (13795), `updateMatchProgressionState` (13557), `computeMatchProgressionState` (13426), `getMatchProgressionSnapshot` (13650), `declareInWorldTime` (13800)
  - Stage definitions: `founding` (1), `expansion_identity` (2), `encounter_establishment` (3), `war_turning_of_tides` (4), `final_convergence` (5)
  - Phase labels: emergence, commitment, resolution
  - `DUAL_CLOCK_DEFAULT_DAYS_PER_REAL_SECOND = 2`
- Current Branch In Flight: `claude/unity-match-progression`
- Last Slice Handoff: none yet

### Lane: world-pressure-escalation

- Status: retired
- Branch Prefix: `claude/unity-world-pressure`
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/WorldPressure/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesWorldPressureSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityWorldPressureSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs` -- stage 5 convergence signal: `|| playerWorldPressureConvergence` added (WorldPressureComponent.Targeted && Level >= 3)
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- ProbeWorldPressureIntegrity probe added after matchProgressionChecked phase; worldPressureChecked field added
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs` -- new partial class file (TryDebugGetWorldPressure, TryDebugGetWorldPressureCycleTracker)
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-18-unity-world-pressure-escalation.md`
- Browser Reference:
  - `src/game/core/simulation.js` `updateWorldPressureEscalation` (13709), `calculateWorldPressureScore` (13225), `applyWorldPressureConsequences` (13695), `getWorldPressureSourceBreakdown` (13162), `getWorldPressureConvergenceProfile` (13272)
  - Score sources ported: territoryExpansion = max(0, territories - 2), greatReckoning = 4 when GR target
  - Score sources reserved for later: divineRightDeclaration, territorialGovernanceRecognition, offHomeHoldings, holyWar, captives, hostileOperations, darkExtremes
  - Dominant leader: score >= 4 and strictly highest; streak thresholds: 1/3/6 for levels 1/2/3
  - GREAT_RECKONING_PRESSURE_SCORE = 4 (simulation.js:406)
- Current Branch In Flight: none (merged into master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-18-unity-world-pressure-escalation.md`

### Lane: world-governance-coalition

- Status: active (validated governance coalition-pressure slice landed on canonical `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-governance-coalition-pressure`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialGovernanceRecognitionComponent.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/GovernanceCoalitionPressureSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGovernanceCoalitionPressureSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityGovernanceCoalitionPressureSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureComponent.cs` -- additive `TerritorialGovernanceRecognitionScore` read-model only
  - `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureEscalationSystem.cs` -- consume governance recognition contribution without reopening the older escalation score sources
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs` -- additive `TryDebugGetGovernanceCoalitionState` readout only
  - `unity/Assets/_Bloodlines/Code/Victory/VictoryConditionEvaluationSystem.cs` -- consume the live recognition component first for Territorial Governance completion while preserving the prior fallback
  - `unity/Assembly-CSharp.csproj` -- compile includes retained for the governance coalition runtime files
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include retained for the governance coalition validator
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve kingdom faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- reuse committed-faith selection/intensity for acceptance support without reopening the faith lane
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- scale prosperity support from live population against food/water stockpiles
  - `unity/Assets/_Bloodlines/Code/Components/SettlementComponent.cs` -- verify dynastic primary-seat coverage and keep ownership for recognition readiness
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- score territory share, loyal holdings, contested marches, and weakest-frontier loyalty
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyStateComponent.cs` -- reuse legitimacy and interregnum-adjacent dynasty state without reopening dynasty-owned source
  - `unity/Assets/_Bloodlines/Code/Components/ConvictionComponent.cs` -- project conviction-band acceptance support without reopening the conviction lane
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- tick sustain/victory timers on deterministic in-world-day time
  - `unity/Assets/_Bloodlines/Code/Time/MatchProgressionComponent.cs` -- enforce the stage-5 territorial-governance trigger floor
  - `unity/Assets/_Bloodlines/Code/TerritoryGovernance/GovernorSeatAssignmentComponent.cs` -- verify governed territory and primary-seat coverage without widening the territory-governance lane
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationHolyWarComponent.cs` -- block recognition readiness while a faction is inside a live holy war window without touching `AI/**`
  - `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs` -- reuse the existing governance pressure booleans as AI-facing read-model outputs without editing Claude's AI-owned file
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-governance-coalition-pressure.md`
- Browser Reference:
  - `src/game/core/simulation.js` `getTerritorialGovernanceAcceptanceProfile`, `shouldIssueTerritorialGovernanceRecognition`, `tickTerritorialGovernanceRecognition`, `getTerritorialGovernanceWorldPressureContribution`, governance-alliance pressure block inside `tickRealmConditionCycle`
- Current Branch In Flight: none (validated slice landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-governance-coalition-pressure.md`

### Lane: world-contested-territory-pressure

- Status: active (validated slice landed on canonical `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-contested-territory-pressure`, `codex/unity-contested-territory-pressure-rerun`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureComponent.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureEvaluationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesContestedTerritoryPressureSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityContestedTerritoryPressureSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs` -- additive `TryDebugGetTerritorialPressureState(...)` readout plus contested / hold-ready fields on the existing governance readout only
  - `unity/Assembly-CSharp.csproj` -- compile includes for the contested-territory runtime files
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include for the contested-territory validator
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve kingdom faction roots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/FactionKindComponent.cs` -- constrain the read-model to kingdom factions only
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- count owned vs external contested control points and identify the weakest owned contested march
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialGovernanceRecognitionComponent.cs` -- project contested-governance blocking from the already-landed governance recognition seam
  - `unity/Assets/_Bloodlines/Code/WorldPressure/GovernanceCoalitionPressureSystem.cs` -- preserve ordering adjacency with the live contested-territory acceptance rules
  - `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureComponent.cs` -- prove contested-territory counting does not create a new score source
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-world-contested-territory-pressure.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-contested-territory-pressure-landing.md`
- Browser Reference:
  - `src/game/core/simulation.js` `getRealmConditionSnapshot`, `getTerritorialGovernanceAcceptanceProfile`, `getTerritorialGovernanceWorldPressureContribution`, and `getRivalContactProfile`
  - `tests/runtime-bridge.mjs` realm-condition snapshot assertions plus the contested territorial-governance stage-4 setup
- Current Branch In Flight: none (merged onto canonical `master` via `ea359daf` on 2026-04-23)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-world-contested-territory-pressure-landing.md`
- Last Slice State:
  - kingdom faction roots on canonical `master` now expose external contested-territory count, owned contested-territory count, weakest owned contested march, and a governance-blocking flag without reopening capture-resolution behavior
  - `BloodlinesDebugCommandSurface.WorldPressure` now exposes `TryDebugGetTerritorialPressureState(...)`, and the governance readout now reports contested-count / hold-ready fields for smoke inspection
  - `BloodlinesContestedTerritoryPressureSmokeValidation` plus `scripts/Invoke-BloodlinesUnityContestedTerritoryPressureSmokeValidation.ps1` prove realm-condition counting, governance blocking, and contest-clear recovery on the merged master-compatible line
  - the merged result reran the full governed 10-gate chain plus the dedicated contested-territory pressure smoke green, and fresh automation worktrees may still need a local `unity/Library` junction to `D:\ProjectsHome\Bloodlines\unity\Library` before `dotnet build` can resolve `Library\ScriptAssemblies`

### Lane: world-trueborn-rise

- Status: active (validated Trueborn rise-arc sub-slices 1-3 and the recognized-pressure follow-up are now landed on canonical `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-world-trueborn-rise-arc-1`, `codex/unity-world-trueborn-rise-arc-2`, `codex/unity-world-trueborn-rise-arc-3`, `codex/unity-world-trueborn-recognized-pressure`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRiseArcComponent.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRiseArcSystem.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/PlayerTruebornRecognitionRequestComponent.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRecognitionUtility.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRecognitionResolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornDiplomaticEscalationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTruebornRiseArcSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTruebornDiplomaticEscalationSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityTruebornRiseArcSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityTruebornDiplomaticEscalationSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs` -- additive `TryDebugGetTruebornRiseArc(...)`, `TryDebugSetTruebornRecognition(...)`, `TryDebugRecognizeTrueborn(...)`, `TryDebugGetTruebornRecognitionState(...)`, and `TryDebugGetTruebornUltimatumState(...)` only
  - `unity/Assembly-CSharp.csproj` -- compile includes retained for the Trueborn rise runtime files, recognition request / utility / resolution registrations, new diplomatic-escalation system registration, and canonicalized Unity analyzer roots at `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile includes retained for the Trueborn rise validators, plus canonicalized Unity analyzer roots at `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve kingdom vs neutral faction roots and the `trueborn_city` singleton faction id
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- count kingdom territories and apply loyalty erosion to owned control points
  - `unity/Assets/_Bloodlines/Code/Combat/HostilityComponent.cs` -- project the browser hostility-driven Trueborn challenge contribution without widening `AI/**`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyStateComponent.cs` -- reuse live kingdom legitimacy for recognition costs and stage-2/3 pressure
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyRenownComponent.cs` -- apply the browser standing bonus as a renown bump on recognition
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventComponent.cs` -- clear recognition-linked cooldown events and rebuild the aggregate through the existing event utility
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- tick stage advancement on deterministic whole in-world days
  - `unity/Assets/_Bloodlines/Code/Time/MatchProgressionComponent.cs` -- project the dominant-kingdom / Great Reckoning stage gate for ultimatum target selection without reopening the time lane
  - `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs` -- preserve intended ordering adjacency for late-stage ultimatum issuance only
  - `unity/Assets/_Bloodlines/Code/WorldPressure/GovernanceCoalitionPressureSystem.cs` -- align the rise arc alongside the existing world-pressure seam without reopening governance implementation
  - `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureEscalationSystem.cs` -- preserve intended world-pressure ordering adjacency without widening the escalation score sources
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-rise-arc-1.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-rise-arc-2.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-rise-arc-3.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-rise-arc-3-landing.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-recognized-pressure.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-recognized-pressure-landing.md`
- Browser Reference:
  - `src/game/core/simulation.js` `tickTruebornRiseArc`, `getTruebornChallengeLevel`, `getTruebornRecognitionTerms`, `recognizeTruebornClaim`, `recognizedPressureMultiplier`, and `TRUEBORN_RISE_STAGE_*` constants
- Current Branch In Flight: none (merged to canonical `master` via `71c19cde` on 2026-04-23)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-world-trueborn-recognized-pressure-landing.md`

### Lane: ai-strategic-layer

- Status: active
- Branch Prefix: `claude/unity-ai-missionary-resolution-rebased` (Bundle 4: sub-slice 25; cherry-picked onto rewritten master after upstream history rewrite orphaned the original `claude/unity-ai-missionary-resolution` branch); prior claude/unity-ai-captive-rescue-and-ransom-execution (Bundle 3: sub-slices 23+24), claude/unity-ai-holy-war-and-divine-right-execution (Bundle 2: sub-slices 21+22), claude/unity-ai-captive-state-and-missionary-execution (Bundle 1: sub-slices 19+20), claude/unity-ai-dynasty-operations-foundation (18), claude/unity-ai-narrative-back-wire (17), claude/unity-ai-narrative-message-bridge (16), claude/unity-ai-pact-break-expiration (15), claude/unity-ai-pact-proposal-execution (14), claude/unity-ai-lesser-house-promotion (13), claude/unity-ai-marriage-acceptance-terms (12), claude/unity-ai-marriage-accept-effects (11), claude/unity-ai-marriage-strategic-profile (10), claude/unity-ai-marriage-inbox-accept (9), codex/unity-ai-command-dispatch (8) also landed; future sub-slices on new branches
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/EnemyAIStrategySystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIStrategicPressureSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIWorkerGatherSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AISiegeOrchestrationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AISiegeOrchestrationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AICovertOpsComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AICovertOpsSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIBuildOrderComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIBuildOrderSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMarriageProposalExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMarriageInboxAcceptSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMarriageStrategicProfileSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/MarriageAcceptEffectsPendingTag.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMarriageAcceptEffectsSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/MarriageAcceptanceTermsComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/MarriageAuthorityEvaluator.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AILesserHousePromotionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/PactComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIPactProposalExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/PactBreakRequestComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/NarrativeMessageComponents.cs`
  - `unity/Assets/_Bloodlines/Code/AI/NarrativeMessageBridge.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationLimits.cs`
  - `unity/Assets/_Bloodlines/Code/AI/CapturedMemberElement.cs`
  - `unity/Assets/_Bloodlines/Code/AI/CapturedMemberHelpers.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationMissionaryComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMissionaryExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationHolyWarComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIHolyWarExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationDivineRightComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIDivineRightExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCaptiveRescueComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AICaptiveRescueExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCaptiveRansomComponent.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AICaptiveRansomExecutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIMissionaryResolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/PactBreakSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AIWorkerCommandSystem.cs`
  - `unity/Assets/_Bloodlines/Code/AI/AITerritoryDispatchSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Components/WorkerGatherOrderComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIStrategySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIStrategicPressureSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIGovernancePressureSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIWorkerGatherSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAISiegeOrchestrationSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAICovertOpsSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIBuildOrderSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageProposalExecutionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageInboxAcceptSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageStrategicProfileSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageAcceptEffectsSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIMarriageAcceptanceTermsSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAILesserHousePromotionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIPactProposalExecutionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPactBreakSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNarrativeMessageBridgeSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAINarrativeBackWireSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyOperationsSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesHolyWarAndDivineRightExecutionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAICommandDispatchSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AIStrategy.cs`
  - `scripts/Invoke-BloodlinesUnityAIStrategySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIStrategicPressureSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIGovernancePressureSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIWorkerGatherSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAISiegeOrchestrationSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAICovertOpsSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIBuildOrderSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIMarriageProposalExecutionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIMarriageInboxAcceptSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIMarriageStrategicProfileSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIMarriageAcceptEffectsSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIMarriageAcceptanceTermsSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAILesserHousePromotionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAIPactProposalExecutionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPactBreakSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityNarrativeMessageBridgeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAINarrativeBackWireSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityDynastyOperationsSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityCaptiveStateAndMissionaryExecutionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityHolyWarAndDivineRightExecutionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityAICommandDispatchSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- `AIStrategyComponent` seeded on non-player Kingdom faction entities alongside `AIEconomyControllerComponent`
  - `unity/Assembly-CSharp.csproj` -- `AIStrategyComponent.cs`, `EnemyAIStrategySystem.cs`, `AIStrategicPressureSystem.cs`, `AIWorkerGatherSystem.cs`, `AISiegeOrchestrationComponent.cs`, `AISiegeOrchestrationSystem.cs`, `AICovertOpsComponent.cs`, `AICovertOpsSystem.cs`, `AIBuildOrderComponent.cs`, `AIBuildOrderSystem.cs`, `AIMarriageProposalExecutionSystem.cs`, `AIMarriageInboxAcceptSystem.cs`, `AIMarriageStrategicProfileSystem.cs`, `MarriageAcceptEffectsPendingTag.cs`, `AIMarriageAcceptEffectsSystem.cs`, `MarriageAcceptanceTermsComponent.cs`, `MarriageAuthorityEvaluator.cs`, `AILesserHousePromotionSystem.cs`, `PactComponent.cs`, `AIPactProposalExecutionSystem.cs`, `PactBreakRequestComponent.cs`, `PactBreakSystem.cs`, `NarrativeMessageComponents.cs`, `NarrativeMessageBridge.cs`, `DynastyOperationComponent.cs`, `DynastyOperationLimits.cs`, `CapturedMemberElement.cs`, `CapturedMemberHelpers.cs`, `DynastyOperationMissionaryComponent.cs`, `AIMissionaryExecutionSystem.cs`, `DynastyOperationHolyWarComponent.cs`, `AIHolyWarExecutionSystem.cs`, `DynastyOperationDivineRightComponent.cs`, `AIDivineRightExecutionSystem.cs`, `AIWorkerCommandSystem.cs`, `AITerritoryDispatchSystem.cs`, `WorkerGatherOrderComponent.cs` registered
  - `unity/Assembly-CSharp-Editor.csproj` -- `BloodlinesAIStrategySmokeValidation.cs`, `BloodlinesAIStrategicPressureSmokeValidation.cs`, `BloodlinesAIGovernancePressureSmokeValidation.cs`, `BloodlinesAIWorkerGatherSmokeValidation.cs`, `BloodlinesAISiegeOrchestrationSmokeValidation.cs`, `BloodlinesAICovertOpsSmokeValidation.cs`, `BloodlinesAIBuildOrderSmokeValidation.cs`, `BloodlinesAIMarriageProposalExecutionSmokeValidation.cs`, `BloodlinesAIMarriageInboxAcceptSmokeValidation.cs`, `BloodlinesAIMarriageStrategicProfileSmokeValidation.cs`, `BloodlinesAIMarriageAcceptEffectsSmokeValidation.cs`, `BloodlinesAIMarriageAcceptanceTermsSmokeValidation.cs`, `BloodlinesAILesserHousePromotionSmokeValidation.cs`, `BloodlinesAIPactProposalExecutionSmokeValidation.cs`, `BloodlinesPactBreakSmokeValidation.cs`, `BloodlinesNarrativeMessageBridgeSmokeValidation.cs`, `BloodlinesAINarrativeBackWireSmokeValidation.cs`, `BloodlinesDynastyOperationsSmokeValidation.cs`, `BloodlinesCaptiveStateAndMissionaryExecutionSmokeValidation.cs`, `BloodlinesHolyWarAndDivineRightExecutionSmokeValidation.cs`, `BloodlinesAICommandDispatchSmokeValidation.cs` registered
  - `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs` -- workers now must travel inside `GatherRadius` before harvesting; `AIWorkerCommandSystem` may flip `Seeking -> Gathering` immediately but harvest does not start until arrival
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs` -- read `MarriageComponent` (already-married gate) and `MarriageProposalComponent` / `MarriageProposalStatus` (already-pending gate, proposal creation, accept flip). Sub-slice 8 creates new `MarriageProposalComponent` entities; sub-slice 9 creates new `MarriageComponent` entities and mutates existing `MarriageProposalComponent.Status` pending->accepted. Does not modify existing dynasty system code.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs` -- read `GestationInWorldDays` constant (currently 280 in-world days after the Codex dynasty marriage parity slice) so expected child timestamps stay synchronized with the canonical gestation window.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs` -- read `ExpirationInWorldDays` constant (currently 90 in-world days after the Codex dynasty marriage parity slice) so expiration timestamps stay synchronized with the canonical expiration window.
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- read `DynastyMemberComponent` fields and `DynastyMemberRef` buffer for candidate selection (sub-slice 8) and for `MarriageAuthorityEvaluator` head-direct/heir-regency/envoy-regency resolution (sub-slice 12).
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read `PopulationComponent.Total` for population-deficit signal (sub-slice 10).
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- read `FaithStateComponent.SelectedFaith` and `DoctrinePath` for simplified faith-compatibility tier (sub-slice 10).
  - `unity/Assets/_Bloodlines/Code/Combat/HostilityComponent.cs` -- read `HostilityComponent` buffer for isHostile signal (sub-slice 10); sub-slice 11 mutates the buffer by removing entries both ways on marriage accept.
  - `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoring.cs` -- call `ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, +2f)` on both factions' `ConvictionComponent` when a marriage is accepted (sub-slice 11). The helper refreshes score and band in place.
  - `unity/Assets/_Bloodlines/Code/Time/DeclareInWorldTimeRequest.cs` -- push a 30-day request onto the DualClock singleton buffer when a marriage is accepted (sub-slice 11). `DualClockDeclarationSystem` drains the buffer.
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- read `InWorldDays` for proposal and marriage timestamping.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs` -- write to `LesserHouseElement` buffer on the AI faction (sub-slice 13). The buffer element struct itself is not modified; new entries seeded by `AILesserHousePromotionSystem` carry FounderMemberId for cross-reference deduplication and `LastDriftAppliedInWorldDays` so the existing `LesserHouseLoyaltyDriftSystem` (tier2-batch lane, retired) picks them up next in-world day.
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read + write `ResourceStockpileComponent.Gold` and `Influence` (sub-slice 14) to deduct the non-aggression-pact cost. Field mutation only; no schema change.
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-1.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-2-pressure.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-3-governance.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-ai-strategic-layer-sub-slice-4-worker-gather.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-5-siege-staging.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-6-dynasty-covert-ops.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-7-build-order.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-8-command-dispatch.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-8-marriage-proposal-execution.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-9-marriage-inbox-accept.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-10-marriage-strategic-profile.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-11-marriage-accept-effects.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-12-marriage-acceptance-terms.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-13-lesser-house-promotion.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-14-pact-proposal-execution.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-15-pact-break-expiration.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-16-narrative-message-bridge.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-17-narrative-back-wire.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-18-dynasty-operations-foundation.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-1-captive-state-and-missionary-execution.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-bundle-2-holy-war-and-divine-right-execution.md`
- Browser Reference:
  - Sub-slice 1: `src/game/core/ai.js` `pickTerritoryTarget` (~747), `pickScoutHarassTarget` (~412), `getWorldPressureRaidTarget` (~817)
  - Sub-slice 2: `src/game/core/ai.js` timer clamp/floor block lines 1127-1241
  - Sub-slice 3: `src/game/core/ai.js` governance/event-context timer clamp block lines 1129-1215
  - Sub-slice 4: `src/game/core/ai.js` idle worker dispatch loop lines 1243-1251, getEnemyGatherPriorities (885-922), chooseGatherNode (924-933)
  - Sub-slice 5: `src/game/core/ai.js` attackTimer<=0 siege staging decision tree ~lines 1825-2090, areSiegeLinesFormed (947), getSiegeStagePoint (935), isReliefArmyApproaching (727)
  - Sub-slice 6: `src/game/core/ai.js` updateEnemyAi dynasty/covert ops dispatch block ~lines 2419-2678 (assassination, missionary, holy war, divine right, captive recovery, marriage proposal/inbox, non-aggression pact, lesser-house promotion)
  - Sub-slice 7: `src/game/core/ai.js` updateEnemyAi buildTimer<=0 block ~lines 1377-1573 (13-step priority chain: barracks, wayshrine, quarry, iron mine, siege workshop, covenant hall, grand sanctuary, apex covenant, supply camp, stable, dwelling, farm, well); timer reset at 4s/6s depending on playerKeepFortified
  - Sub-slice 8 (Codex command dispatch): `src/game/core/ai.js` idle worker `issueGatherCommand` dispatch (~1243-1260), territory expansion command dispatch (~1575-1600)
  - Sub-slice 8: `src/game/core/ai.js` `tryAiMarriageProposal` (~2897-2944) plus updateEnemyAi dispatch hook (~2616-2624); simulation-side sink `proposeMarriage` (~7340); expiration delegated to `MarriageProposalExpirationSystem`, now corrected to the browser 90 in-world days
  - Sub-slice 9: `src/game/core/ai.js` `tryAiAcceptIncomingMarriage` (~2880-2895) plus updateEnemyAi dispatch hook (~2632-2636); simulation-side sink `acceptMarriage` (~7388-7469); gestation delegated to `MarriageGestationSystem`, now corrected to the browser 280 in-world days while still spawning only from `IsPrimary == true` records
  - Sub-slice 10: `src/game/core/ai.js` `getAiMarriageStrategicProfile` (~2803-2839); simplified port of `simulation.js getMarriageFaithCompatibilityProfile` (~596-730) using SelectedFaith+DoctrinePath equality rather than covenantName grouping (Unity has no covenant-name covariance yet); populates `AICovertOpsComponent.MarriageProposalGateMet` and `MarriageInboxAcceptGate` so sub-slices 6/8/9 gate on browser-accurate 4-signal strategic profile
  - Sub-slice 11: `src/game/core/simulation.js` `acceptMarriage` (~7388-7469) post-record effects block; ports legitimacy +2 clamped to 100 both sides, hostility drop both ways, oathkeeping conviction +2 both sides via `ConvictionScoring.ApplyEvent`, and 30-day `DeclareInWorldTimeRequest` jump via the existing DualClock request buffer; uses new `MarriageAcceptEffectsPendingTag` attached in sub-slice 9 at primary marriage creation for one-shot application
  - Sub-slice 12: `src/game/core/simulation.js` `getMarriageAcceptanceTerms` (~6327), `applyMarriageGovernanceLegitimacyCost` (~6232), `getMarriageAuthorityProfile` (~6134), and `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (~6091); `acceptMarriage` cost-before-bonus order at simulation.js:7449 (cost) vs simulation.js:7458 (legitimacy +2); ports head-direct (cost 0) / heir-regency (cost 1) / envoy-regency (cost 2) and the no-authority rejection; Stewardship -cost conviction event via the same `ConvictionScoring.ApplyEvent` helper used by oathkeeping
  - Sub-slice 13: `src/game/core/ai.js` `tryAiPromoteLesserHouse` (~2784-2801) plus updateEnemyAi dispatch hook (~2674-2677); simulation-side sink `promoteMemberToLesserHouse` (~7184-7258) ported at the mechanical level Unity tracks; constants block at simulation.js (~6444-6457): LESSER_HOUSE_RENOWN_THRESHOLD = 30, LESSER_HOUSE_MIN_PROMOTIONS = 1 (deferred), LESSER_HOUSE_LEGITIMACY_BONUS = 3, LESSER_HOUSE_INITIAL_LOYALTY = 75, LESSER_HOUSE_QUALIFYING_PATHS = {Governance, MilitaryCommand, CovertOperations}; `memberIsLesserHouseCandidate` (~6469-6479) gates ported (renown, role, status, path; foundedLesserHouseId replaced by cross-reference of LesserHouseElement.FounderMemberId since DynastyMemberComponent has no foundedLesserHouseId field); strategic gates: legitimacy &lt; 90, lesser-house buffer count &lt; 3
  - Sub-slice 14: `src/game/core/ai.js` pact dispatch block (~2643-2666); simulation-side sinks `getNonAggressionPactTerms` (~5150-5183) and `proposeNonAggressionPact` (~5185-5222); constants at simulation.js ~5126-5128 (NON_AGGRESSION_PACT_INFLUENCE_COST = 50, NON_AGGRESSION_PACT_GOLD_COST = 80, NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180); gates: both FactionKind.Kingdom, source != target, source hostile to target, no existing PactComponent between them, source ResourceStockpileComponent.Influence &gt;= 50 + Gold &gt;= 80; on success deducts cost, removes HostilityComponent buffer entries both ways, creates one PactComponent entity; holy-war gate from getNonAggressionPactTerms deferred until a Unity holy-war system exists; browser creates symmetric per-faction `faction.diplomacy.nonAggressionPacts` records, Unity collapses to a single entity per pact because both sides carry identical fields
  - Sub-slices pending: narrative message push (no AI->UI message component yet); per-member FoundedLesserHouseId on DynastyMemberComponent (cross-reference workaround in place); promotion-history gate; marital-anchor and cadet world-pressure profiles on lesser houses; holy-war pact gate; pact expiration / break system (browser breakNonAggressionPact ~5224 plus early-break legitimacy penalty); and dispatch-execution sub-slices for assassination, missionary, holy war, divine right, captive recovery, captive ransom CovertOpKind values
  - Sub-slice 15: `src/game/core/simulation.js` `breakNonAggressionPact` (~5224-5257); NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8 (~5129); ports the explicit-break semantic (no auto-expiration; minimumExpiresAtInWorldDays marks only the early-break threshold for messaging, not an auto-dissolve). Unity introduces PactBreakRequestComponent as the producer surface; PactBreakSystem consumes the request, marks the PactComponent broken, re-establishes mutual hostility idempotently, applies legitimacy -8 clamped [0, 100] and Oathkeeping -2 via ConvictionScoring.ApplyEvent; browser's direct conviction.score -= 2 maps to Oathkeeping in Unity because Score is derived from bucket values (Score = Stewardship + Oathkeeping - Ruthlessness - Desecration) and breaking an oath-like pact is semantically a Oathkeeping penalty; penalty is unconditional regardless of early-break status (browser earlyBreak flag affects messaging only)
  - Sub-slice 16: `src/game/core/simulation.js` `pushMessage` call sites; narrative message bridge carries the browser `{ text, tone, ttl }` shape; Unity adds a CreatedAtInWorldDays stamp so consumers can render chronologically. Browser uses `state.messages.unshift(...)`; Unity appends to the buffer (ordering difference is consumer-side, buffer preserves append order). PactBreakSystem (sub-slice 15) wired as first consumer to prove the bridge end-to-end.
  - Sub-slice 17: `src/game/core/simulation.js` pushMessage call sites at `acceptMarriage` (~7463), `promoteMemberToLesserHouse` (~7251-7255), and `proposeNonAggressionPact` (~5216-5220). Back-wires NarrativeMessageBridge.Push into AIMarriageAcceptEffectsSystem (marriage accept ceremonial line with authority-mode suffix), AILesserHousePromotionSystem (founding line), and AIPactProposalExecutionSystem (pact entry line). Tone routing follows the browser source/target rule: player-source marriage accept and pact proposal promote to Good, player-faction lesser-house promotes to Good, else Info. AIPactProposalExecutionSystem hardcodes target to "player" per the browser ai.js dispatch so the Good branch is defensive for future extension. Marriage member titles are resolved via the DynastyMemberRef buffer with a direct FixedString64Bytes memberId fallback when no roster is seeded. FactionId substitutes for display name until a Unity display-name component lands.
  - Sub-slice 18: `src/game/core/simulation.js` `DYNASTY_OPERATION_ACTIVE_LIMIT = 6` (line 17) plus the seven dispatch sites that gate on `(operations.active ?? []).length >= DYNASTY_OPERATION_ACTIVE_LIMIT` (startMissionaryOperation ~10523, startHolyWarDeclaration ~10565, startCounterIntelligenceOperation ~10837, startEspionageOperation ~10876, startAssassinationOperation ~10912, startSabotageOperation ~10952, captive rescue/ransom dispatch sites referenced in ai.js ~2566-2608). Unity's foundation exposes the cap and the BeginOperation producer helper without porting the per-kind resolution fields (resolveAt, operatorId, successScore, intelligenceReportId, etc.); each dispatch slice that follows attaches its own per-kind component struct to the entity created here. Browser trims the active array to the cap via `operations.active.slice(0, DYNASTY_OPERATION_ACTIVE_LIMIT)` after unshift; Unity does not silently trim because an entity-per-operation model cannot drop entries without orphaning per-kind data. The gate is strict at the call site: callers must check HasCapacity before BeginOperation. DynastyOperationKind enum covers Missionary, HolyWar, DivineRight, CaptiveRescue, CaptiveRansom, LesserHousePromotion, and None; LesserHousePromotion is reserved for enum completeness only and is not currently gated by the cap (browser's promoteMemberToLesserHouse does not route through getDynastyOperationsState). Active=false entities are retained for audit and excluded from HasCapacity counts.
  - Sub-slice 19 (Bundle 1): `src/game/core/simulation.js` captive member state ports the per-faction `faction.dynasty.captives` array and the captive record shape created by `transferMemberToCaptor` (~4422-4453). Browser fields ported: id (Unity uses entity buffer index instead), memberId, sourceFactionId (Unity OriginFactionId), title (Unity MemberTitle), capturedAt (Unity CapturedAtInWorldDays via DualClock). Browser fields deferred: roleId, pathId, renown, age, sourceFactionName, reason (these will land alongside captive rescue/ransom dispatch when those slices need them). Unity adds RansomCost (default 0; sub-slice 24 ransom dispatch sets the canonical value) and CapturedMemberStatus enum (Held / RansomOffered / Released / Executed) so audit-retained entries can stay on the buffer without being mistaken for currently-held captives. Browser CAPTIVE_LEDGER_LIMIT = 16 (simulation.js:14) trim is not enforced; the buffer accepts new entries unconditionally and downstream consumers read the full buffer.
  - Sub-slice 20 (Bundle 1): `src/game/core/simulation.js` `startMissionaryOperation` (~10523-10563) plus `getMissionaryTerms` (~10362-10422) and the constants block (`MISSIONARY_COST = { influence: 14 }` ~9766, `MISSIONARY_DURATION_SECONDS = 32` ~9770, `MISSIONARY_INTENSITY_COST = 12` ~9773). Browser dispatch hook ported: `ai.js` missionary block ~2469-2496 (hardcoded source "enemy" / target "player"). Browser tone routing simulation.js:10560-10561 ported to NarrativeMessageTone (Warn when target is player, Info otherwise). First production caller of DynastyOperationLimits.HasCapacity / BeginOperation. AIMissionaryExecutionSystem mirrors the one-shot dispatch pattern shared with sub-slices 8/9/12/13/14 (consume LastFiredOp == CovertOpKind.Missionary, attempt dispatch, clear to None unconditionally). Per-kind component DynastyOperationMissionaryComponent attached to the entity created by BeginOperation carries ResolveAtInWorldDays (current + 32f using browser numeric value reinterpreted on the in-world timeline rather than translated through DualClock.DaysPerRealSecond), OperatorMemberId, OperatorTitle, SourceFaithId (CovenantId enum mapped to canonical string token), ExposureGain / IntensityErosion / LoyaltyPressure / SuccessScore / ProjectedChance / IntensityCost (computed from the simplified browser parity formula: offenseScore = operatorRenown + sourceIntensity*0.65 + (8 if dark, 4 if light); defenseScore = targetIntensity * 0.55 with the ward-profile bonus deferred until a Unity faith-ward readout lands; per-kind formulas for exposure/intensity/loyalty mirror simulation.js:10399-10405), and DynastyOperationEscrowCost struct mirroring ResourceStockpileComponent fields with only Influence set. Per-kind resolution system intentionally deferred: created entities sit Active=true until a future slice walks expired ResolveAtInWorldDays entries, applies effects to the target faction, and flips Active=false. Unity-side defensive gates beyond browser parity: target must have at least one ControlPointComponent owned by player (browser permits the call with default ward when the target has no primary keep, but a missionary in a court with zero territories produces no meaningful effect); operator role gate substitutes Spymaster for the canonical Sorcerer role until DynastyRole gains a Sorcerer entry.
  - Sub-slice 21 (Bundle 2): `src/game/core/simulation.js` `startHolyWarDeclaration` (~10565-10602) plus `getHolyWarDeclarationTerms` (~10424-10471) and the constants block (`HOLY_WAR_COST = { influence: 24 }` ~9767, `HOLY_WAR_DECLARATION_DURATION_SECONDS = 18` ~9771, `HOLY_WAR_INTENSITY_COST = 18` ~9774, `HOLY_WAR_DURATION_SECONDS = 180` ~9775). Browser dispatch hook ported: `ai.js` holy war block ~2512-2551. Browser tone routing simulation.js:10599 ported to NarrativeMessageTone (Warn when source or target is player, Info otherwise; browser uses warn for both because holy war is an escalation either way). Second production caller of DynastyOperationLimits. Per-kind component DynastyOperationHolyWarComponent attached to the BeginOperation entity carries ResolveAtInWorldDays (current + 18f for declaration window), WarExpiresAtInWorldDays (current + 180f for full holy-war duration), OperatorMemberId/OperatorTitle, IntensityPulse / LoyaltyPulse from doctrine bias (browser simulation.js:10468-10469: 1.2 dark / 0.9 light intensity, 1.8 dark / 1.2 light loyalty), CompatibilityLabel from a simplified Unity-side derivation (identical-faith-different-doctrine -> "fractured", different-faith -> "discordant"; identical (faith, doctrine) is gated out as harmonious before this point), IntensityCost (18f), and DynastyOperationEscrowCost with Influence (24f). Compatibility gate uses simplified identical-(faith, doctrine) equality instead of browser's getMarriageFaithCompatibilityProfile tier ladder which depends on covenant-name covariance not yet ported (Unity gate strictly looser than browser; future tightening lands when the covenant compatibility surface ports). Reuses sub-slice 20's faith operator role gate (IdeologicalLeader / Spymaster / HeadOfBloodline / Diplomat with non-Fallen, non-Captured status; Spymaster substitutes for the canonical Sorcerer role pending DynastyRole.Sorcerer addition). Per-kind resolution intentionally deferred: future slice materializes the holy-war entry on the source faction's faith state via browser createHolyWarEntry at simulation.js:10505, applies intensityPulse / loyaltyPulse to the target faction at war-tick boundaries, and flips Active=false at WarExpiresAtInWorldDays.
  - Sub-slice 22 (Bundle 2): `src/game/core/simulation.js` `startDivineRightDeclaration` (~10784-10835) plus `getDivineRightDeclarationTerms` (~10604-10653) and the constants block (`DIVINE_RIGHT_DECLARATION_DURATION_SECONDS = 180` ~9779, `DIVINE_RIGHT_INTENSITY_THRESHOLD = 80` ~9782). Browser dispatch hook ported: `ai.js` divine right block ~2553-2564 (hardcoded source "enemy" with no target argument; divine right is a unilateral source-side declaration). Browser tone routing simulation.js:10832 ported to NarrativeMessageTone (always Warn). Third production caller of DynastyOperationLimits. **Notable Unity-side departure from browser parity**: divine right routes through DynastyOperationLimits.BeginOperation with DynastyOperationKind.DivineRight even though browser does NOT gate divine right on DYNASTY_OPERATION_ACTIVE_LIMIT (startDivineRightDeclaration writes directly to faction.faith.divineRightDeclaration and never calls getDynastyOperationsState). This Unity-side routing is for surface consistency with all other dispatch consumers and lets future intel-report and resolution-system queries use one shape. Browser's per-faction one-active-at-a-time semantic is preserved by an explicit "no existing active divine right operation for this faction" gate before the capacity check. Per-kind component DynastyOperationDivineRightComponent carries ResolveAtInWorldDays (current + 180f), SourceFaithId mapped from CovenantId enum, DoctrinePath captured at declaration, RecognitionShare/RecognitionSharePct placeholders defaulting to 0 (browser global recognition share calculator not yet ported), and ActiveApexStructureId/ActiveApexStructureName placeholders defaulting to empty (browser apex structure surface not yet ported). The component carries no EscrowCost or IntensityCost because browser divine right does NOT deduct intensity or charge an escrow cost (the cost is the covenant-test prerequisite that gates getDivineRightDeclarationTerms). Gates intentionally deferred to future slices that port their respective surfaces: covenant-test-passed gate (browser ensureFaithCovenantTestCompletionFromLegacyState + faithState.covenantTestPassed at simulation.js:10606/10614), cooldown gate (profile.cooldownRemaining at :10626), stage-ready gate (profile.stageReady Final Convergence threshold at :10629), active-apex-structure gate (profile.activeApexStructureReady at :10635), recognition-share gate (profile.recognitionReady at :10638), faction-kind == kingdom gate (at :10607; defensible to add via FactionKindComponent in a hardening pass). Effects intentionally deferred to a future resolution slice: mutual hostility application against all non-same-faith kingdoms (browser ensureMutualHostility at simulation.js:10819), AI timer cap propagation to candidate factions (attackTimer / territoryTimer / raidTimer / missionaryTimer / holyWarTimer caps at :10822-10826), conviction event recording (browser recordConvictionEvent at :10806; oathkeeping for light, desecration for dark, +3 either way), and resolution at ResolveAtInWorldDays (apex faith claim on success vs failDivineRightDeclaration cooldown + legitimacy penalty on failure at :10691).
- Current Branch In Flight: `claude/unity-ai-captive-rescue-and-ransom-execution`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-20-unity-ai-strategic-layer-bundle-3-captive-rescue-and-ransom-execution.md`

### Lane: victory-conditions

- Status: retired (all 3 conditions merged to master 2026-04-18 via codex/unity-fortification-siege)
- Branch Prefix: `codex/unity-fortification-siege` (merged)
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Victory/**`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryConditionsSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityVictoryConditionsSmokeValidation.ps1`
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-18-unity-victory-conditions.md`
- Browser Reference:
  - `src/game/core/simulation.js` command hall fall (~7821), territorial governance recognition (~1661), divine right declaration completion (~10738)
  - Constants: TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD=90, TERRITORIAL_GOVERNANCE_VICTORY_SECONDS=120, DIVINE_RIGHT_INTENSITY_THRESHOLD=80, faith Level 5 required
  - VictoryConditionIds: CommandHallFall (1), TerritorialGovernance (2), DivinRight (3)
- Current Branch In Flight: `codex/unity-fortification-siege`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-18-unity-victory-conditions.md`

### Lane: tier2-batch-dynasty-systems

- Status: retired (all 5 systems merged to master 2026-04-18 via codex/unity-fortification-siege)
- Branch Prefix: `codex/unity-fortification-siege` (merged)
- Owner Agent: claude-code
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/RenownAwardRequestComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/RenownAwardSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MinorHouseLevySystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTier2BatchSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityTier2BatchSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- `MinorHouse = 3` added to `FactionKind` enum
  - `unity/Assembly-CSharp.csproj` -- all 7 Dynasties files registered
  - `unity/Assembly-CSharp-Editor.csproj` -- `BloodlinesTier2BatchSmokeValidation.cs` registered
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-18-unity-tier2-batch-dynasty-systems.md`
- Browser Reference:
  - `src/game/core/simulation.js` `tickLesserHouseLoyaltyDrift` (~6631), `spawnDefectedMinorFaction` (~6851), `tickMarriageProposalExpiration` (~7274), `proposeMarriage` (~7340), `acceptMarriage` (~7388), `tickMarriageGestation` (~7496), `tickMinorHouseTerritorialLevies` (~7060)
  - Base daily loyalty delta = -0.5 (BaseDailyDelta in LesserHouseLoyaltyDriftSystem)
  - MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 30, MARRIAGE_GESTATION_IN_WORLD_DAYS = 60
- Current Branch In Flight: `codex/unity-fortification-siege`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-18-unity-tier2-batch-dynasty-systems.md`

### Lane: fortification-siege-imminent-engagement

- Status: paused (the fortification queue is closed cleanly through sub-slice 13; any future sub-slice 14 requires fresh operator direction)
- Branch Prefix: `codex/unity-fortification-repair-narrative` (sub-slice 13), `codex/unity-fortification-sealing-worker-locality` (sub-slice 12), `codex/unity-fortification-sealing-cost-tier-scaling` (sub-slice 11), `codex/unity-fortification-breach-depth-telemetry` (sub-slice 10), `codex/unity-fortification-destroyed-counter-recovery` (sub-slice 9), `codex/unity-fortification-breach-sealing-recovery` (sub-slice 8), `codex/unity-fortification-breach-legibility-readout` (sub-slice 7), `codex/unity-fortification-breach-assault-pressure` (sub-slice 6), `codex/unity-fortification-wall-segment-destruction` (sub-slice 5); prior `codex/unity-fortification-siege*` slices 1-4 also landed; future follow-ups should continue on fresh `codex/unity-fortification-*` branches
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Fortification/**`
  - `unity/Assets/_Bloodlines/Code/Siege/**`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/BreachSealingProgressComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/DestroyedCounterRecoveryProgressComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/ImminentEngagementComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.BreachReadout.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSupplyInterdictionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesWallSegmentDestructionSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachAssaultPressureSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachLegibilityReadoutSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingRecoverySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDestroyedCounterRecoverySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingTierScalingSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBreachSealingWorkerLocalitySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationRepairNarrativeSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityImminentEngagementSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSupplyInterdictionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityWallSegmentDestructionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBreachAssaultPressureSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBreachLegibilityReadoutSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBreachSealingRecoverySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityDestroyedCounterRecoverySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBreachSealingTierScalingSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBreachSealingWorkerLocalitySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityFortificationRepairNarrativeSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` -- additive wrapper update preserved fortification smoke ownership while keeping the existing validation surface intact for the rebased imminent-engagement lane
  - `unity/Assembly-CSharp.csproj` -- verified Claude Bundle 3 runtime entries were present on disk and locally registered `DestroyedCounterRecoveryProgressComponent.cs` plus `DestroyedCounterRecoverySystem.cs` for the governed `dotnet build` gate; Unity-generated gitignored file, not part of the commit
  - `unity/Assembly-CSharp-Editor.csproj` -- verified Claude Bundle 3 editor entries were present on disk and locally registered `BloodlinesDestroyedCounterRecoverySmokeValidation.cs` for the governed `dotnet build` gate; Unity-generated gitignored file, not part of the commit
  - `unity/Assembly-CSharp.csproj` -- re-verified local registration for Claude Bundles 1-4 runtime files plus Codex fortification sub-slices 7-13 runtime files before the governed `dotnet build` gate; Unity-generated gitignored file, not part of the commit
  - `unity/Assembly-CSharp-Editor.csproj` -- re-verified local registration for Claude Bundles 1-4 editor validators plus Codex fortification sub-slices 7-13 editor validators, including `BloodlinesFortificationRepairNarrativeSmokeValidation.cs`, before the governed `dotnet build` gate; Unity-generated gitignored file, not part of the commit
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-fortification-siege-fortification-tier-and-reserves.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-imminent-engagement-warnings.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-camp-supply-interdiction.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-wall-segment-destruction-resolution.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-assault-pressure.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-legibility-readout.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-breach-sealing-recovery.md`
  - `docs/unity/session-handoffs/2026-04-20-unity-fortification-siege-destroyed-counter-recovery.md`
  - `docs/unity/session-handoffs/2026-04-20-unity-fortification-siege-breach-depth-telemetry.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-fortification-siege-sealing-cost-tier-scaling.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-fortification-siege-sealing-worker-locality.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-fortification-repair-narrative.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-fortification-siege-session-wrap-10-through-13.md`
- Current Branch In Flight: none (queued fortification-siege follow-ups through sub-slice 13 are landed; any new fortification work requires a fresh claim)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-21-unity-fortification-repair-narrative.md`

### Lane: dynasty-house-parity

- Status: paused (minor-house levy completion follow-up landed on canonical `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-dynasty-*`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/LesserHouseLoyaltyDriftSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/MinorHouseLevySystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMarriageParitySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesLesserHouseLoyaltyParitySmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMinorHouseLevyParitySmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityMarriageParitySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityLesserHouseLoyaltyParitySmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityMinorHouseLevyParitySmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs` -- additive `TryDebugGetMinorHouseLevyState(...)` readout only
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-18-unity-tier2-batch-dynasty-systems.md`
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-marriage-parity.md`
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-lesser-house-loyalty-parity.md`
  - `docs/unity/session-handoffs/2026-04-20-unity-dynasty-minor-house-levy-parity.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-dynasty-minor-house-levy-completion.md`
- Browser Reference:
  - `src/game/core/simulation.js` `MARRIAGE_GESTATION_IN_WORLD_DAYS` (6088), `MARRIAGE_DISSOLUTION_LEGITIMACY_LOSS` (6089), `MARRIAGE_DISSOLUTION_OATHKEEPING_GAIN` (6090), `dissolveMarriageFromDeath` (6382), `tickLesserHouseLoyaltyDrift` (~6631), `spawnDefectedMinorTerritoryClaim` (~6801), `spawnDefectedMinorFaction` (~6851), `getMinorHouseClaim` (~6982), `ensureMinorHouseLevyState` (~6996), `getMinorHouseRetinueCap` (~7011), `pickMinorHouseLevyProfile` (~7024), `spawnMinorHouseRetinueUnit` (~7034), `tickMinorHouseTerritorialLevies` (~7060), `getMinorHousePressureOpportunityProfile` (~13913), `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS` (7272), `tickMarriageProposalExpiration` (7274), `tickMarriageDissolutionFromDeath` (7471), `tickMarriageGestation` (7496)
  - `tests/runtime-bridge.mjs` mixed-bloodline and death-dissolution assertions (3180-3229, 3270-3297)
- Current Branch In Flight: none (validated follow-up landed onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-dynasty-minor-house-levy-completion.md`
- Last Slice State:
  - the dedicated levy parity validator now proves the low-loyalty unsettled claim gate through both runtime state and the new debug surface readout
  - breakaway minor-house levy state now explicitly initializes `LastLevyUnitId`, and the dedicated wrapper now honors explicit PASS markers plus delayed Unity log flushes

### Lane: scout-raids-logistics-interdiction

- Status: active (foundation landed on master; no follow-up branch currently in flight)
- Branch Prefix: `codex/unity-scout-raids-logistics-interdiction`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Raids/**`
  - `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesScoutRaidAndInterdictionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityScoutRaidAndInterdictionSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- additive bootstrap spawn wiring for `BuildingRaidStateComponent` on raidable structures
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` -- additive debug construction spawn wiring for `BuildingRaidStateComponent`
- Coordinated Same-Agent Edits On Fortification-Owned Paths:
  - `unity/Assets/_Bloodlines/Code/Siege/FieldWaterSupportScanSystem.cs` -- active raid state now suppresses well and supply-camp support sources until expiry
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeSupportRefreshSystem.cs` -- active raid state now suppresses raided supply camps as linked logistics anchors
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-20-unity-scout-raids-and-logistics-interdiction.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-scout-raids-logistics-interdiction-rebase-validation.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-scout-raids-logistics-interdiction-landing.md`
- Browser Reference:
  - `src/game/core/simulation.js` `SCOUT_RAID_TARGET_RANGE` (35), `SCOUT_RAID_RETREAT_DISTANCE` (36), `SCOUT_RAID_LOYALTY_RADIUS` (37), `isBuildingUnderScoutRaid` (2046), `getRaidRetreatCommand` (2349), `executeScoutRaid` (2362), `executeScoutLogisticsInterdiction` (2515)
- Current Branch In Flight: none (merged into master via `dda7c25e`; future scout follow-ups should start from a fresh branch)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-21-unity-scout-raids-logistics-interdiction-landing.md`

### Lane: player-marriage-diplomacy

- Status: active
- Branch Prefix: `codex/unity-player-marriage-*`, `codex/unity-player-holy-war-divine-right`, `codex/unity-player-missionary-dispatch`, `codex/unity-player-pact-proposal`, `codex/unity-player-captive-rescue`, `codex/unity-player-captive-ransom-*`, `codex/unity-player-captive-ransom-trickle`, `codex/unity-player-succession-influence`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageProposalSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageAcceptanceSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageDissolutionSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMissionaryDispatchSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerPactSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRescueSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRansomSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCaptiveRansomTrickleSmokeValidation.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerSuccessionInfluenceSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityPlayerMarriageProposalSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerMarriageAcceptanceSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerMarriageDissolutionSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerHolyWarDivineRightSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerMissionaryDispatchSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerPactSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerCaptiveRescueSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerCaptiveRansomSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerCaptiveRansomTrickleSmokeValidation.ps1`
- `scripts/Invoke-BloodlinesUnityPlayerSuccessionInfluenceSmokeValidation.ps1`
- Coordinated Narrow Edit On Retired Dynasty-Owned Path:
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastySuccessionSystem.cs` -- additive preferred-heir override and preference-consumption seam only; no broader dynasty-core rewrite
- Shared-File Narrow Edits Applied:
  - `unity/Assembly-CSharp.csproj` -- additive compile includes for `PlayerSuccessionPreferenceRequestComponent.cs`, `SuccessionPreferenceComponent.cs`, and `SuccessionPreferenceResolutionSystem.cs`
  - `unity/Assembly-CSharp-Editor.csproj` -- additive compile include for `BloodlinesPlayerSuccessionInfluenceSmokeValidation.cs`
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs` -- reuse `MarriageProposalComponent`, `MarriageComponent`, and `MarriageProposalStatus` without widening the retired dynasty parity lane
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs` -- read `ExpirationInWorldDays` so proposal expiry stays aligned with the canonical 90-day window
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs` -- read `GestationInWorldDays` so accepted marriages schedule child timing on the same 280-day cadence as the canonical dynasty lane
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs` -- reuse the already-landed death-driven dissolution runtime instead of cloning it under `PlayerDiplomacy/`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastySuccessionSystem.cs` -- validate ruler-death succession compatibility in the dedicated player marriage dissolution smoke
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyAgingSystem.cs` -- include the required predecessor for succession ordering in the dedicated validation world
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- read member roster, role, and status when resolving proposal candidates and governance authority
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- read committed covenant to enforce the browser polygamy gate
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationComponent.cs` -- read-only dependency for the AI-owned dynasty-operation entity shape consumed by player holy war and divine right dispatch
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationHolyWarComponent.cs` -- read-only holy-war operation payload shape reused by the player-faith declaration slice
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationDivineRightComponent.cs` -- read-only divine-right operation payload shape reused by the player-faith declaration slice
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationMissionaryComponent.cs` -- read-only missionary operation payload shape reused by the player missionary dispatch slice
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationLimits.cs` -- read-only active-operation cap helper reused by the player-faith declaration slice
  - `unity/Assets/_Bloodlines/Code/AI/PactComponent.cs` -- read-only pact entity shape reused by the player pact proposal/break slice
  - `unity/Assets/_Bloodlines/Code/AI/PactBreakSystem.cs` -- read-only reference for live Unity pact-break penalties and narrative parity without widening the AI lane
  - `unity/Assets/_Bloodlines/Code/AI/CapturedMemberElement.cs` -- read-only captive holding buffer shape reused by the player captive rescue slice
  - `unity/Assets/_Bloodlines/Code/AI/CapturedMemberHelpers.cs` -- read-only validation helper seam reused by the dedicated player captive rescue smoke
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationCaptiveRescueComponent.cs` -- read-only rescue operation payload shape reused by the player captive rescue slice
  - `unity/Assets/_Bloodlines/Code/AI/AICaptiveRescueExecutionSystem.cs` -- read-only rescue constants and operator-priority seam reused for browser-parity dispatch terms without widening the AI lane
  - `unity/Assets/_Bloodlines/Code/Narrative/NarrativeMessageBridge.cs` -- shared message bridge used for player declaration messaging without widening the AI-owned lane
  - `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoring.cs` -- apply proposal and acceptance-side stewardship/oathkeeping conviction events
  - `unity/Assets/_Bloodlines/Code/Combat/HostilityComponent.cs` -- remove reciprocal hostility on successful acceptance
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- read `InWorldDays` for proposal timestamps and acceptance timing
  - `unity/Assets/_Bloodlines/Code/Time/DeclareInWorldTimeRequest.cs` -- enqueue the browser-matching 30-day declaration jump after accept
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-proposal.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-proposal-landing.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-acceptance.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-marriage-dissolution.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-holy-war-divine-right.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-missionary-dispatch.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-pact-proposal.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-captive-rescue.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-captive-ransom.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-captive-ransom-trickle.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-succession-influence.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-succession-influence-landing.md`
- Browser Reference:
  - `src/game/core/simulation.js` `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (6091), `getMarriageAuthorityProfile` (6134), `getMarriageEnvoyProfile` (6192), `buildMarriageGovernanceStatus` (6217), `applyMarriageGovernanceLegitimacyCost` (6232), `getMarriageProposalContext` (6247), `getMarriageProposalTerms` (6296), `getMarriageAcceptanceTerms` (6327), `memberHasActiveMarriage` (7260), `proposeMarriage` (7340), `acceptMarriage` (7388), `tickMarriageDissolutionFromDeath` (7471), `tickMarriageGestation` (7496)
  - `tests/runtime-bridge.mjs` marriage proposal and acceptance assertions (2072-2113, 2240-2308), death-driven dissolution assertions (3234-3298)
  - `src/game/core/simulation.js` `getMissionaryTerms` (~10362-10421), `startMissionaryOperation` (~10523-10563)
  - `src/game/core/simulation.js` `getHolyWarDeclarationTerms` (~10424-10471), `startHolyWarDeclaration` (~10565-10602), `getDivineRightDeclarationTerms` (~10604-10653), `startDivineRightDeclaration` (~10784-10835)
  - `src/game/core/simulation.js` `NON_AGGRESSION_PACT_INFLUENCE_COST` (5126), `NON_AGGRESSION_PACT_GOLD_COST` (5127), `NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS` (5128), `NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST` (5129), `getNonAggressionPactTerms` (5150-5183), `proposeNonAggressionPact` (5185-5222), `breakNonAggressionPact` (5224-5257)
  - `src/game/core/simulation.js` `getCapturedMemberRescueTerms` (~11153-11234), `startRescueOperation` (~11236-11341)
  - `src/game/core/simulation.js` `getCapturedMemberRansomTerms` (~11343-11431), `startCaptiveRansomOperation` (~11433-11532), `updateCaptiveRansomTrickle` (~4885-4903), `CAPTIVE_INFLUENCE_TRICKLE`, `CAPTIVE_RENOWN_WEIGHT`
  - `src/game/core/simulation.js` `applySuccessionRipple` (~4550-4608) and `backfillHeir` (~4612-4649); no direct browser `PlayerSuccessionPreference` helper exists, so this slice uses the directive-canon seam to pre-select an eligible successor before the landed succession cascade runs
  - `src/game/core/ai.js` captive recovery contest / operator selection logic (~2550-2760)
- Current Branch In Flight: none (merged onto canonical `master` via `fcfbc39c` on 2026-04-23)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-player-succession-influence-landing.md`
- Last Slice State:
  - `SuccessionPreferenceComponent`, `PlayerSuccessionPreferenceRequestComponent`, and `SuccessionPreferenceResolutionSystem` now live on canonical `master`, so the player kingdom can pay 50 gold and 4 legitimacy to designate a preferred eligible heir for up to 365 in-world days before the existing succession cascade executes
  - `DynastySuccessionSystem` now consumes that live preferred-heir designation before falling back to default succession order and clears the preference once it is spent or invalid without widening the broader dynasty lane
  - `BloodlinesDebugCommandSurface.PlayerDiplomacy` exposes `TryDebugSetSuccessionPreference(...)` and `TryDebugGetSuccessionPreferenceState(...)`, and `BloodlinesPlayerSuccessionInfluenceSmokeValidation` plus `scripts/Invoke-BloodlinesUnityPlayerSuccessionInfluenceSmokeValidation.ps1` now prove designation cost deduction, valid override, and invalid-preference fallback on the merged line
  - the merged result reran the full governed 10-gate chain plus the dedicated player succession influence smoke green, and the detached landing worktree re-established a local `unity/Library` junction back to `D:\ProjectsHome\Bloodlines\unity\Library`
  - next Codex pickup should move to Priority 21 `siege-escalation-arc`

### Lane: player-covert-ops

- Status: active (resolution-effects follow-up landed on `master`; no branch currently in flight)
- Branch Prefix: `codex/unity-player-covert-ops-*`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCounterIntelligenceSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsResolutionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityPlayerCovertOpsSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerCounterIntelligenceSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPlayerCovertOpsResolutionSmokeValidation.ps1`
- Shared-File Narrow Edits Planned:
  - `unity/Assembly-CSharp.csproj` -- add compile includes for new `PlayerCovertOps/**` runtime files only if the local generated project file does not already pick them up
  - `unity/Assembly-CSharp-Editor.csproj` -- add compile includes for new player-covert-ops editor validators only if the local generated project file does not already pick them up
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/AI/DynastyOperationLimits.cs` -- reuse the canonical dynasty-operation active-cap constant/helper without widening Claude's AI-owned operation surfaces
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve source/target faction entities by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read and deduct `ResourceStockpileComponent` fields from the owning faction
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- resolve the player covert operator roster (spymaster, diplomat, merchant) and member availability
  - `unity/Assets/_Bloodlines/Code/Components/BuildingTypeComponent.cs` -- resolve sabotage subtype legality from fortification role, siege-logistics support, and building type id
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- require live sabotage targets and live assassination targets
  - `unity/Assets/_Bloodlines/Code/Components/PositionComponent.cs` -- resolve nearest hostile settlement context for sabotage and assassination telemetry
  - `unity/Assets/_Bloodlines/Code/Components/SettlementComponent.cs` -- derive fortification tier and keep-tier simplifications for sabotage and assassination contests without widening the fortification lane
  - `unity/Assets/_Bloodlines/Code/Components/CommanderComponent.cs` -- resolve commander-attached battlefield units for assassination death fallout without reopening the combat lane
  - `unity/Assets/_Bloodlines/Code/TerritoryGovernance/GovernorSeatAssignmentComponent.cs` -- clear dead-governor seat occupancy without editing the territorial-governance lane
  - `unity/Assets/_Bloodlines/Code/TerritoryGovernance/GovernorSpecializationComponent.cs` -- remove dead-governor specialization state without editing the territorial-governance lane
  - `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs` -- compensate queue time after production updates so sabotage can freeze output without mutating the shared runtime file
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastySuccessionSystem.cs` -- same-frame ruler death fallout relies on the existing succession seam to consume fallen heads of bloodline
  - `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisEvaluationSystem.cs` -- reuse the landed succession-crisis seam after successful ruler assassinations
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs` -- keep assassination-driven family fallout aligned with the existing dynasty death seam
  - `unity/Assets/_Bloodlines/Code/Systems/DeathResolutionSystem.cs` -- keep sabotage burn / commander death cleanup ordered with the existing death-resolution seam
  - `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs` -- apply well-poisoning water-strain fallout without widening the economy lane
  - `unity/Assets/_Bloodlines/Code/Combat/HostilityComponent.cs` -- reserved for later player assassination/sabotage follow-ups that trigger hostility without widening the AI lane
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-covert-ops-foundation.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-covert-ops-foundation-landing.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-assassination-sabotage.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-assassination-sabotage-landing.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-covert-ops-resolution-effects.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-player-covert-ops-resolution-effects-followup.md`
- Browser Reference:
  - `src/game/core/simulation.js` `DYNASTY_OPERATION_ACTIVE_LIMIT` (17), `getActiveDynastyOperationForTargetFaction` (4084), `getActiveIntelligenceReport` (4097), `tickDynastyIntelligenceReports` (4106), `getEspionageContest` (10187), `getEspionageTerms` (10248), `startEspionageOperation` (10876)
  - `src/game/core/simulation.js` `SABOTAGE_COSTS` (9739-9744), `SABOTAGE_DURATIONS` (9746-9751), `ASSASSINATION_COST` (9765), `ASSASSINATION_DURATION_SECONDS` (9769), `validateSabotageTarget` (9795-9815), `getSabotageTerms` (9900-9958), `getAssassinationContest` (10214-10282), `getAssassinationTerms` (10284-10323), `startAssassinationOperation` (10912-10950), `startSabotageOperation` (10952-10991)
  - `src/game/core/simulation.js` `getActiveCounterIntelligence` (4104-4111), `getCounterIntelligenceRoleGuardBonus` (4143-4157), `createDynastyIntelligenceReport` (5348-5368), `storeDynastyIntelligenceReport` (5370-5386), `recordCounterIntelligenceInterception` (10121-10171), `createCounterIntelligenceWatch` (10173-10203), `getCounterIntelligenceTerms` (10309-10360), `startCounterIntelligenceOperation` (10836-10874)
  - `src/game/core/simulation.js` search `applyAssassinationEffect`, `tickBuildingStatusEffects`, `LEGITIMACY_LOSS_HEAD_FALL`, `LEGITIMACY_LOSS_COMMANDER_KILL`, `LEGITIMACY_LOSS_GOVERNOR_LOSS`, `LEGITIMACY_LOSS_INTERREGNUM`, and `LEGITIMACY_RECOVERY_ON_SUCCESSION` for the resolution-effects fallout constants and burn/status tick behavior
  - `tests/runtime-bridge.mjs` sabotage assertions (1378-1412), espionage + assassination assertions (3490-3628)
  - `tests/runtime-bridge.mjs` counter-intelligence watch + dossier assertions (4130-4240, 4884-4970)
- Current Branch In Flight: none (resolution-effects follow-up merged onto canonical `master` in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-player-covert-ops-resolution-effects-followup.md`
- Last Slice State:
  - `EspionageResolutionSystem`, `AssassinationResolutionSystem`, and `SabotageResolutionSystem` now consume the stored dispatch-time `SuccessScore`; active gate/burn sabotage windows live on additive `PlayerSabotageEffectComponent` data while the earlier `PlayerSabotageStatusComponent` file remains preserved
  - gate-opening sabotage now promotes a same-frame temporary breach, fire raising burns over elapsed time, supply poisoning writes `BuildingRaidStateComponent` windows, and successful well poisoning raises `RealmConditionComponent.WaterStrainStreak`
  - `IntelligenceReportElement` now carries compact resource/building summaries, and `BloodlinesDebugCommandSurface.PlayerCovertOps` exposes richer dossier plus sabotage-effect readouts for live validation
  - `BloodlinesPlayerCovertOpsResolutionSmokeValidation` plus `scripts/Invoke-BloodlinesUnityPlayerCovertOpsResolutionSmokeValidation.ps1` prove enriched espionage, commander/governor assassination fallout, same-frame breach exposure, and the fire / supply / well sabotage variants
  - local `Assembly-CSharp*.csproj` metadata now explicitly includes `PlayerSabotageEffectComponent` and points Unity.Entities analyzer roots back at the canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` surface
  - next Codex pickup should move to Priority 12 `governance-coalition-pressure`

### Lane: player-hud-realm-condition-legibility

- Status: active
- Branch Prefix: `codex/unity-player-hud-*`, `codex/unity-player-hud-realm-condition-legibility`, `codex/unity-hud-*`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/HUD/**`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryReadoutSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryLeaderboardHUDSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPoliticalStateHUDSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityRealmConditionHUDSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityMatchProgressionHUDSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityVictoryReadoutSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityVictoryLeaderboardHUDSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityBattlefieldCommandDeckSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityPoliticalStateHUDSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/RealmConditionHUDComponent.cs`, `Code/HUD/RealmConditionHUDSystem.cs`, and `Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/MatchProgressionHUDComponent.cs` and `Code/HUD/MatchProgressionHUDSystem.cs`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/FortificationHUDComponent.cs` and `Code/HUD/FortificationHUDSystem.cs`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/VictoryConditionReadoutComponent.cs` and `Code/HUD/VictoryConditionReadoutSystem.cs`; stale analyzer/source-generator paths were corrected back to this worktree's `unity/Library/PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesVictoryReadoutSmokeValidation.cs`; stale analyzer/source-generator paths were corrected back to this worktree's `unity/Library/PackageCache`
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/VictoryLeaderboardHUDComponent.cs` and `Code/HUD/VictoryLeaderboardHUDSystem.cs`; stale analyzer/source-generator paths were corrected back to this worktree's `unity/Library/PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesVictoryLeaderboardHUDSmokeValidation.cs`; stale analyzer/source-generator paths were corrected back to this worktree's `unity/Library/PackageCache`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` -- additive battlefield command-deck summary rendering that consumes existing HUD read-models without widening runtime ownership beyond the player command shell
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBattlefieldCommandDeckSmokeValidation.cs` -- dedicated validator for the unified command-deck summary seam
  - `unity/Assembly-CSharp.csproj` -- compile includes added for `Code/HUD/SuccessionCrisisHUDComponent.cs`, `Code/HUD/SuccessionCrisisHUDSystem.cs`, `Code/HUD/PoliticalEventsTrayHUDComponent.cs`, `Code/HUD/PoliticalEventsTrayHUDSystem.cs`, `Code/HUD/CovenantTestProgressHUDComponent.cs`, `Code/HUD/CovenantTestProgressHUDSystem.cs`, `Code/HUD/TruebornRiseHUDComponent.cs`, and `Code/HUD/TruebornRiseHUDSystem.cs`; stale analyzer/source-generator paths were corrected back to canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
  - `unity/Assembly-CSharp-Editor.csproj` -- compile include added for `Code/Editor/BloodlinesPoliticalStateHUDSmokeValidation.cs`; stale analyzer/source-generator paths were corrected back to canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- resolve HUD snapshots by `FactionId`
  - `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs` -- read realm cycle accumulator, cycle count, strain streaks, and realm legibility thresholds
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read total and cap for population-pressure readout
  - `unity/Assets/_Bloodlines/Code/Economy/FactionLoyaltyComponent.cs` -- read current/max/floor loyalty for the player-facing loyalty band
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- read covenant, doctrine path, intensity, and level for faith readout
  - `unity/Assets/_Bloodlines/Code/Components/ConvictionComponent.cs` -- read the already-resolved conviction band for conviction readout
  - `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoring.cs` -- reuse canonical band-label mapping helpers rather than duplicating conviction text rules
  - `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs` -- update ordering only; HUD must observe post-response state
  - `unity/Assets/_Bloodlines/Code/Economy/CapPressureResponseSystem.cs` -- update ordering only; HUD must observe post-response state
  - `unity/Assets/_Bloodlines/Code/Economy/StabilitySurplusResponseSystem.cs` -- update ordering only; HUD must observe post-response state
  - `unity/Assets/_Bloodlines/Code/Faith/FaithIntensityResolveSystem.cs` -- update ordering only; HUD must observe resolved faith levels
  - `unity/Assets/_Bloodlines/Code/Time/MatchProgressionComponent.cs` -- read stage, phase, readiness, declaration, and Great Reckoning state for the match HUD read-model
  - `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureComponent.cs` -- read dominant-leader or Great Reckoning target pressure state for the match HUD read-model
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs` -- read settlement tier and live breach counts for the fortification HUD read-model
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs` -- read reserve readiness, recovery, and threat state for the fortification HUD read-model
  - `unity/Assets/_Bloodlines/Code/Components/BreachSealingProgressComponent.cs` -- read normalized sealing progress for breached settlements
  - `unity/Assets/_Bloodlines/Code/Components/DestroyedCounterRecoveryProgressComponent.cs` -- read normalized rebuild progress after sealing completes
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveAssignmentComponent.cs` -- read mustered-vs-ready reserve duty state without widening the fortification lane
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationSettlementLinkComponent.cs` -- resolve linked defenders back to each settlement HUD row
  - `unity/Assets/_Bloodlines/Code/Victory/VictoryComponents.cs` -- reserved for follow-up victory readout slice inside this same lane
  - `unity/Assets/_Bloodlines/Code/Victory/VictoryConditionEvaluationSystem.cs` -- reuse canonical thresholds and the player sovereignty-hold timer already tracked by the retired victory lane
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- read control-point owner and loyalty for Territorial Governance progress
  - `unity/Assets/_Bloodlines/Code/Components/BuildingTypeComponent.cs` -- read command-hall identity for Command Hall Fall progress
  - `unity/Assets/_Bloodlines/Code/Components/HealthComponent.cs` -- read `DeadTag` on command halls for Command Hall Fall completion
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- read in-world-day cadence plus days-per-real-second for the HUD throttle and sovereignty ETA
  - `unity/Assets/_Bloodlines/Code/Dynasty/SuccessionCrisisComponent.cs` -- read live succession severity, fallback timers, and recovery windows for the dynasty-political HUD state
  - `unity/Assets/_Bloodlines/Code/Dynasty/DynastyPoliticalEventComponent.cs` -- read active event type, expiry, and target ordering for the political events tray
  - `unity/Assets/_Bloodlines/Code/Faith/CovenantTestComponent.cs` -- read covenant-test stage, elapsed progress, and political cooldown windows for the covenant HUD panel
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRiseArcComponent.cs` -- read recognition bitmask, ultimatum timings, and stage progression for the Trueborn HUD panel
  - `unity/Assets/_Bloodlines/Code/WorldPressure/TruebornRecognitionUtility.cs` -- reuse canonical recognition-slot counting rather than duplicating Trueborn state rules
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-21-unity-player-hud-realm-condition-legibility.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-hud-realm-condition-legibility-landing.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-hud-match-progression.md`
  - `docs/unity/session-handoffs/2026-04-21-unity-player-hud-match-progression-landing.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-hud-fortification.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-hud-fortification-rerun.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-hud-victory-distance-readout.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-hud-victory-panel.md`
  - `docs/unity/session-handoffs/2026-04-22-unity-player-hud-command-deck-summary.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-hud-political-state-panels.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-hud-political-state-panels-landing.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-hud-political-state-panels-rerun-landing.md`
  - `docs/unity/session-handoffs/2026-04-23-unity-hud-political-state-panels-rerun-master-refresh.md`
- Browser Reference:
  - `src/game/core/simulation.js` `getRealmConditionSnapshot` (14291-14764), `getMatchProgressionSnapshot` (13650-13658)
  - `tests/runtime-bridge.mjs` realm-condition snapshot assertions (1344-1364), match-progression assertions (7521, 7773-7871, 7923-7975, 8133, 8185), fortification/readout assertions (1438-1444), hostile-post-repulse world-pressure assertions (1718-1733)
- `src/game/core/simulation.js` dynasty political-state helpers `getDynastyPoliticalEventState`, `getActiveSuccessionCrisis`, `getActiveCovenantTest`, and `tickTruebornRiseArc`
- Current Branch In Flight: none (refreshed against the current canonical `master` lineage in this session)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-hud-political-state-panels-rerun-master-refresh.md`
- Last Slice State:
  - the original HUD political-state panel landing remains preserved on canonical `master`, and the rerun refresh now proves that same behavior still holds on top of the newer master ancestry that already includes the player covenant-test dispatch landing, without widening ownership into `AI/**`
  - `BloodlinesDebugCommandSurface.HUD` still exposes the political-state snapshot helpers, `BloodlinesPoliticalStateHUDSmokeValidation` reran green on the refreshed merge result, and the merged line also retained the dedicated player covenant-test dispatch smoke proof surface
  - the refreshed merged result reran the full governed 10-gate chain green, plus the dedicated HUD and player covenant-test dispatch smokes, and the committed `unity/Assembly-CSharp*.csproj` files still preserve the canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer roots for fresh automation worktrees

### Lane: siege-escalation

- Status: complete (merged to master via `claude/unity-siege-escalation-arc`)
- Branch Prefix: `claude/unity-siege-escalation-arc`
- Owner Agent: claude
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationCanon.cs`
  - `unity/Assets/_Bloodlines/Code/Siege/SiegeEscalationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SiegeEscalation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeEscalationSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnitySiegeEscalationSmokeValidation.ps1`
- Shared-File Narrow Edits:
  - `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs` -- added `using Bloodlines.Siege;`, `WithEntityAccess()` on the query, optional read of `FactionSiegeEscalationStateComponent.StarvationMultiplier`, applied to `scaledDecline` before conviction protection
  - `unity/Assembly-CSharp.csproj` -- added Compile entries for SiegeEscalation*.cs and debug surface partial
  - `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for BloodlinesSiegeEscalationSmokeValidation.cs
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveComponent.cs` -- read `ThreatActive` as siege-active gate
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- faction identity lookup
  - `unity/Assets/_Bloodlines/Code/Economy/FactionLoyaltyComponent.cs` -- apply accumulated morale penalty
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- read `InWorldDays` for duration accumulation
  - `unity/Assets/_Bloodlines/Code/Components/SettlementComponent.cs` -- settlement query filter
- Browser Reference: absent (tickSiegeEscalation not in simulation.js -- implemented from canonical siege doctrine)
- Current Branch In Flight: none (merged to master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-siege-escalation.md`
- Last Slice State:
  - `SiegeEscalationComponent` (per-settlement), `FactionSiegeEscalationStateComponent` (per-faction), `SiegeEscalationCanon` (thresholds/multipliers), and `SiegeEscalationSystem` now live on canonical `master`
  - System increments SiegeDurationInWorldDays each frame, advances tier at 7/14/21d thresholds, writes per-faction aggregate StarvationMultiplier to `FactionSiegeEscalationStateComponent`, applies accumulated MoralePenaltyPerDay to faction loyalty
  - `StarvationResponseSystem` now reads `FactionSiegeEscalationStateComponent` (narrow edit) and scales famine population decline by the siege multiplier
  - `BloodlinesSiegeEscalationSmokeValidation` proves Normal/Prolonged/Severe/Critical canon behavior and starvation wiring contract; PS1 wrapper and csproj entries in place
  - All 10 validation gates passed (CS0006 Library-absent only, no code errors)

### Lane: patrol-route

- Status: complete (merged to master via `claude/unity-combat-patrol-route`)
- Branch Prefix: `claude/unity-combat-patrol-route`
- Owner Agent: claude
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/PatrolRouteComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Combat/PatrolOrderSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Combat/PatrolMovementSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Patrol.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPatrolRouteSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityPatrolRouteSmokeValidation.ps1`
- Shared-File Narrow Edits:
  - `unity/Assembly-CSharp.csproj` -- added Compile entries for PatrolRoute*.cs, PatrolOrder/Movement systems, debug surface partial
  - `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for BloodlinesPatrolRouteSmokeValidation.cs
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/AttackOrderComponent.cs` -- read IsActive to suspend patrol
  - `unity/Assets/_Bloodlines/Code/Components/MoveCommandComponent.cs` -- write waypoint move orders
  - `unity/Assets/_Bloodlines/Code/Combat/AttackOrderResolutionSystem.cs` -- UpdateBefore target
  - `unity/Assets/_Bloodlines/Code/Components/UnitTypeComponent.cs` -- unit entity validation in debug surface
- Browser Reference: absent (patrol route not in simulation.js; implemented from canonical garrison/perimeter design)
- Current Branch In Flight: none (merged to master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-patrol-route.md`
- Last Slice State:
  - `PatrolRouteComponent` (per-unit: WaypointA/B, CurrentTarget, IsPatrolling, ArrivalThreshold), `PlayerPatrolOrderRequestComponent` (one-shot set), `PlayerPatrolCancelRequestComponent` (one-shot cancel), `PatrolOrderSystem` (processes set/cancel), and `PatrolMovementSystem` (flips waypoints at arrival, suspends on attack order, reissues on cleared move command) now live on canonical `master`
  - All 10 validation gates passed (CS0006 Library-absent only, no code errors)

### Lane: dynasty-progression

- Status: complete (merged to master via `claude/unity-dynasty-progression-unlocks`)
- Branch Prefix: `claude/unity-dynasty-progression-unlocks`
- Owner Agent: claude
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionCanon.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyXPAwardSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyUnlockSlotElement.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionUnlockSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Dynasties/SpecialUnitSwapApplicatorSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.DynastyProgression.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesDynastyProgressionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityDynastyProgressionSmokeValidation.ps1`
- Shared-File Narrow Edits:
  - `unity/Assembly-CSharp.csproj` -- added Compile entries for DynastyProgression*.cs, DynastyXPAwardSystem.cs, DynastyUnlockSlotElement.cs, DynastyProgressionUnlockSystem.cs, SpecialUnitSwapApplicatorSystem.cs, and debug surface partial
  - `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for BloodlinesDynastyProgressionSmokeValidation.cs
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- faction entity query filter
- Browser Reference: absent (dynasty progression XP/unlock system not in simulation.js; implemented from canonical progression design per owner direction 2026-04-19)
- Current Branch In Flight: none (merged to master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-dynasty-progression.md`
- Last Slice State:
  - `DynastyProgressionComponent` (per-faction: AccumulatedXP, CurrentTier, LastMatchXPAward, TierUnlocksPending), `DynastyProgressionCanon` (XP thresholds, placement schedule, tier-for-XP helpers), `DynastyXPAwardSystem` (consumes XPAwardRequest, advances tier, fires TierUnlockNotification), `DynastyUnlockSlotElement` (DynamicBuffer element: SlotIndex, UnlockTypeId, UnlockTargetId, GrantedAtTier, IsActive), `DynastyProgressionUnlockSystem` (consumes notification, writes slots cycling SpecialUnitSwap/ResourceBonus/DiplomacyBonus/CombatBonus), and `SpecialUnitSwapApplicatorSystem` (reads active swap slot, writes FactionSpecialUnitSwapComponent) now live on canonical `master`
  - `BloodlinesDynastyProgressionSmokeValidation` proves tier advancement, unlock slot cycling, and applicator logic across 3 phases; PS1 wrapper and csproj entries in place
  - All 10 validation gates passed (CS0006 Library-absent only, no code errors; Unity batch-mode smoke SKIP-env per established environment condition)

### Lane: rally-point

- Status: complete (merged to master via `claude/unity-combat-rally-point`)
- Branch Prefix: `claude/unity-combat-rally-point`
- Owner Agent: claude
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Combat/RallyPointComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Combat/RallyPointSetSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.RallyPoint.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRallyPointSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityRallyPointSmokeValidation.ps1`
- Shared-File Narrow Edits:
  - `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs` -- added `using Bloodlines.Combat;`, reads `RallyPointComponent` from building entity before spawn, passes `hasActiveRallyPoint` and `rallyPosition` to `SpawnQueuedUnit`, `SpawnQueuedUnit` sets `MoveCommandComponent.Destination` to rally position when active
  - `unity/Assembly-CSharp.csproj` -- added Compile entries for RallyPointComponent.cs, RallyPointSetSystem.cs, and debug surface partial
  - `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for BloodlinesRallyPointSmokeValidation.cs
- Cross-Lane Fixes (bugs corrected in prior-lane files while running build gate):
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SiegeEscalation.cs` -- fixed `Unity.Collections.ComponentType` to `ComponentType` (siege-escalation lane bug, CS0234)
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyXPAwardSystem.cs` -- fixed `Unity.Collections.ComponentType` to `ComponentType` (dynasty-progression lane bug, CS0234); fixed `math.min(byte, byte)` ambiguity by casting to `int` (CS0121)
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionCanon.cs` -- fixed `math.min(byte, byte)` ambiguity by casting to `int` (CS0121)
  - `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionUnlockSystem.cs` -- fixed `Unity.Collections.ComponentType` to `ComponentType` (dynasty-progression lane bug, CS0234)
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/BuildingTypeComponent.cs` -- read for building entity validation in debug surface
  - `unity/Assets/_Bloodlines/Code/Components/MoveCommandComponent.cs` -- write rally destination into spawn
- Browser Reference: absent (rally point not in simulation.js; implemented from canonical production design)
- Current Branch In Flight: none (merged to master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-rally-point.md`
- Last Slice State:
  - `RallyPointComponent` (per-building: TargetPosition, IsActive) and `RallyPointSetSystem` (consumes `PlayerRallyPointSetRequestComponent`) now live on canonical `master`
  - `UnitProductionSystem` reads `RallyPointComponent` at spawn time and routes `MoveCommandComponent` to rally position when active
  - `BloodlinesRallyPointSmokeValidation` proves struct initialization, set/clear request round-trips, and spawn resolution logic; PS1 wrapper and csproj entries in place
  - Four bugs in prior-lane files (CS0234 + CS0121) corrected as part of build-gate pass
  - All 10 validation gates passed (CS0006 Library-absent only, no code errors)

### Lane: world-trade-routes

- Status: complete (merged to master via `claude/unity-world-trade-routes`)
- Branch Prefix: `claude/unity-world-trade-routes`
- Owner Agent: claude
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Economy/TradeRouteComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Economy/TradeRouteEvaluationSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.TradeRoutes.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTradeRouteSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityTradeRouteSmokeValidation.ps1`
- Shared-File Narrow Edits:
  - `unity/Assembly-CSharp.csproj` -- added Compile entries for TradeRouteComponent.cs, TradeRouteEvaluationSystem.cs, and debug surface partial
  - `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for BloodlinesTradeRouteSmokeValidation.cs
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs` -- read IsContested, OwnerFactionId, RadiusTiles for adjacency evaluation
  - `unity/Assets/_Bloodlines/Code/Components/PositionComponent.cs` -- read faction position for distance calculation
  - `unity/Assets/_Bloodlines/Code/Components/FactionComponent.cs` -- faction entity query filter
  - `unity/Assets/_Bloodlines/Code/Economy/ResourceStockpileComponent.cs` -- add gold yield to stockpile
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- day-cadence gate (once per in-world day)
- Browser Reference: absent (trade route system not in simulation.js; implemented from canonical trade route design)
- Current Branch In Flight: none (merged to master)
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-23-unity-world-trade-routes.md`
- Last Slice State:
  - `TradeRouteComponent` (per-faction: ActiveRouteCount, TotalGoldPerTickFromTrades, LastUpdatedAtInWorldDays) and `TradeRouteEvaluationSystem` (once-per-day O(n²) adjacency check: two uncontested owned CPs adjacent when distance ≤ radiusA + radiusB + 2 tiles; 5 gold/route/day added to ResourceStockpileComponent) now live on canonical `master`
  - `BloodlinesTradeRouteSmokeValidation` proves adjacent uncontested pair yields 1 route/5 gold, contested CP exclusion, and cross-faction route rejection across 3 phases; PS1 wrapper and csproj entries in place
  - All 10 validation gates passed (0 errors; Unity batch-mode smoke SKIP-env per established environment condition)

## Next Unblocked Tier 1 Lanes (Unclaimed)

Forward work is prioritized in the browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`. The items below are unblocked and unclaimed. Any agent resuming a session may claim one by adding an entry under Active Lanes above, bumping Revision, and proceeding.

Note: the fortification queue is now closed cleanly through sub-slice 13 and the older `fortification-siege-imminent-engagement` lane remains paused outside fresh claims like `fortification-postures`. The repo already contains the retired `tier2-batch-dynasty-systems` lane and Codex's follow-up `dynasty-house-parity` hardening work, so do not duplicate marriages, lesser houses, or minor houses under a fresh zero-code lane. The scout-raids foundation, player covert ops, non-AI Trueborn follow-up slices through recognized-pressure, the HUD political-state panels landing plus rerun refresh, the player covenant-test dispatch landing, and the contested-territory pressure landing are now on canonical `master`. The next clean Codex pickup should claim the next unblocked non-AI lane from `docs/plans/2026-04-17-browser-to-unity-migration-plan.md` rather than reopening stale rerun branches.

### Next Lane Candidate: ai-strategic-layer-sub-slice-5-siege-staging

- Status: DONE (merged to master via `claude/unity-ai-siege-staging`; see sub-slice 5 handoff).

### Next Lane Candidate: ai-strategic-layer-sub-slice-6-dynasty-covert-ops

- Status: DONE (merged to master via `claude/unity-ai-dynasty-covert-ops`; see sub-slice 6 handoff).
- Browser Reference: `src/game/core/ai.js` updateEnemyAi dynasty covert ops block ~lines 2419-2678.

### Next Lane Candidate: ai-strategic-layer-sub-slice-7-build-timer-chain

- Status: DONE (landed via `claude/unity-ai-build-timer-chain`; see sub-slice 7 handoff).
- Browser Reference: `src/game/core/ai.js` updateEnemyAi buildTimer<=0 block ~lines 1377-1573.

### Next Lane Candidate: ai-strategic-layer-sub-slice-8-marriage-proposal-execution

- Status: DONE (landed via `claude/unity-ai-marriage-proposal-execution`; see sub-slice 8 handoff).
- Browser Reference: `src/game/core/ai.js` `tryAiMarriageProposal` (~2897-2944).

### Next Lane Candidate: ai-strategic-layer-sub-slice-9-marriage-inbox-accept-execution

- Status: DONE (landed via `claude/unity-ai-marriage-inbox-accept`; see sub-slice 9 handoff).
- Browser Reference: `src/game/core/ai.js` `tryAiAcceptIncomingMarriage` (~2880-2895).

### Next Lane Candidate: ai-strategic-layer-sub-slice-10-marriage-strategic-profile

- Status: DONE (landed via `claude/unity-ai-marriage-strategic-profile`; see sub-slice 10 handoff).
- Browser Reference: `src/game/core/ai.js` `getAiMarriageStrategicProfile` (~2803-2839).

### Next Lane Candidate: ai-strategic-layer-sub-slice-11-marriage-accept-effects

- Status: DONE (landed via `claude/unity-ai-marriage-accept-effects`; see sub-slice 11 handoff).
- Browser Reference: `src/game/core/simulation.js` `acceptMarriage` (~7388-7469).

### Next Lane Candidate: ai-strategic-layer-sub-slice-12-marriage-acceptance-terms

- Status: DONE (landed via `claude/unity-ai-marriage-acceptance-terms`; see sub-slice 12 handoff).
- Browser Reference: `src/game/core/simulation.js` `getMarriageAcceptanceTerms` (~6327), `applyMarriageGovernanceLegitimacyCost` (~6232), `getMarriageAuthorityProfile` (~6134), `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (~6091).

### Next Lane Candidate: ai-strategic-layer-sub-slice-13-lesser-house-promotion

- Status: DONE (landed via `claude/unity-ai-lesser-house-promotion`; see sub-slice 13 handoff).
- Browser Reference: `src/game/core/ai.js` `tryAiPromoteLesserHouse` (~2784-2801), `src/game/core/simulation.js` `promoteMemberToLesserHouse` (~7184-7258), `memberIsLesserHouseCandidate` (~6469-6479), constants block (~6444-6457).

### Next Lane Candidate: ai-strategic-layer-sub-slice-14-narrative-message-bridge

- Status: DONE (landed via `claude/unity-ai-narrative-message-bridge` as sub-slice 16; see sub-slice 16 handoff).
- Browser Reference: `src/game/core/simulation.js` `pushMessage` (search), called from `acceptMarriage` (~7463), `promoteMemberToLesserHouse` (~7251), and many other AI paths.

### Next Lane Candidate: ai-strategic-layer-sub-slice-17-narrative-back-wire

- Status: DONE (landed via `claude/unity-ai-narrative-back-wire`; see sub-slice 17 handoff).
- Browser Reference: `src/game/core/simulation.js` pushMessage call sites at `acceptMarriage` (~7463), `promoteMemberToLesserHouse` (~7251-7255), and `proposeNonAggressionPact` (~5216-5220).

### Next Lane Candidate: ai-strategic-layer-sub-slice-14-pact-proposal-execution

- Status: DONE (landed via `claude/unity-ai-pact-proposal-execution`; see sub-slice 14 handoff).
- Browser Reference: `src/game/core/ai.js` pact block (~2643-2666); `src/game/core/simulation.js` `getNonAggressionPactTerms` (~5150-5183), `proposeNonAggressionPact` (~5185-5222), constants block (~5126-5128).

### Next Lane Candidate: ai-strategic-layer-sub-slice-15-pact-break-and-expiration

- Status: DONE (landed via `claude/unity-ai-pact-break-expiration`; see sub-slice 15 handoff).
- Browser Reference: `src/game/core/simulation.js` `breakNonAggressionPact` (~5224-5257); NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8 (~5129).

### Next Lane Candidate: ai-strategic-layer-sub-slice-16-captive-recovery-execution

- Suggested Branch: new branch `claude/unity-ai-captive-recovery-execution` or similar.
- Target Paths: `unity/Assets/_Bloodlines/Code/AI/**` (additive).
- Browser Reference: `src/game/core/ai.js` captive-recovery dispatch block (~2566-2608) plus the simulation-side `startCaptiveRescueOperation` and `startCaptiveRansomOperation` sinks.
- Scope: consume `AICovertOpsComponent.LastFiredOp == CovertOpKind.CaptiveRescue` or `== CaptiveRansom` from sub-slice 6 and execute the respective operation. The two paths share dispatch context (CaptivesSourceFocused, HighPriorityCaptive, EnemyIsHostileToPlayer) but diverge on operation mechanics (rescue attempts a military extraction; ransom pays resources to return). Follows the one-shot pattern from sub-slices 8/9/12/13/14/15.

### Next Lane Candidate: ai-strategic-layer-sub-slice-8-command-dispatch

- Status: DONE (landed via `codex/unity-ai-command-dispatch`; see sub-slice 8 command-dispatch handoff).
- Browser Reference: `src/game/core/ai.js` idle worker `issueGatherCommand` dispatch (~1243-1260), territory expansion command dispatch (~1575-1600).

## Shared Files (Narrow Modification Rights for All Lanes)

The following files may be edited by any lane, but only via narrow, additive changes. No lane may rewrite, restructure, or behaviorally alter these files outside of its own seam.

- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
  Rule: add new field readings from definition assets and seed payloads only. No behavioral rewrites. No moving or removing existing authoring logic.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
  Rule: add new component baking calls for new seed element fields only. No behavioral rewrites. No removing existing baking logic.

- `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs`
  Rule: add new fields to seed element structs only. No removing or renaming existing fields. No restructuring the seed element layout.

- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
  Rule: add new component attachments in `SpawnFactionEntity` and adjacent spawn helpers only. No behavioral rewrites. No removing or reordering existing spawn logic.

- `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs`
  Rule: add new component attachments on produced-unit spawn only. No behavioral rewrites. No removing existing component wiring.

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`
  Rule: narrow fallback-check additions and new entity proxy types only. No rewrite. No behavioral change to existing proxy types. Both the combat lane and the economy/AI lane depend on this file.

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
  Rule: add new debug API methods and HUD readout segments only. Do not remove or rename existing methods. Do not reorder the partial class split. When adding a substantial new command surface area, use a new partial file (e.g. `BloodlinesDebugCommandSurface.AI.cs`, `BloodlinesDebugCommandSurface.Combat.cs`).

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`
  Rule: narrow additions only. New fields in `RuntimeSmokeState`, new probe phases appended after existing phases, new diagnostic output fields. Do not remove or reorder existing probe phases. Do not change the success-line format of prior probes. All existing probes must remain green after each addition.

- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`
  Rule: add new field mappings in importer record types only. No removing existing field mappings. No changing existing import logic.

- `unity/Assembly-CSharp.csproj`
  Rule: add new `<Compile Include="..." />` entries for new `.cs` files only. Do not remove existing references. Do not change project structure, target framework, or preprocessor defines.

- `unity/Assembly-CSharp-Editor.csproj`
  Rule: same as `Assembly-CSharp.csproj`. New editor-script registrations only.

## Forbidden Paths (All Lanes)

No lane may touch the following paths without explicit per-slice authorization from Lance. Each use requires a named justification in the slice handoff.

- `unity/Assets/_Bloodlines/Scenes/**` - Unity scene files (`.unity`). Structural scene changes may only occur via the governed scene shell tools (`scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`, etc.) with explicit authorization.
- `unity/ProjectSettings/**` - Unity project-wide settings. Changes affect all lanes and require explicit authorization.
- `unity/Packages/manifest.json`, `unity/Packages/packages-lock.json` - Package manifest changes require explicit authorization and justification for the new package.
- `src/**`, `play.html` - Browser runtime source. Frozen as behavioral specification. No new systems; only bug fixes and parity-instrumentation explicitly authorized.
- `data/*.json` - Canonical game-data files. These drive the JSON import pipeline. Changes require explicit authorization and re-import verification.
- `archive_preserved_sources/**`, `19_ARCHIVE/**`, `governance/**` - Preservation zones. Read-only without explicit authorization.

## State Documents (Append-Only At End Of Slice)

The following files are immutable mid-slice and append-only at end-of-slice (after rebase, before push):

- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `continuity/PROJECT_STATE.json`

Update these only after all validation gates are green. Never overwrite another lane's entries. Always rebase before updating to avoid divergent state.

## Branch Discipline

Every lane works on its own named branch following the prefix documented in the lane's subsection above. Before pushing, rebase the lane branch onto `origin/master` to keep the merge history linear. Push to the lane branch only. Never push directly to `master`. Merges to `master` are human-coordinated by Lance; neither agent auto-merges the other's branch. If two lanes have diverged significantly at merge time, produce a coordination note in a merge plan document before merging.

## Wrapper Lock

Every Unity batch invocation must go through `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1` with `-Session <lane-identifier>` matching the lane name. The lock file lives at `.unity-wrapper-lock` in the repo root. Before invoking any Unity wrapper: check whether the lock file exists; if it does and its timestamp is less than 15 minutes old and its session name is not yours, poll every 30 seconds up to 10 minutes. If the lock is older than 15 minutes, reclaim it by overwriting with your session name and current ISO UTC timestamp. Never run two Unity wrapper scripts in parallel from the same session. Delete the lock file after the wrapper completes (success or failure).

## Validation Gate (Canonical Order)

Every slice must pass all of the following before handoff. Run them serially because Unity holds a project lock.

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- must exit 0 with 0 errors.
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- must exit 0 with 0 errors.
3. `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` -- must remain green; success line must carry all prior proof fields.
4. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1` -- must remain green for all currently-governed combat phases. All lanes are responsible for not breaking the combat smoke.
5. `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` -- both Bootstrap and Gameplay scene shells must validate.
6. `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` -- must remain green for fortification tier and reserve-duty proof phases.
7. `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1` -- must remain green for baseline, strain, recovery, and siege-support proof phases.
8. `node tests/data-validation.mjs` -- must exit 0.
9. `node tests/runtime-bridge.mjs` -- must exit 0.
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- must exit 0; confirms this contract is not older than the newest session handoff.

## Staleness Rule

Any resume session that detects the Last Updated date of this document is older than the date of the latest file under `docs/unity/session-handoffs/` must STOP and surface the staleness before doing any other work. The correct action is to bump the Revision, set Last Updated to today, set Last Updated By to the current session identifier, and amend the affected lane subsections to reflect current reality. The `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` script automates this check and is part of the canonical validation gate. The contract revision must be bumped whenever a lane is created, renamed, retired, or has its owned paths changed.

## Reconciliation Notes

The following discrepancies were found between the pre-schema contract (revision 0), the per-slice handoffs, and `NEXT_SESSION_HANDOFF.md`. They are recorded for one revision cycle and will be cleared on the next revision if resolved.

1. **Economy branch name mismatch.** The pre-schema contract named the Claude economy branch `claude/unity-food-water-economy`. The actual branches used were `claude/unity-food-water-economy` (Sessions 113-116) and `claude/unity-enemy-ai-economic-base` (Sessions 120-124). Both are now merged. The lane is documented here as `economy-and-ai` to reflect the full scope it grew to cover.

2. **AI paths absent from pre-schema contract.** The pre-schema contract did not list `Code/AI/**` or `BloodlinesDebugCommandSurface.AI.cs` as Claude-owned. Per the Session 120-121 handoffs, Claude created `AIEconomyControllerComponent.cs` under `Code/AI/` and the partial class file `BloodlinesDebugCommandSurface.AI.cs` under `Code/Debug/`. These are now retroactively documented under the retired economy-and-ai lane.

3. **Two Codex combat branches not distinguished.** The pre-schema contract described a single combat-foundation lane on `codex/unity-combat-foundation`. In practice two branches were used: `codex/unity-combat-foundation` (merged) and `codex/unity-projectile-combat` (merged). Both are now documented under the retired combat-and-projectile lane.

4. **Combat smoke gate was lane-specific, not universal.** The pre-schema contract listed the combat smoke validator as optional for the economy lane. Per the handoff record, every economy-lane slice preserved the combat smoke as green once it existed. The canonical gate now mandates it for all lanes.

5. **`BloodlinesBootstrapRuntimeSmokeValidation.cs` ownership ambiguity.** The pre-schema contract listed this file as exclusively owned by the economy lane. Per the Session 113+ handoff record, all lane slices must keep it green and may add narrow diagnostic phases when their new systems require proof. It is now documented as a shared file, not exclusively owned.

6. **`BloodlinesDebugCommandSurface.cs` absent from shared-file list.** This file was implicitly economy-lane-owned in the pre-schema contract. The combat-attack-move lane will need to add debug command APIs for attack orders. It is now documented as a shared file with narrow-modification rules. No conflict has occurred yet.

7. **Stacked Codex combat follow-up.** Lance delegated merge coordination for `codex/unity-attack-move` separately to Claude while Codex continued onto the next combat opportunity. Revision 2 documents the stacked follow-up branch `codex/unity-target-acquisition-los` explicitly so the lane and wrapper-lock ownership remain unambiguous.
