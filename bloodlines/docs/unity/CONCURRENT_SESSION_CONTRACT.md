# Bloodlines Unity Concurrent Session Contract

## Contract Metadata

- Revision: 28
- Last Updated: 2026-04-19
- Last Updated By: claude-ai-lesser-house-promotion-2026-04-19
- Supersedes: revision 27 (ai-strategic-layer sub-slice 13 landed: AILesserHousePromotionSystem ports ai.js tryAiPromoteLesserHouse (~2784-2801) plus the mechanical core of simulation.js promoteMemberToLesserHouse (~7184-7258); consumes AICovertOpsComponent.LastFiredOp == LesserHousePromotion from sub-slice 6; gates on dynasty legitimacy < 90 and lesser-house buffer count < 3; eligibility walks DynastyMemberRef buffer for Active/Ruling non-head members on Governance/MilitaryCommand/CovertOperations paths with Renown >= 30 who are not already founders of an existing LesserHouseElement; on success appends a new LesserHouseElement at Loyalty=75 so LesserHouseLoyaltyDriftSystem picks it up next in-world day, applies legitimacy +3 clamped to 100, and records a Stewardship +2 conviction event via ConvictionScoring.ApplyEvent; one-shot dispatch consumption matches sub-slices 8/9/12 pattern; deferred: per-member FoundedLesserHouseId field, promotion-history gate, marital-anchor + cadet world-pressure profiles, narrative pushMessage. BloodlinesAILesserHousePromotionSmokeValidation 6-phase validator covers successful promotion, legitimacy >= 90 block, lesser-house cap (3) block, head-of-bloodline rejection, non-qualifying-path rejection, and near-ceiling promotion arithmetic)

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

### Lane: ai-strategic-layer

