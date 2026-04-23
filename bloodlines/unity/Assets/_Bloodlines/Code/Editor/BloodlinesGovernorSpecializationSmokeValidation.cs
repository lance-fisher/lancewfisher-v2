#if UNITY_EDITOR
using System;
using Bloodlines.Components;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesGovernorSpecializationSmokeValidation
    {
        [MenuItem("Bloodlines/Validation/Run Governor Specialization Smoke")]
        public static void RunMenu()
        {
            RunInternal(batchMode: false);
        }

        public static string RunBatchGovernorSpecializationSmokeValidation()
        {
            return RunInternal(batchMode: true);
        }

        private static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                string phase1 = RunAssignmentPhase();
                string phase2 = RunConsumerPhase();
                string phase3 = RunNoGovernorPhase();
                string summary =
                    $"BLOODLINES_GOVERNOR_SPECIALIZATION_SMOKE PASS {phase1}; {phase2}; {phase3}";
                UnityDebug.Log(summary);
                return summary;
            }
            catch (Exception ex)
            {
                exitCode = 1;
                string summary = "BLOODLINES_GOVERNOR_SPECIALIZATION_SMOKE FAIL " + ex;
                UnityDebug.LogError(summary);
                return summary;
            }
            finally
            {
                if (batchMode)
                {
                    EditorApplication.Exit(exitCode);
                }
            }
        }

        private static string RunAssignmentPhase()
        {
            using var world = CreateValidationWorld("GovernorAssignmentPhase");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 12f);
            Entity faction = SeedFaction(entityManager, "player");
            AddGovernanceMember(entityManager, faction, "player-governor", DynastyRole.Governor, 36f);
            AddGovernanceMember(entityManager, faction, "player-merchant", DynastyRole.Merchant, 18f);
            Entity controlPoint = SeedControlPoint(entityManager, "border-cp", "player", "border_settlement", 4f);
            Entity settlement = SeedSettlement(
                entityManager,
                "primary-seat",
                "player",
                "primary_dynastic_keep",
                fortificationTier: 3,
                addReserve: true);

            Tick(world, 12f);

            if (!entityManager.HasComponent<GovernorSpecializationComponent>(controlPoint))
            {
                throw new InvalidOperationException("Phase 1 failed: no control-point specialization materialized.");
            }

            GovernorSpecializationComponent specialization =
                entityManager.GetComponentData<GovernorSpecializationComponent>(controlPoint);
            if (specialization.SpecializationId != GovernorSpecializationId.BorderMarshal ||
                !specialization.GovernorMemberId.Equals(new FixedString64Bytes("player-governor")))
            {
                throw new InvalidOperationException(
                    $"Phase 1 failed: expected Border Marshal on player-governor, got specialization={specialization.SpecializationId} member={specialization.GovernorMemberId}.");
            }

            if (!entityManager.HasComponent<GovernorSeatAssignmentComponent>(settlement))
            {
                throw new InvalidOperationException("Phase 1 failed: primary settlement did not receive a governor seat assignment.");
            }

            return $"phase1ControlPoint={specialization.SpecializationId},phase1Governor={specialization.GovernorMemberId}";
        }

        private static string RunConsumerPhase()
        {
            using var world = CreateValidationWorld("GovernorConsumerPhase");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 20f);

            Entity specializedFaction = SeedFaction(entityManager, "player");
            AddGovernanceMember(entityManager, specializedFaction, "player-governor", DynastyRole.Governor, 42f);
            AddGovernanceMember(entityManager, specializedFaction, "player-merchant", DynastyRole.Merchant, 24f);
            Entity specializedControlPoint =
                SeedControlPoint(entityManager, "city-cp", "player", "trade_town", 61f);
            Entity specializedKeep = SeedSettlement(
                entityManager,
                "keep-a",
                "player",
                "primary_dynastic_keep",
                fortificationTier: 3,
                addReserve: true);
            SeedFortificationDefender(entityManager, specializedKeep, "player", healthCurrent: 40f, healthMax: 100f);

            Entity plainFaction = SeedFaction(entityManager, "minor");
            Entity plainControlPoint =
                SeedControlPoint(entityManager, "plain-cp", "minor", "trade_town", 61f);
            Entity plainKeep = SeedSettlement(
                entityManager,
                "keep-b",
                "minor",
                "primary_dynastic_keep",
                fortificationTier: 3,
                addReserve: true);
            SeedFortificationDefender(entityManager, plainKeep, "minor", healthCurrent: 40f, healthMax: 100f);

            Tick(world, 20f);

            float specializedGoldBefore =
                entityManager.GetComponentData<ResourceStockpileComponent>(specializedFaction).Gold;
            float plainGoldBefore =
                entityManager.GetComponentData<ResourceStockpileComponent>(plainFaction).Gold;
            float specializedHealthBefore = GetLinkedDefenderHealth(entityManager, "player");
            float plainHealthBefore = GetLinkedDefenderHealth(entityManager, "minor");

            world.SetTime(new Unity.Core.TimeData(20.5f, 0.5f));
            world.Update();

            float specializedGoldAfter =
                entityManager.GetComponentData<ResourceStockpileComponent>(specializedFaction).Gold;
            float plainGoldAfter =
                entityManager.GetComponentData<ResourceStockpileComponent>(plainFaction).Gold;
            float specializedHealthAfter = GetLinkedDefenderHealth(entityManager, "player");
            float plainHealthAfter = GetLinkedDefenderHealth(entityManager, "minor");

            float specializedGain = specializedGoldAfter - specializedGoldBefore;
            float plainGain = plainGoldAfter - plainGoldBefore;
            float specializedHeal = specializedHealthAfter - specializedHealthBefore;
            float plainHeal = plainHealthAfter - plainHealthBefore;

            if (specializedGain <= plainGain ||
                specializedHeal <= plainHeal ||
                !entityManager.HasComponent<GovernorSpecializationComponent>(specializedControlPoint))
            {
                throw new InvalidOperationException(
                    $"Phase 2 failed: specialized gains did not exceed baseline. specializedGain={specializedGain:0.000} plainGain={plainGain:0.000} specializedHeal={specializedHeal:0.000} plainHeal={plainHeal:0.000}");
            }

            GovernorSpecializationComponent specialization =
                entityManager.GetComponentData<GovernorSpecializationComponent>(specializedControlPoint);
            GovernorSpecializationComponent keepProfile =
                GovernorSpecializationCanon.GetSettlementProfile(entityManager, specializedKeep);
            if (specialization.ResourceTrickleMultiplier <= 1f ||
                keepProfile.HealRegenMultiplier <= 1f ||
                specialization.CaptureResistanceBonus <= 1f)
            {
                throw new InvalidOperationException(
                    $"Phase 2 failed: multipliers were not non-unity. trickle={specialization.ResourceTrickleMultiplier:0.00} keepHeal={keepProfile.HealRegenMultiplier:0.00} capture={specialization.CaptureResistanceBonus:0.00}");
            }

            return $"phase2GoldGain={specializedGain:0.000}>{plainGain:0.000},phase2Heal={specializedHeal:0.000}>{plainHeal:0.000}";
        }

        private static string RunNoGovernorPhase()
        {
            using var world = CreateValidationWorld("GovernorNoGovernorPhase");
            var entityManager = world.EntityManager;

            SeedDualClock(entityManager, 35f);
            SeedFaction(entityManager, "player");
            SeedControlPoint(entityManager, "ungoverned-cp", "player", "trade_town", 68f);

            Tick(world, 35f);

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!controlPoints[i].ControlPointId.Equals(new FixedString32Bytes("ungoverned-cp")))
                {
                    continue;
                }

                if (entityManager.HasComponent<GovernorSpecializationComponent>(entities[i]))
                {
                    throw new InvalidOperationException("Phase 3 failed: specialization remained on an ungoverned control point.");
                }

                return "phase3Ungoverned=clear";
            }

            throw new InvalidOperationException("Phase 3 failed: control point not found.");
        }

        private static World CreateValidationWorld(string name)
        {
            var world = new World(name);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<GovernorSpecializationSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<ControlPointCaptureSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<ControlPointResourceTrickleSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationReserveSystem>());
            simulation.SortSystems();
            return world;
        }

        private static void SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            Entity clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
            });
        }

        private static Entity SeedFaction(EntityManager entityManager, string factionId)
        {
            Entity faction = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ResourceStockpileComponent));
            entityManager.SetComponentData(faction, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(faction, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            entityManager.SetComponentData(faction, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = 70f,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            entityManager.SetComponentData(faction, new ResourceStockpileComponent
            {
                Gold = 10f,
                Food = 10f,
                Water = 10f,
                Wood = 10f,
                Stone = 10f,
                Iron = 10f,
                Influence = 10f,
            });
            entityManager.AddBuffer<DynastyMemberRef>(faction);
            entityManager.AddBuffer<DynastyFallenLedger>(faction);
            return faction;
        }

        private static void AddGovernanceMember(
            EntityManager entityManager,
            Entity factionEntity,
            string memberId,
            DynastyRole role,
            float renown)
        {
            FixedString32Bytes factionId =
                entityManager.GetComponentData<FactionComponent>(factionEntity).FactionId;
            Entity member = entityManager.CreateEntity(typeof(FactionComponent), typeof(DynastyMemberComponent));
            entityManager.SetComponentData(member, new FactionComponent { FactionId = factionId });
            entityManager.SetComponentData(member, new DynastyMemberComponent
            {
                MemberId = new FixedString64Bytes(memberId),
                Title = new FixedString64Bytes(memberId),
                Role = role,
                Path = DynastyPath.Governance,
                AgeYears = 31f,
                Status = DynastyMemberStatus.Active,
                Renown = renown,
                Order = entityManager.GetBuffer<DynastyMemberRef>(factionEntity).Length,
                FallenAtWorldSeconds = -1f,
            });
            entityManager.GetBuffer<DynastyMemberRef>(factionEntity).Add(new DynastyMemberRef { Member = member });
        }

        private static Entity SeedControlPoint(
            EntityManager entityManager,
            string controlPointId,
            string ownerFactionId,
            string settlementClassId,
            float loyalty)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(ControlPointComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = new float3(0f, 0f, 0f),
            });
            entityManager.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes(controlPointId),
                OwnerFactionId = new FixedString32Bytes(ownerFactionId),
                CaptureFactionId = default,
                ContinentId = new FixedString32Bytes("west"),
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = loyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes(settlementClassId),
                FortificationTier = 2,
                RadiusTiles = 3f,
                CaptureTimeSeconds = 9f,
                GoldTrickle = 2f,
                FoodTrickle = 0.5f,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = 0f,
            });
            return entity;
        }

        private static Entity SeedSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            string settlementClassId,
            int fortificationTier,
            bool addReserve)
        {
            Entity entity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(SettlementComponent),
                typeof(PositionComponent));
            entityManager.SetComponentData(entity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(entity, new PositionComponent
            {
                Value = new float3(5f, 0f, 5f),
            });
            entityManager.SetComponentData(entity, new SettlementComponent
            {
                SettlementId = new FixedString64Bytes(settlementId),
                SettlementClassId = new FixedString32Bytes(settlementClassId),
                FortificationTier = fortificationTier,
                FortificationCeiling = fortificationTier,
            });

            if (addReserve)
            {
                entityManager.AddComponentData(entity, new FortificationComponent
                {
                    Tier = fortificationTier,
                    ThreatRadiusTiles = 6f,
                    ReserveRadiusTiles = 6f,
                });
                entityManager.AddComponentData(entity, new FortificationReserveComponent
                {
                    MusterIntervalSeconds = 10f,
                    ReserveHealPerSecond = 10f,
                    RetreatHealthRatio = 0.3f,
                    RecoveryHealthRatio = 0.5f,
                    TriageRadiusTiles = 2f,
                    ThreatActive = false,
                    ReadyReserveCount = 1,
                });
            }

            return entity;
        }

        private static void SeedFortificationDefender(
            EntityManager entityManager,
            Entity settlementEntity,
            string factionId,
            float healthCurrent,
            float healthMax)
        {
            Entity defender = entityManager.CreateEntity(
                typeof(FortificationCombatantTag),
                typeof(FortificationSettlementLinkComponent),
                typeof(FortificationReserveAssignmentComponent),
                typeof(PositionComponent),
                typeof(FactionComponent),
                typeof(HealthComponent));
            entityManager.SetComponentData(defender, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlementEntity,
            });
            entityManager.SetComponentData(defender, new FortificationReserveAssignmentComponent
            {
                Duty = ReserveDutyState.Recovering,
            });
            entityManager.SetComponentData(defender, new PositionComponent
            {
                Value = new float3(5f, 0f, 5f),
            });
            entityManager.SetComponentData(defender, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(defender, new HealthComponent
            {
                Current = healthCurrent,
                Max = healthMax,
            });
        }

        private static float GetLinkedDefenderHealth(EntityManager entityManager, string factionId)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FortificationCombatantTag>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var healths = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.Equals(new FixedString32Bytes(factionId)))
                {
                    return healths[i].Current;
                }
            }

            return -1f;
        }

        private static void Tick(World world, float inWorldDays)
        {
            world.SetTime(new Unity.Core.TimeData(inWorldDays, 0.05f));
            world.Update();
        }
    }
}
#endif
