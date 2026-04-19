using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Consumes AICovertOpsComponent.LastFiredOp == CovertOpKind.DivineRight
    /// (written by AICovertOpsSystem in sub-slice 6) and executes the
    /// browser-side startDivineRightDeclaration path
    /// (simulation.js:10784-10835). Third consumer of the sub-slice 18
    /// dynasty-operation foundation.
    ///
    /// Browser dispatch site: ai.js divine right dispatch ~2553-2564,
    /// hardcoded to source "enemy" with no target argument (divine
    /// right is a unilateral source-side declaration; affected
    /// factions are derived at resolution time, not declaration time).
    ///
    /// Unity-side surface consistency departure from browser: the
    /// browser does not route divine right through
    /// DYNASTY_OPERATION_ACTIVE_LIMIT (startDivineRightDeclaration
    /// never calls getDynastyOperationsState; declaration writes
    /// directly to faction.faith.divineRightDeclaration). Unity DOES
    /// create a DynastyOperationComponent entity with
    /// Kind=DivineRight so future intel-report and resolution-system
    /// queries can use one shape for all dispatch consumers. The
    /// browser per-faction one-active-at-a-time semantic is preserved
    /// by the explicit "no existing active divine right operation"
    /// gate before the capacity check.
    ///
    /// Gates ported from getDivineRightDeclarationTerms (~10604-10653)
    /// in their simplified Unity form:
    ///   1. Source has a committed faith (FaithStateComponent.SelectedFaith
    ///      != CovenantId.None). Browser equivalent: profile.selectedFaithId
    ///      check at simulation.js:10611.
    ///   2. Source faith intensity >= DIVINE_RIGHT_INTENSITY_THRESHOLD
    ///      (80 at simulation.js:9782). Browser combined gate:
    ///      simulation.js:956 ("intensityReady").
    ///   3. Source faith level >= 5. Browser combined gate at the same
    ///      line ("Level 5 covenant conviction").
    ///   4. No existing active DivineRight operation for this faction
    ///      (Active=true DynastyOperationComponent with
    ///      OperationKind=DivineRight and matching SourceFactionId).
    ///      Browser equivalent: getActiveDivineRightDeclaration check
    ///      at simulation.js:10623.
    ///
    /// Capacity gate: DynastyOperationLimits.HasCapacity must return
    /// true for the source faction (sub-slice 18 capacity surface).
    /// Per the Unity-side departure noted above, this gate is checked
    /// even though browser does not enforce DYNASTY_OPERATION_ACTIVE_LIMIT
    /// for divine right.
    ///
    /// Gates intentionally deferred (Unity has no canonical surface
    /// yet for these browser checks):
    ///   - Covenant Test passed gate (browser
    ///     ensureFaithCovenantTestCompletionFromLegacyState at
    ///     simulation.js:10606 + faithState.covenantTestPassed at
    ///     :10614). Unity covenant-test execution lane is not yet
    ///     ported.
    ///   - Cooldown gate (browser profile.cooldownRemaining at
    ///     simulation.js:10626). Unity divine-right cooldown surface
    ///     not yet ported; relies on the existing-declaration gate
    ///     above to prevent stacking during the declaration window.
    ///   - Stage-ready gate (browser profile.stageReady at
    ///     simulation.js:10629). Final Convergence threshold has no
    ///     Unity equivalent yet.
    ///   - Active apex structure gate (browser
    ///     profile.activeApexStructureReady at simulation.js:10635).
    ///     Apex structure surface not yet ported; the per-kind
    ///     ActiveApexStructureId / Name fields default to empty.
    ///   - Recognition share gate (browser profile.recognitionReady
    ///     at simulation.js:10638). Global recognition share calculator
    ///     not yet ported; per-kind RecognitionShare /
    ///     RecognitionSharePct fields default to 0.
    ///   - Faction kind == kingdom gate (browser at simulation.js:10607).
    ///     Defensible to add as a Unity-side gate via FactionKindComponent;
    ///     omitted for this slice because the AI dispatch hook only
    ///     fires for the enemy faction which is canonically a kingdom.
    ///     A future hardening pass can add it without reshape.
    ///
    /// Effects on success:
    ///   - Call DynastyOperationLimits.BeginOperation with
    ///     DynastyOperationKind.DivineRight and default target (divine
    ///     right has no specific target faction; affected factions
    ///     are derived at resolution time).
    ///   - Attach DynastyOperationDivineRightComponent to the created
    ///     entity carrying the per-kind fields (ResolveAtInWorldDays,
    ///     SourceFaithId, DoctrinePath, recognition placeholders, apex
    ///     structure placeholders).
    ///   - Push narrative message via NarrativeMessageBridge.Push
    ///     mirroring simulation.js:10829-10833 ("declares Divine Right
    ///     under X. The spread window opens for 180s..."). Tone is
    ///     always Warn per browser (simulation.js:10832 hardcodes
    ///     warn for both source==player and target paths).
    ///
    /// Effects intentionally deferred (require future resolution slice):
    ///   - Mutual hostility application against all non-same-faith
    ///     kingdoms (browser ensureMutualHostility at simulation.js:10819).
    ///   - AI timer cap propagation to candidate factions (browser
    ///     attackTimer / territoryTimer / raidTimer / missionaryTimer /
    ///     holyWarTimer caps at simulation.js:10822-10826).
    ///   - Conviction event recording (browser recordConvictionEvent
    ///     at simulation.js:10806; oathkeeping for light, desecration
    ///     for dark, +3 either way).
    ///   - Resolution at ResolveAtInWorldDays (apex faith claim on
    ///     success vs failDivineRightDeclaration cooldown + legitimacy
    ///     penalty on failure).
    ///
    /// Always clears LastFiredOp to None after processing regardless of
    /// outcome so one dispatch produces one execution attempt, matching
    /// the one-shot pattern shared with sub-slices 8/9/12/13/14/20/21.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AIDivineRightExecutionSystem : ISystem
    {
        public const float DivineRightIntensityThreshold     = 80f;
        public const int   DivineRightLevelThreshold         = 5;
        public const float DivineRightDeclarationDurationInWorldDays = 180f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var dispatchQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>());
            var dispatchEntities = dispatchQuery.ToEntityArray(Allocator.Temp);
            dispatchQuery.Dispose();

            for (int i = 0; i < dispatchEntities.Length; i++)
            {
                var sourceEntity = dispatchEntities[i];
                var covert = em.GetComponentData<AICovertOpsComponent>(sourceEntity);
                if (covert.LastFiredOp != CovertOpKind.DivineRight)
                    continue;

                var sourceFaction = em.GetComponentData<FactionComponent>(sourceEntity);

                TryDispatchDivineRight(em, sourceEntity, sourceFaction.FactionId, inWorldDays);

                covert.LastFiredOp = CovertOpKind.None;
                em.SetComponentData(sourceEntity, covert);
            }

            dispatchEntities.Dispose();
        }

        // ------------------------------------------------------------------ dispatch

        private static void TryDispatchDivineRight(
            EntityManager em,
            Entity sourceEntity,
            FixedString32Bytes sourceFactionId,
            float inWorldDays)
        {
            // Gate 1: source must have a committed faith.
            if (!em.HasComponent<FaithStateComponent>(sourceEntity)) return;
            var sourceFaith = em.GetComponentData<FaithStateComponent>(sourceEntity);
            if (sourceFaith.SelectedFaith == CovenantId.None) return;

            // Gate 2: intensity must meet the apex threshold.
            if (sourceFaith.Intensity < DivineRightIntensityThreshold) return;

            // Gate 3: faith level must be 5+.
            if (sourceFaith.Level < DivineRightLevelThreshold) return;

            // Gate 4: no existing active divine right operation for this faction.
            if (HasActiveDivineRightOperation(em, sourceFactionId)) return;

            // Capacity gate (sub-slice 18). Browser does not enforce this
            // for divine right; Unity adds it for surface consistency.
            if (!DynastyOperationLimits.HasCapacity(em, sourceFactionId)) return;

            // ------------------------------------------------------------------ effects

            var operationId = BuildOperationId(sourceFactionId, inWorldDays);
            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId,
                sourceFactionId,
                DynastyOperationKind.DivineRight,
                default,
                default);

            em.AddComponentData(entity, new DynastyOperationDivineRightComponent
            {
                ResolveAtInWorldDays    = inWorldDays + DivineRightDeclarationDurationInWorldDays,
                SourceFaithId           = SelectedFaithIdString(sourceFaith.SelectedFaith),
                DoctrinePath            = sourceFaith.DoctrinePath,
                RecognitionShare        = 0f,
                RecognitionSharePct     = 0f,
                ActiveApexStructureId   = default,
                ActiveApexStructureName = default,
            });

            PushDivineRightDeclarationMessage(em, sourceFactionId,
                SelectedFaithIdString(sourceFaith.SelectedFaith));
        }

        // ------------------------------------------------------------------ narrative

        private static void PushDivineRightDeclarationMessage(
            EntityManager em,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes sourceFaithId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" declares Divine Right under ");
            message.Append(sourceFaithId);
            message.Append((FixedString64Bytes)". The spread window opens for ");
            message.Append((int)DivineRightDeclarationDurationInWorldDays);
            message.Append((FixedString32Bytes)" in-world days.");

            // Browser tone routing simulation.js:10832: always warn.
            NarrativeMessageBridge.Push(em, message, NarrativeMessageTone.Warn);
        }

        // ------------------------------------------------------------------ helpers

        private static bool HasActiveDivineRightOperation(
            EntityManager em,
            FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            if (q.IsEmpty) { q.Dispose(); return false; }

            var arr = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            q.Dispose();

            bool found = false;
            for (int i = 0; i < arr.Length; i++)
            {
                var op = arr[i];
                if (!op.Active) continue;
                if (op.OperationKind != DynastyOperationKind.DivineRight) continue;
                if (!op.SourceFactionId.Equals(factionId)) continue;
                found = true;
                break;
            }
            arr.Dispose();
            return found;
        }

        private static FixedString64Bytes SelectedFaithIdString(CovenantId selected)
        {
            switch (selected)
            {
                case CovenantId.OldLight:      return new FixedString64Bytes("old_light");
                case CovenantId.BloodDominion: return new FixedString64Bytes("blood_dominion");
                case CovenantId.TheOrder:      return new FixedString64Bytes("the_order");
                case CovenantId.TheWild:       return new FixedString64Bytes("the_wild");
                default:                       return new FixedString64Bytes("none");
            }
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("dynop-divineright-");
            id.Append(sourceFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
