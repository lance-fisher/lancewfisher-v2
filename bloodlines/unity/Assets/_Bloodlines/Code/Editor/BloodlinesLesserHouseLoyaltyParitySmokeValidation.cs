#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Lesser-house loyalty drift parity smoke validator for the dynasty lane.
    /// Proves:
    ///   1. active marriage anchors and mixed-bloodline recovery raise the
    ///      current daily delta exactly per browser canon;
    ///   2. hostility + death-dissolution + targeted world pressure strain a
    ///      cadet branch into the fractured state with the expected daily loss;
    ///   3. loyalty hitting zero starts a five-day grace window before actual
    ///      defection, then applies legitimacy/ruthlessness consequences and
    ///      spawns the minor-house faction.
    /// </summary>
    public static class BloodlinesLesserHouseLoyaltyParitySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-lesser-house-loyalty-parity-smoke.log";

        [MenuItem("Bloodlines/Dynasty/Run Lesser House Loyalty Parity Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchLesserHouseLoyaltyParitySmokeValidation()
        {
            RunInternal(batchMode: true);
        }

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception e)
            {
                success = false;
                message = "Lesser-house loyalty parity smoke errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunActiveAnchorRecoveryPhase(out string activeMessage))
            {
                message = activeMessage;
                return false;
            }

            if (!RunDissolutionHostilityWorldPressurePhase(out string dissolutionMessage))
            {
                message = dissolutionMessage;
                return false;
            }

            if (!RunDefectionGracePhase(out string defectionMessage))
            {
                message = defectionMessage;
                return false;
            }

            message =
                "Lesser-house loyalty parity smoke validation passed: activeAnchor=True, dissolvedAnchor=True, defectionGrace=True. " +
                activeMessage + " " + dissolutionMessage + " " + defectionMessage;
            return true;
        }

        private static bool RunActiveAnchorRecoveryPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesLesserHouseLoyaltyParity_ActiveAnchor");
            var em = world.EntityManager;

            var player = CreateDynastyFaction(
                em,
                "player",
                "ironmark",
                legitimacy: 75f,
                oathkeeping: 10f,
                ruthlessness: 0f);
            var enemy = CreateDynastyFaction(
                em,
                "enemy",
                "stonehelm",
                legitimacy: 60f,
                oathkeeping: 0f,
                ruthlessness: 0f);

            AddDynastyMember(
                em,
                player,
                "player-founder",
                "Lord Founder",
                DynastyRole.Governor,
                DynastyMemberStatus.Active,
                mixedBloodlineSpouseHouseId: "stonehelm");
            AddDynastyMember(
                em,
                enemy,
                "enemy-spouse",
                "Stone Spouse",
                DynastyRole.HeirDesignate,
                DynastyMemberStatus.Active);

            AddLesserHouse(
                em,
                player,
                houseId: "cadet-goldbranch",
                founderMemberId: "player-founder",
                loyalty: 50f,
                lastDriftAppliedInWorldDays: 10f);
            CreateMarriage(
                em,
                "marriage-active",
                "player",
                "player-founder",
                "enemy",
                "enemy-spouse",
                dissolved: false,
                childCount: 1);
            CreateClock(em, 11f);

            world.Update();

            var cadet = GetSingleLesserHouse(em, player);
            if (!cadet.MixedBloodlineHouseId.Equals(new FixedString32Bytes("stonehelm")) ||
                cadet.MaritalAnchorStatus != LesserHouseMaritalAnchorStatus.Active ||
                cadet.MaritalAnchorChildCount != 1)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: active anchor metadata drifted. " +
                    $"mixed={cadet.MixedBloodlineHouseId}, status={cadet.MaritalAnchorStatus}, children={cadet.MaritalAnchorChildCount}.";
                return false;
            }

            if (Mathf.Abs(cadet.CurrentDailyLoyaltyDelta - 0.66f) > 0.01f ||
                Mathf.Abs(cadet.Loyalty - 50.66f) > 0.01f)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: active anchor delta drifted. " +
                    $"daily={cadet.CurrentDailyLoyaltyDelta}, loyalty={cadet.Loyalty}.";
                return false;
            }

            message =
                $"ActiveAnchor: dailyDelta={cadet.CurrentDailyLoyaltyDelta:F2}, loyalty={cadet.Loyalty:F2}, mixedHouse={cadet.MixedBloodlineHouseId}.";
            return true;
        }

        private static bool RunDissolutionHostilityWorldPressurePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesLesserHouseLoyaltyParity_Dissolution");
            var em = world.EntityManager;

            var player = CreateDynastyFaction(
                em,
                "player",
                "ironmark",
                legitimacy: 40f,
                oathkeeping: 0f,
                ruthlessness: 0f);
            var enemy = CreateDynastyFaction(
                em,
                "enemy",
                "stonehelm",
                legitimacy: 60f,
                oathkeeping: 0f,
                ruthlessness: 0f);

            AddDynastyMember(
                em,
                player,
                "player-founder",
                "Lord Founder",
                DynastyRole.Governor,
                DynastyMemberStatus.Active,
                mixedBloodlineSpouseHouseId: "stonehelm");
            AddDynastyMember(
                em,
                enemy,
                "enemy-spouse",
                "Stone Spouse",
                DynastyRole.HeirDesignate,
                DynastyMemberStatus.Fallen);

            AddLesserHouse(
                em,
                player,
                houseId: "cadet-stormbranch",
                founderMemberId: "player-founder",
                loyalty: 50f,
                lastDriftAppliedInWorldDays: 10f);
            CreateMarriage(
                em,
                "marriage-dissolving",
                "player",
                "player-founder",
                "enemy",
                "enemy-spouse",
                dissolved: false,
                childCount: 2);

            AddHostility(em, player, "enemy");
            em.AddComponentData(player, new WorldPressureComponent
            {
                Score = 6,
                Streak = 3,
                Level = 2,
                Label = "Overwhelming",
                Targeted = true,
                TerritoryExpansionScore = 2,
                GreatReckoningScore = 4,
            });

            CreateClock(em, 11f);
            world.Update();

            var cadet = GetSingleLesserHouse(em, player);
            if (cadet.MaritalAnchorStatus != LesserHouseMaritalAnchorStatus.Fractured ||
                cadet.WorldPressureStatus != LesserHouseWorldPressureStatus.Overwhelming)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: dissolved hostility/world-pressure status drifted. " +
                    $"anchor={cadet.MaritalAnchorStatus}, world={cadet.WorldPressureStatus}.";
                return false;
            }

            if (Mathf.Abs(cadet.CurrentDailyLoyaltyDelta - (-1.10f)) > 0.01f ||
                Mathf.Abs(cadet.Loyalty - 48.90f) > 0.01f)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: dissolved hostility delta drifted. " +
                    $"daily={cadet.CurrentDailyLoyaltyDelta}, loyalty={cadet.Loyalty}.";
                return false;
            }

            using var marriages = FindMarriageRecords(em, "marriage-dissolving");
            if (marriages.Length != 2 || !marriages[0].Dissolved || !marriages[1].Dissolved)
            {
                message = "Lesser-house loyalty parity smoke failed: death dissolution did not precede cadet drift.";
                return false;
            }

            message =
                $"DissolutionAnchor: dailyDelta={cadet.CurrentDailyLoyaltyDelta:F2}, loyalty={cadet.Loyalty:F2}, anchor={cadet.MaritalAnchorStatus}, world={cadet.WorldPressureStatus}.";
            return true;
        }

        private static bool RunDefectionGracePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesLesserHouseLoyaltyParity_Defection");
            var em = world.EntityManager;

            var player = CreateDynastyFaction(
                em,
                "player",
                "ironmark",
                legitimacy: 20f,
                oathkeeping: 0f,
                ruthlessness: 0f);

            AddDynastyMember(
                em,
                player,
                "player-founder",
                "Lord Founder",
                DynastyRole.Governor,
                DynastyMemberStatus.Active);
            AddLesserHouse(
                em,
                player,
                houseId: "cadet-ashfall",
                founderMemberId: "player-founder",
                loyalty: 0.25f,
                lastDriftAppliedInWorldDays: 10f);

            var clock = CreateClock(em, 11f);
            world.Update();

            var firstTick = GetSingleLesserHouse(em, player);
            if (firstTick.Defected || Mathf.Abs(firstTick.BrokeAtLoyaltyZeroInWorldDays - 11f) > 0.01f)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: zero-loyalty crossing should start grace, not defect immediately. " +
                    $"defected={firstTick.Defected}, brokeAt={firstTick.BrokeAtLoyaltyZeroInWorldDays}.";
                return false;
            }

            SetClockDays(em, clock, 15f);
            world.Update();
            var fourthDay = GetSingleLesserHouse(em, player);
            if (fourthDay.Defected)
            {
                message = "Lesser-house loyalty parity smoke failed: cadet branch defected before the five-day grace elapsed.";
                return false;
            }

            SetClockDays(em, clock, 16f);
            world.Update();

            var defected = GetSingleLesserHouse(em, player);
            if (!defected.Defected || Mathf.Abs(defected.DepartedAtInWorldDays - 16f) > 0.01f)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: cadet branch did not defect on grace completion. " +
                    $"defected={defected.Defected}, departedAt={defected.DepartedAtInWorldDays}.";
                return false;
            }

            var dynasty = em.GetComponentData<DynastyStateComponent>(player);
            var conviction = em.GetComponentData<ConvictionComponent>(player);
            if (Mathf.Abs(dynasty.Legitimacy - 14f) > 0.01f ||
                Mathf.Abs(conviction.Ruthlessness - 1f) > 0.01f)
            {
                message =
                    "Lesser-house loyalty parity smoke failed: defection consequences drifted. " +
                    $"legitimacy={dynasty.Legitimacy}, ruthlessness={conviction.Ruthlessness}.";
                return false;
            }

            var spawnedMinor = FindFactionById(em, "minor-cadet-ashfall");
            if (spawnedMinor == Entity.Null ||
                !em.HasComponent<FactionKindComponent>(spawnedMinor) ||
                em.GetComponentData<FactionKindComponent>(spawnedMinor).Kind != FactionKind.MinorHouse)
            {
                message = "Lesser-house loyalty parity smoke failed: defected cadet branch did not spawn a minor-house faction.";
                return false;
            }

            if (!HasHostility(em, player, "minor-cadet-ashfall") ||
                !HasHostility(em, spawnedMinor, "player"))
            {
                message = "Lesser-house loyalty parity smoke failed: defection did not establish mutual hostility.";
                return false;
            }

            message =
                $"DefectionGrace: brokeAt={defected.BrokeAtLoyaltyZeroInWorldDays:F0}, departedAt={defected.DepartedAtInWorldDays:F0}, legitimacy={dynasty.Legitimacy:F0}, spawnedMinor=minor-cadet-ashfall.";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageDeathDissolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<LesserHouseLoyaltyDriftSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static Entity CreateClock(EntityManager em, float inWorldDays)
        {
            var clock = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = inWorldDays,
            });
            return clock;
        }

        private static void SetClockDays(EntityManager em, Entity clock, float inWorldDays)
        {
            var value = em.GetComponentData<DualClockComponent>(clock);
            value.InWorldDays = inWorldDays;
            em.SetComponentData(clock, value);
        }

        private static Entity CreateDynastyFaction(
            EntityManager em,
            string factionId,
            string houseId,
            float legitimacy,
            float oathkeeping,
            float ruthlessness)
        {
            var faction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionHouseComponent),
                typeof(FactionKindComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = factionId });
            em.SetComponentData(faction, new FactionHouseComponent { HouseId = houseId });
            em.SetComponentData(faction, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(faction, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = legitimacy,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });

            var conviction = new ConvictionComponent
            {
                Oathkeeping = oathkeeping,
                Ruthlessness = ruthlessness,
            };
            ConvictionScoring.Refresh(ref conviction);
            em.SetComponentData(faction, conviction);

            em.AddBuffer<DynastyMemberRef>(faction);
            em.AddBuffer<DynastyFallenLedger>(faction);
            em.AddBuffer<LesserHouseElement>(faction);
            return faction;
        }

        private static Entity AddDynastyMember(
            EntityManager em,
            Entity factionEntity,
            string memberId,
            string title,
            DynastyRole role,
            DynastyMemberStatus status,
            string mixedBloodlineSpouseHouseId = null)
        {
            var faction = em.GetComponentData<FactionComponent>(factionEntity);
            var member = em.CreateEntity(typeof(FactionComponent), typeof(DynastyMemberComponent));
            em.SetComponentData(member, new FactionComponent { FactionId = faction.FactionId });
            em.SetComponentData(member, new DynastyMemberComponent
            {
                MemberId = memberId,
                Title = title,
                Role = role,
                Path = DynastyPath.Governance,
                AgeYears = 28f,
                Status = status,
                Renown = 40f,
                Order = em.GetBuffer<DynastyMemberRef>(factionEntity).Length,
                FallenAtWorldSeconds = status == DynastyMemberStatus.Fallen ? 0f : -1f,
            });
            if (!string.IsNullOrWhiteSpace(mixedBloodlineSpouseHouseId))
            {
                em.AddComponentData(member, new DynastyMixedBloodlineComponent
                {
                    HeadHouseId = em.GetComponentData<FactionHouseComponent>(factionEntity).HouseId,
                    SpouseHouseId = mixedBloodlineSpouseHouseId,
                });
            }

            em.GetBuffer<DynastyMemberRef>(factionEntity).Add(new DynastyMemberRef { Member = member });
            if (status == DynastyMemberStatus.Fallen)
            {
                em.GetBuffer<DynastyFallenLedger>(factionEntity).Add(new DynastyFallenLedger
                {
                    MemberId = memberId,
                    Title = title,
                    Role = role,
                    FallenAtWorldSeconds = 0f,
                });
            }

            return member;
        }

        private static void AddLesserHouse(
            EntityManager em,
            Entity factionEntity,
            string houseId,
            string founderMemberId,
            float loyalty,
            float lastDriftAppliedInWorldDays)
        {
            em.GetBuffer<LesserHouseElement>(factionEntity).Add(new LesserHouseElement
            {
                HouseId = houseId,
                HouseName = houseId + " cadet",
                FounderMemberId = founderMemberId,
                Loyalty = loyalty,
                DailyLoyaltyDelta = 0f,
                LastDriftAppliedInWorldDays = lastDriftAppliedInWorldDays,
                Defected = false,
                BrokeAtLoyaltyZeroInWorldDays = -1f,
                DepartedAtInWorldDays = -1f,
            });
        }

        private static void CreateMarriage(
            EntityManager em,
            string marriageId,
            string headFactionId,
            string headMemberId,
            string spouseFactionId,
            string spouseMemberId,
            bool dissolved,
            int childCount)
        {
            var primary = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(primary, new MarriageComponent
            {
                MarriageId = marriageId,
                HeadFactionId = headFactionId,
                HeadMemberId = headMemberId,
                SpouseFactionId = spouseFactionId,
                SpouseMemberId = spouseMemberId,
                MarriedAtInWorldDays = 1f,
                ExpectedChildAtInWorldDays = 280f,
                IsPrimary = true,
                ChildGenerated = childCount > 0,
                Dissolved = dissolved,
                DissolvedAtInWorldDays = dissolved ? 8f : 0f,
            });

            var mirror = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(mirror, new MarriageComponent
            {
                MarriageId = marriageId,
                HeadFactionId = spouseFactionId,
                HeadMemberId = spouseMemberId,
                SpouseFactionId = headFactionId,
                SpouseMemberId = headMemberId,
                MarriedAtInWorldDays = 1f,
                ExpectedChildAtInWorldDays = 280f,
                IsPrimary = false,
                ChildGenerated = childCount > 0,
                Dissolved = dissolved,
                DissolvedAtInWorldDays = dissolved ? 8f : 0f,
            });

            if (childCount <= 0)
            {
                return;
            }

            var children = em.AddBuffer<MarriageChildElement>(primary);
            for (int i = 0; i < childCount; i++)
            {
                children.Add(new MarriageChildElement
                {
                    ChildMemberId = $"{marriageId}-child-{i}",
                });
            }
        }

        private static void AddHostility(EntityManager em, Entity factionEntity, string hostileFactionId)
        {
            var buffer = em.HasBuffer<HostilityComponent>(factionEntity)
                ? em.GetBuffer<HostilityComponent>(factionEntity)
                : em.AddBuffer<HostilityComponent>(factionEntity);
            buffer.Add(new HostilityComponent
            {
                HostileFactionId = hostileFactionId,
            });
        }

        private static LesserHouseElement GetSingleLesserHouse(EntityManager em, Entity factionEntity)
        {
            return em.GetBuffer<LesserHouseElement>(factionEntity)[0];
        }

        private static NativeList<MarriageComponent> FindMarriageRecords(EntityManager em, string marriageId)
        {
            var matches = new NativeList<MarriageComponent>(Allocator.Temp);
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            using var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();

            var targetId = new FixedString64Bytes(marriageId);
            for (int i = 0; i < marriages.Length; i++)
            {
                if (marriages[i].MarriageId.Equals(targetId))
                {
                    matches.Add(marriages[i]);
                }
            }

            return matches;
        }

        private static Entity FindFactionById(EntityManager em, string factionId)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            var targetId = new FixedString32Bytes(factionId);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(targetId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static bool HasHostility(EntityManager em, Entity factionEntity, string hostileFactionId)
        {
            if (!em.HasBuffer<HostilityComponent>(factionEntity))
            {
                return false;
            }

            var targetId = new FixedString32Bytes(hostileFactionId);
            var buffer = em.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].HostileFactionId.Equals(targetId))
                {
                    return true;
                }
            }

            return false;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            string result =
                "BLOODLINES_LESSER_HOUSE_LOYALTY_PARITY_SMOKE " +
                (success ? "PASS" : "FAIL") +
                Environment.NewLine +
                message;

            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, result);
            }
            catch
            {
            }

            UnityDebug.Log(result);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
#endif
