using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Evaluates the browser's getAiMarriageStrategicProfile (ai.js ~2803-2839)
    /// each frame and writes the two marriage gate flags into
    /// AICovertOpsComponent so AICovertOpsSystem can dispatch proposal /
    /// inbox-accept only when strategically justified.
    ///
    /// The profile combines four pressure signals and a faith-compatibility
    /// tier into a "willing" decision:
    ///   signalCount = isHostile + populationDeficit
    ///               + (legitimacyDistress AND faithAllowsLegitimacySignal)
    ///               + (ownSuccessionCrisis OR rivalSuccessionCrisis)
    ///   blockedByFaith = faithBlocksWeakMatch AND signalCount lt 2
    ///   willing = signalCount gt 0 AND NOT blockedByFaith
    ///
    /// Both MarriageProposalGateMet and MarriageInboxAcceptGate are set to
    /// `willing`. The browser comment on ai.js line 2630-2631 states explicitly
    /// that the accept gate "reuses the strategic criteria from
    /// tryAiMarriageProposal so AI behavior is symmetric".
    ///
    /// Runs before AICovertOpsSystem so the gate flags are fresh when
    /// dispatch fires. Does not modify any other context flags.
    ///
    /// Scope:
    /// - Inputs resolved from existing Unity components:
    ///     HostilityComponent buffer on source faction (for isHostile check)
    ///     PopulationComponent on both factions (for population deficit)
    ///     DynastyStateComponent on source (for legitimacy distress)
    ///     AIStrategyComponent on source (for succession crisis flags already
    ///       seeded by sub-slices 1-3)
    ///     FaithStateComponent on both factions (for compatibility tier)
    /// - Target faction hardcoded to "player" matching the browser's
    ///   getAiMarriageStrategicProfile signature which only reads
    ///   state.factions.enemy and state.factions.player.
    ///
    /// Browser reference: ai.js getAiMarriageStrategicProfile (~2803-2839),
    /// simulation.js getMarriageFaithCompatibilityProfile (~596-730). The
    /// faith-compatibility tier is simplified: Unity lacks covenantName
    /// grouping, so the port uses SelectedFaith + DoctrinePath equality to
    /// derive harmonious / sectarian / incompatible.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AICovertOpsSystem))]
    public partial struct AIMarriageStrategicProfileSystem : ISystem
    {
        // Matches browser AI_MARRIAGE_LEGITIMACY_DISTRESS_THRESHOLD = 50 (ai.js:44).
        private const float LegitimacyDistressThreshold = 50f;

        // Matches browser populationDeficit = enemyPop < playerPop * 0.85 (ai.js:2809).
        private const float PopulationDeficitFactor = 0.85f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AICovertOpsComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Resolve the "player" faction once per tick. All AI factions in this
            // match share the same target per browser convention.
            var targetFactionId = new FixedString32Bytes("player");
            if (!TryResolvePlayerSnapshot(em, targetFactionId,
                    out int playerPop, out CovenantId playerFaith, out DoctrinePath playerDoctrine))
            {
                return;
            }

            // Iterate every AI faction with covert ops + strategy components.
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AICovertOpsComponent>(),
                ComponentType.ReadOnly<AIStrategyComponent>());
            var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var aiEntity = entities[i];
                var faction  = em.GetComponentData<FactionComponent>(aiEntity);

                // Only evaluate non-player factions.
                if (faction.FactionId.Equals(targetFactionId)) continue;

                var covert   = em.GetComponentData<AICovertOpsComponent>(aiEntity);
                var strategy = em.GetComponentData<AIStrategyComponent>(aiEntity);

                bool willing = EvaluateWilling(em, aiEntity, faction.FactionId,
                    strategy, playerPop, playerFaith, playerDoctrine);

                covert.MarriageProposalGateMet = willing;
                covert.MarriageInboxAcceptGate = willing;
                em.SetComponentData(aiEntity, covert);
            }

            entities.Dispose();
        }

        // ------------------------------------------------------------------ core evaluation

        private static bool EvaluateWilling(
            EntityManager em,
            Entity aiFactionEntity,
            FixedString32Bytes aiFactionId,
            AIStrategyComponent strategy,
            int playerPop,
            CovenantId playerFaith,
            DoctrinePath playerDoctrine)
        {
            // Signal 1: hostility toward player.
            bool isHostile = IsHostileToPlayer(em, aiFactionEntity);

            // Signal 2: population deficit.
            int aiPop = em.HasComponent<PopulationComponent>(aiFactionEntity)
                ? em.GetComponentData<PopulationComponent>(aiFactionEntity).Total
                : 0;
            bool populationDeficit = aiPop > 0 && (float)aiPop < playerPop * PopulationDeficitFactor;

            // Signal 3: legitimacy distress, gated by faith compatibility tier.
            float legitimacy = em.HasComponent<DynastyStateComponent>(aiFactionEntity)
                ? em.GetComponentData<DynastyStateComponent>(aiFactionEntity).Legitimacy
                : 100f;
            bool legitimacyDistress = legitimacy < LegitimacyDistressThreshold;

            CovenantId aiFaith      = CovenantId.None;
            DoctrinePath aiDoctrine = DoctrinePath.Unassigned;
            if (em.HasComponent<FaithStateComponent>(aiFactionEntity))
            {
                var f = em.GetComponentData<FaithStateComponent>(aiFactionEntity);
                aiFaith    = f.SelectedFaith;
                aiDoctrine = f.DoctrinePath;
            }
            var compat = EvaluateFaithCompatibility(aiFaith, aiDoctrine, playerFaith, playerDoctrine);

            bool faithBackedLegitimacySignal = legitimacyDistress && compat.LegitimacySignalAllowed;

            // Signal 4: succession pressure on either side. Sub-slice 3 populates
            // EnemySuccessionCrisisActive for this faction and
            // PlayerSuccessionCrisisActive for the opposing faction.
            bool successionPressure =
                strategy.EnemySuccessionCrisisActive || strategy.PlayerSuccessionCrisisActive;

            int signalCount =
                (isHostile               ? 1 : 0) +
                (populationDeficit       ? 1 : 0) +
                (faithBackedLegitimacySignal ? 1 : 0) +
                (successionPressure      ? 1 : 0);

            bool blockedByFaith = compat.BlocksWeakMatch && signalCount < 2;

            return signalCount > 0 && !blockedByFaith;
        }

        // ------------------------------------------------------------------ hostility

        private static bool IsHostileToPlayer(EntityManager em, Entity aiFactionEntity)
        {
            if (!em.HasBuffer<HostilityComponent>(aiFactionEntity)) return false;
            var buffer = em.GetBuffer<HostilityComponent>(aiFactionEntity);
            var playerId = new FixedString32Bytes("player");
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].HostileFactionId.Equals(playerId)) return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ player snapshot

        private static bool TryResolvePlayerSnapshot(
            EntityManager em,
            FixedString32Bytes targetFactionId,
            out int playerPop,
            out CovenantId playerFaith,
            out DoctrinePath playerDoctrine)
        {
            playerPop      = 0;
            playerFaith    = CovenantId.None;
            playerDoctrine = DoctrinePath.Unassigned;

            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity playerEntity = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(targetFactionId))
                {
                    playerEntity = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();

            if (playerEntity == Entity.Null) return false;

            if (em.HasComponent<PopulationComponent>(playerEntity))
                playerPop = em.GetComponentData<PopulationComponent>(playerEntity).Total;

            if (em.HasComponent<FaithStateComponent>(playerEntity))
            {
                var f = em.GetComponentData<FaithStateComponent>(playerEntity);
                playerFaith    = f.SelectedFaith;
                playerDoctrine = f.DoctrinePath;
            }

            return true;
        }

        // ------------------------------------------------------------------ faith compatibility

        /// <summary>
        /// Simplified port of simulation.js getMarriageFaithCompatibilityProfile.
        /// Full browser version uses covenantName grouping that Unity lacks, so
        /// the port derives compatibility tier from SelectedFaith + DoctrinePath
        /// equality. Preserves the mechanical intent: harmonious unlocks
        /// legitimacy-repair signal; fully different faith+doctrine blocks weak
        /// matches; unbound (either side uncommitted) is neutral.
        /// </summary>
        private static FaithCompatibility EvaluateFaithCompatibility(
            CovenantId sourceFaith,
            DoctrinePath sourceDoctrine,
            CovenantId targetFaith,
            DoctrinePath targetDoctrine)
        {
            // Unbound: either side has not committed to a covenant.
            if (sourceFaith == CovenantId.None || targetFaith == CovenantId.None
                || sourceDoctrine == DoctrinePath.Unassigned
                || targetDoctrine == DoctrinePath.Unassigned)
            {
                return new FaithCompatibility
                {
                    LegitimacySignalAllowed = false,
                    BlocksWeakMatch         = false,
                };
            }

            bool sameFaith    = sourceFaith == targetFaith;
            bool sameDoctrine = sourceDoctrine == targetDoctrine;

            // Harmonious: identical covenant and doctrine. Legitimacy-repair path unlocked.
            if (sameFaith && sameDoctrine)
            {
                return new FaithCompatibility
                {
                    LegitimacySignalAllowed = true,
                    BlocksWeakMatch         = false,
                };
            }

            // Sectarian (same faith, different doctrine) or ecumenical (same doctrine,
            // different faith). Both allow legitimacy signal but neither blocks a
            // weak match.
            if (sameFaith || sameDoctrine)
            {
                return new FaithCompatibility
                {
                    LegitimacySignalAllowed = true,
                    BlocksWeakMatch         = false,
                };
            }

            // Incompatible: different faith AND different doctrine. Blocks weak matches.
            return new FaithCompatibility
            {
                LegitimacySignalAllowed = false,
                BlocksWeakMatch         = true,
            };
        }

        private struct FaithCompatibility
        {
            public bool LegitimacySignalAllowed;
            public bool BlocksWeakMatch;
        }
    }
}