- Status: active
- Branch Prefix: `claude/unity-ai-lesser-house-promotion` (sub-slice 13); prior claude/unity-ai-marriage-acceptance-terms (12), claude/unity-ai-marriage-accept-effects (11), claude/unity-ai-marriage-strategic-profile (10), claude/unity-ai-marriage-inbox-accept (9), codex/unity-ai-command-dispatch (8) also landed; future sub-slices on new branches
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
  - `scripts/Invoke-BloodlinesUnityAICommandDispatchSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- `AIStrategyComponent` seeded on non-player Kingdom faction entities alongside `AIEconomyControllerComponent`
  - `unity/Assembly-CSharp.csproj` -- `AIStrategyComponent.cs`, `EnemyAIStrategySystem.cs`, `AIStrategicPressureSystem.cs`, `AIWorkerGatherSystem.cs`, `AISiegeOrchestrationComponent.cs`, `AISiegeOrchestrationSystem.cs`, `AICovertOpsComponent.cs`, `AICovertOpsSystem.cs`, `AIBuildOrderComponent.cs`, `AIBuildOrderSystem.cs`, `AIMarriageProposalExecutionSystem.cs`, `AIMarriageInboxAcceptSystem.cs`, `AIMarriageStrategicProfileSystem.cs`, `MarriageAcceptEffectsPendingTag.cs`, `AIMarriageAcceptEffectsSystem.cs`, `MarriageAcceptanceTermsComponent.cs`, `MarriageAuthorityEvaluator.cs`, `AILesserHousePromotionSystem.cs`, `AIWorkerCommandSystem.cs`, `AITerritoryDispatchSystem.cs`, `WorkerGatherOrderComponent.cs` registered
  - `unity/Assembly-CSharp-Editor.csproj` -- `BloodlinesAIStrategySmokeValidation.cs`, `BloodlinesAIStrategicPressureSmokeValidation.cs`, `BloodlinesAIGovernancePressureSmokeValidation.cs`, `BloodlinesAIWorkerGatherSmokeValidation.cs`, `BloodlinesAISiegeOrchestrationSmokeValidation.cs`, `BloodlinesAICovertOpsSmokeValidation.cs`, `BloodlinesAIBuildOrderSmokeValidation.cs`, `BloodlinesAIMarriageProposalExecutionSmokeValidation.cs`, `BloodlinesAIMarriageInboxAcceptSmokeValidation.cs`, `BloodlinesAIMarriageStrategicProfileSmokeValidation.cs`, `BloodlinesAIMarriageAcceptEffectsSmokeValidation.cs`, `BloodlinesAIMarriageAcceptanceTermsSmokeValidation.cs`, `BloodlinesAILesserHousePromotionSmokeValidation.cs`, `BloodlinesAICommandDispatchSmokeValidation.cs` registered
  - `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs` -- workers now must travel inside `GatherRadius` before harvesting; `AIWorkerCommandSystem` may flip `Seeking -> Gathering` immediately but harvest does not start until arrival
- Cross-Lane Reads (no writes):
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs` -- read `MarriageComponent` (already-married gate) and `MarriageProposalComponent` / `MarriageProposalStatus` (already-pending gate, proposal creation, accept flip). Sub-slice 8 creates new `MarriageProposalComponent` entities; sub-slice 9 creates new `MarriageComponent` entities and mutates existing `MarriageProposalComponent.Status` pending->accepted. Does not modify existing dynasty system code.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageGestationSystem.cs` -- read `GestationInWorldDays` constant (sub-slice 9) so expected child timestamps stay synchronized with the canonical gestation window.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageProposalExpirationSystem.cs` -- read `ExpirationInWorldDays` constant (sub-slice 8) so expiration timestamps stay synchronized with the canonical expiration window.
  - `unity/Assets/_Bloodlines/Code/Components/DynastyMemberComponent.cs` -- read `DynastyMemberComponent` fields and `DynastyMemberRef` buffer for candidate selection (sub-slice 8) and for `MarriageAuthorityEvaluator` head-direct/heir-regency/envoy-regency resolution (sub-slice 12).
  - `unity/Assets/_Bloodlines/Code/Components/PopulationComponent.cs` -- read `PopulationComponent.Total` for population-deficit signal (sub-slice 10).
  - `unity/Assets/_Bloodlines/Code/Components/FaithComponent.cs` -- read `FaithStateComponent.SelectedFaith` and `DoctrinePath` for simplified faith-compatibility tier (sub-slice 10).
  - `unity/Assets/_Bloodlines/Code/Combat/HostilityComponent.cs` -- read `HostilityComponent` buffer for isHostile signal (sub-slice 10); sub-slice 11 mutates the buffer by removing entries both ways on marriage accept.
  - `unity/Assets/_Bloodlines/Code/Conviction/ConvictionScoring.cs` -- call `ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, +2f)` on both factions' `ConvictionComponent` when a marriage is accepted (sub-slice 11). The helper refreshes score and band in place.
  - `unity/Assets/_Bloodlines/Code/Time/DeclareInWorldTimeRequest.cs` -- push a 30-day request onto the DualClock singleton buffer when a marriage is accepted (sub-slice 11). `DualClockDeclarationSystem` drains the buffer.
  - `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs` -- read `InWorldDays` for proposal and marriage timestamping.
  - `unity/Assets/_Bloodlines/Code/Dynasties/MarriageComponents.cs` -- write to `LesserHouseElement` buffer on the AI faction (sub-slice 13). The buffer element struct itself is not modified; new entries seeded by `AILesserHousePromotionSystem` carry FounderMemberId for cross-reference deduplication and `LastDriftAppliedInWorldDays` so the existing `LesserHouseLoyaltyDriftSystem` (tier2-batch lane, retired) picks them up next in-world day.
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
- Browser Reference:
  - Sub-slice 1: `src/game/core/ai.js` `pickTerritoryTarget` (~747), `pickScoutHarassTarget` (~412), `getWorldPressureRaidTarget` (~817)
  - Sub-slice 2: `src/game/core/ai.js` timer clamp/floor block lines 1127-1241
  - Sub-slice 3: `src/game/core/ai.js` governance/event-context timer clamp block lines 1129-1215
  - Sub-slice 4: `src/game/core/ai.js` idle worker dispatch loop lines 1243-1251, getEnemyGatherPriorities (885-922), chooseGatherNode (924-933)
  - Sub-slice 5: `src/game/core/ai.js` attackTimer<=0 siege staging decision tree ~lines 1825-2090, areSiegeLinesFormed (947), getSiegeStagePoint (935), isReliefArmyApproaching (727)
  - Sub-slice 6: `src/game/core/ai.js` updateEnemyAi dynasty/covert ops dispatch block ~lines 2419-2678 (assassination, missionary, holy war, divine right, captive recovery, marriage proposal/inbox, non-aggression pact, lesser-house promotion)
  - Sub-slice 7: `src/game/core/ai.js` updateEnemyAi buildTimer<=0 block ~lines 1377-1573 (13-step priority chain: barracks, wayshrine, quarry, iron mine, siege workshop, covenant hall, grand sanctuary, apex covenant, supply camp, stable, dwelling, farm, well); timer reset at 4s/6s depending on playerKeepFortified
  - Sub-slice 8 (Codex command dispatch): `src/game/core/ai.js` idle worker `issueGatherCommand` dispatch (~1243-1260), territory expansion command dispatch (~1575-1600)
  - Sub-slice 8: `src/game/core/ai.js` `tryAiMarriageProposal` (~2897-2944) plus updateEnemyAi dispatch hook (~2616-2624); simulation-side sink `proposeMarriage` (~7340); expiration at 30 in-world days delegated to existing `MarriageProposalExpirationSystem`
  - Sub-slice 9: `src/game/core/ai.js` `tryAiAcceptIncomingMarriage` (~2880-2895) plus updateEnemyAi dispatch hook (~2632-2636); simulation-side sink `acceptMarriage` (~7388-7469); gestation at 60 in-world days delegated to existing `MarriageGestationSystem` which only processes `IsPrimary == true` records
  - Sub-slice 10: `src/game/core/ai.js` `getAiMarriageStrategicProfile` (~2803-2839); simplified port of `simulation.js getMarriageFaithCompatibilityProfile` (~596-730) using SelectedFaith+DoctrinePath equality rather than covenantName grouping (Unity has no covenant-name covariance yet); populates `AICovertOpsComponent.MarriageProposalGateMet` and `MarriageInboxAcceptGate` so sub-slices 6/8/9 gate on browser-accurate 4-signal strategic profile
  - Sub-slice 11: `src/game/core/simulation.js` `acceptMarriage` (~7388-7469) post-record effects block; ports legitimacy +2 clamped to 100 both sides, hostility drop both ways, oathkeeping conviction +2 both sides via `ConvictionScoring.ApplyEvent`, and 30-day `DeclareInWorldTimeRequest` jump via the existing DualClock request buffer; uses new `MarriageAcceptEffectsPendingTag` attached in sub-slice 9 at primary marriage creation for one-shot application
  - Sub-slice 12: `src/game/core/simulation.js` `getMarriageAcceptanceTerms` (~6327), `applyMarriageGovernanceLegitimacyCost` (~6232), `getMarriageAuthorityProfile` (~6134), and `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (~6091); `acceptMarriage` cost-before-bonus order at simulation.js:7449 (cost) vs simulation.js:7458 (legitimacy +2); ports head-direct (cost 0) / heir-regency (cost 1) / envoy-regency (cost 2) and the no-authority rejection; Stewardship -cost conviction event via the same `ConvictionScoring.ApplyEvent` helper used by oathkeeping
  - Sub-slice 13: `src/game/core/ai.js` `tryAiPromoteLesserHouse` (~2784-2801) plus updateEnemyAi dispatch hook (~2674-2677); simulation-side sink `promoteMemberToLesserHouse` (~7184-7258) ported at the mechanical level Unity tracks; constants block at simulation.js (~6444-6457): LESSER_HOUSE_RENOWN_THRESHOLD = 30, LESSER_HOUSE_MIN_PROMOTIONS = 1 (deferred), LESSER_HOUSE_LEGITIMACY_BONUS = 3, LESSER_HOUSE_INITIAL_LOYALTY = 75, LESSER_HOUSE_QUALIFYING_PATHS = {Governance, MilitaryCommand, CovertOperations}; `memberIsLesserHouseCandidate` (~6469-6479) gates ported (renown, role, status, path; foundedLesserHouseId replaced by cross-reference of LesserHouseElement.FounderMemberId since DynastyMemberComponent has no foundedLesserHouseId field); strategic gates: legitimacy &lt; 90, lesser-house buffer count &lt; 3
  - Sub-slices pending: narrative message push (no AI->UI message component yet); per-member FoundedLesserHouseId on DynastyMemberComponent (cross-reference workaround in place); promotion-history gate; marital-anchor and cadet world-pressure profiles on lesser houses; and dispatch-execution sub-slices for assassination, missionary, holy war, divine right, captive recovery, and pact proposal CovertOpKind values
- Current Branch In Flight: `claude/unity-ai-lesser-house-promotion`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-19-unity-ai-strategic-layer-sub-slice-13-lesser-house-promotion.md`

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

