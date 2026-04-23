using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Victory
{
    /// <summary>
    /// Evaluates all active victory conditions each tick and writes the result to the
    /// VictoryStateComponent singleton. Once Status != Playing the system returns early
    /// on every subsequent tick.
    ///
    /// Condition 1 -- Command Hall Fall (browser: simulation.js ~7821)
    ///   Any command_hall entity carrying DeadTag triggers immediate resolution.
    ///   Player command hall dead -> Lost. Enemy command hall dead -> Won.
    ///
    /// Condition 2 -- Territorial Governance (browser: simulation.js ~1545, 1723, 1814)
    ///   TerritorialGovernanceRecognitionComponent owns the live acceptance,
    ///   integration, and victory-hold state. Once the recognition completes or the
    ///   acceptance-gated victory hold reaches 120 seconds, player victory resolves.
    ///   If the recognition surface is absent, the older loyalty-only fallback remains.
    ///
    /// Condition 3 -- Divine Right (browser: simulation.js ~10738, const line 9782)
    ///   A faction with FaithStateComponent.Level >= 5 and Intensity >=
    ///   DIVINE_RIGHT_INTENSITY_THRESHOLD (80) triggers divine right recognition.
    ///   Player faction -> Won. Enemy faction -> Lost.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct VictoryConditionEvaluationSystem : ISystem
    {
        public const float TerritorialGovernanceLoyaltyThreshold = 90f;
        public const float TerritorialGovernanceVictorySeconds   = 120f;
        public const int   DivinRightFaithLevel                  = 5;
        public const float DivinRightIntensityThreshold          = 80f;

        private static readonly FixedString32Bytes  PlayerFactionId   = new FixedString32Bytes("player");
        private static readonly FixedString64Bytes  CommandHallTypeId = new FixedString64Bytes("command_hall");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VictoryStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var victory = SystemAPI.GetSingleton<VictoryStateComponent>();
            if (victory.Status != MatchStatus.Playing)
                return;

            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f) dt = 0.016f;

            // --- Condition 1: Command Hall Fall ---
            var deadBuildingQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DeadTag>());
            var buildingTypes   = deadBuildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            var buildingFactions = deadBuildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            deadBuildingQuery.Dispose();

            for (int i = 0; i < buildingTypes.Length; i++)
            {
                if (!buildingTypes[i].TypeId.Equals(CommandHallTypeId))
                    continue;

                bool isPlayer = buildingFactions[i].FactionId.Equals(PlayerFactionId);
                victory.Status       = isPlayer ? MatchStatus.Lost : MatchStatus.Won;
                victory.VictoryType  = VictoryConditionId.CommandHallFall;
                victory.WinnerFactionId = isPlayer
                    ? buildingFactions[i].FactionId
                    : PlayerFactionId;
                victory.VictoryReason = isPlayer
                    ? new FixedString128Bytes("Player Command Hall destroyed.")
                    : new FixedString128Bytes("Enemy Command Hall destroyed.");

                buildingTypes.Dispose();
                buildingFactions.Dispose();
                SystemAPI.SetSingleton(victory);
                return;
            }
            buildingTypes.Dispose();
            buildingFactions.Dispose();

            // --- Condition 2: Territorial Governance ---
            bool usedRecognitionPath = false;
            var governanceQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TerritorialGovernanceRecognitionComponent>());
            if (!governanceQuery.IsEmptyIgnoreFilter)
            {
                var governanceFactions =
                    governanceQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var governanceStates =
                    governanceQuery.ToComponentDataArray<TerritorialGovernanceRecognitionComponent>(Allocator.Temp);
                governanceQuery.Dispose();

                for (int i = 0; i < governanceFactions.Length; i++)
                {
                    if (!governanceFactions[i].FactionId.Equals(PlayerFactionId))
                    {
                        continue;
                    }

                    usedRecognitionPath = true;
                    victory.TerritorialGovernanceHoldSeconds = governanceStates[i].VictoryHoldSeconds;
                    if (governanceStates[i].Completed ||
                        governanceStates[i].VictoryHoldSeconds >= TerritorialGovernanceVictorySeconds)
                    {
                        victory.Status = MatchStatus.Won;
                        victory.VictoryType = VictoryConditionId.TerritorialGovernance;
                        victory.WinnerFactionId = PlayerFactionId;
                        victory.VictoryReason =
                            new FixedString128Bytes("Territorial Governance victory.");
                        governanceFactions.Dispose();
                        governanceStates.Dispose();
                        SystemAPI.SetSingleton(victory);
                        return;
                    }

                    break;
                }

                governanceFactions.Dispose();
                governanceStates.Dispose();
            }

            if (!usedRecognitionPath)
            {
                var cpQuery = em.CreateEntityQuery(
                    ComponentType.ReadOnly<ControlPointComponent>(),
                    ComponentType.ReadOnly<FactionComponent>());
                var cpFactions = cpQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                var cpComponents = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
                cpQuery.Dispose();

                int playerHeld = 0;
                int playerLoyal = 0;
                for (int i = 0; i < cpComponents.Length; i++)
                {
                    if (!cpFactions[i].FactionId.Equals(PlayerFactionId))
                        continue;
                    playerHeld++;
                    if (cpComponents[i].Loyalty >= TerritorialGovernanceLoyaltyThreshold)
                        playerLoyal++;
                }
                cpComponents.Dispose();
                cpFactions.Dispose();

                if (playerHeld > 0 && playerLoyal == playerHeld)
                {
                    victory.TerritorialGovernanceHoldSeconds += dt;
                    if (victory.TerritorialGovernanceHoldSeconds >= TerritorialGovernanceVictorySeconds)
                    {
                        victory.Status = MatchStatus.Won;
                        victory.VictoryType = VictoryConditionId.TerritorialGovernance;
                        victory.WinnerFactionId = PlayerFactionId;
                        victory.VictoryReason = new FixedString128Bytes("Territorial Governance victory.");
                        SystemAPI.SetSingleton(victory);
                        return;
                    }
                }
                else
                {
                    victory.TerritorialGovernanceHoldSeconds = 0f;
                }
            }

            // --- Condition 3: Divine Right ---
            var faithQuery   = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            var faithFactions = faithQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var faithStates   = faithQuery.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);
            faithQuery.Dispose();

            for (int i = 0; i < faithFactions.Length; i++)
            {
                if (faithStates[i].Level < DivinRightFaithLevel)
                    continue;
                if (faithStates[i].Intensity < DivinRightIntensityThreshold)
                    continue;

                bool isPlayer = faithFactions[i].FactionId.Equals(PlayerFactionId);
                victory.Status       = isPlayer ? MatchStatus.Won : MatchStatus.Lost;
                victory.VictoryType  = VictoryConditionId.DivinRight;
                victory.WinnerFactionId = faithFactions[i].FactionId;
                victory.VictoryReason   = new FixedString128Bytes("Divine Right recognized.");
                faithFactions.Dispose();
                faithStates.Dispose();
                SystemAPI.SetSingleton(victory);
                return;
            }
            faithFactions.Dispose();
            faithStates.Dispose();

            SystemAPI.SetSingleton(victory);
        }
    }
}