- Status: active
- Branch Prefix: `codex/unity-fortification-siege`
- Owner Agent: codex
- Owned Paths (exclusive):
  - `unity/Assets/_Bloodlines/Code/Fortification/**`
  - `unity/Assets/_Bloodlines/Code/Siege/**`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupportComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/FieldWaterComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/SiegeSupplyTrainComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/ImminentEngagementComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs`
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Siege.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesImminentEngagementSmokeValidation.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSiegeSupplyInterdictionSmokeValidation.cs`
- Owned Scripts:
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityImminentEngagementSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSupplyInterdictionSmokeValidation.ps1`
- Shared-File Narrow Edits Applied:
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` -- additive wrapper update preserved fortification smoke ownership while keeping the existing validation surface intact for the rebased imminent-engagement lane
- Lane Authority Documents:
  - `docs/unity/session-handoffs/2026-04-17-unity-fortification-siege-fortification-tier-and-reserves.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-siege-support-and-field-water.md`
  - `docs/unity/session-handoffs/2026-04-18-unity-fortification-siege-imminent-engagement-warnings.md`
  - `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-camp-supply-interdiction.md`
- Current Branch In Flight: `codex/unity-fortification-siege-camp-supply-interdiction`
- Last Slice Handoff: `docs/unity/session-handoffs/2026-04-19-unity-fortification-siege-camp-supply-interdiction.md`

## Next Unblocked Tier 1 Lanes (Unclaimed)

Forward work is prioritized in the browser-to-Unity migration plan at `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`. The items below are unblocked and unclaimed. Any agent resuming a session may claim one by adding an entry under Active Lanes above, bumping Revision, and proceeding.

Note: `fortification-siege-sub-slice-4-siege-camp-supply-interdiction` is implemented on `codex/unity-fortification-siege-camp-supply-interdiction` and documented in this revision. The next fortification follow-up should claim a fresh `codex/unity-fortification-*` branch rather than widening the earlier fortification branches.

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

- Suggested Branch: new branch `claude/unity-ai-narrative-message-bridge` (or similar) once an AI-to-UI message surface is in scope.
- Target Paths: TBD; needs a message component or buffer on the player faction (or a singleton) that AI systems can push narrative entries onto. Sub-slices 11, 12, and 13 all deferred this because no Unity message bridge exists yet.
- Browser Reference: `src/game/core/simulation.js` `pushMessage` (search), called from `acceptMarriage` (~7463), `promoteMemberToLesserHouse` (~7251), and many other AI paths.
- Scope: design and add a minimal AI-to-UI narrative message channel (component or buffer), then push the marriage-accept, marriage-acceptance-terms, and lesser-house-founded ceremonial messages via that channel. Likely paired with a dedicated lane that also covers other AI-pushed narrative pushes (proposal sent, expiration, child birth, etc.) so the channel is not single-purpose.

### Next Lane Candidate: ai-strategic-layer-sub-slice-15-pact-proposal-execution

- Suggested Branch: new branch `claude/unity-ai-pact-proposal-execution`.
- Target Paths: `unity/Assets/_Bloodlines/Code/AI/**` (additive, owned by ai-strategic-layer lane); a new `PactProposalComponent` likely needed; reads across Dynasty / Faction state for gating.
- Browser Reference: `src/game/core/ai.js` updateEnemyAi pact proposal block (~2643-2665) plus the simulation-side `proposeNonAggressionPact` sink (search). Mirrors the marriage proposal pattern from sub-slice 8.
- Scope: consume `AICovertOpsComponent.LastFiredOp == PactProposal` from sub-slice 6 and create a `PactProposalComponent` entity with proposal metadata. Add expiration and accept paths in subsequent sub-slices following the marriage-proposal lifecycle (sub-slices 8/9/11/12).

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
