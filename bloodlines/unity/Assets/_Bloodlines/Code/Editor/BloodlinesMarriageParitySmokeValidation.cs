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
    /// Marriage parity smoke validator for the dynasty-marriage-parity lane.
    /// Proves:
    ///   1. proposals remain pending at day 89;
    ///   2. proposals expire at day 90;
    ///   3. gestation generates one child at day 280, records the child id on
    ///      the primary marriage, and attaches mixed-bloodline metadata;
    ///   4. death-driven dissolution marks both mirror records dissolved,
    ///      applies legitimacy/oathkeeping effects, and halts gestation.
    /// </summary>
    public static class BloodlinesMarriageParitySmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-marriage-parity-smoke.log";

        [MenuItem("Bloodlines/Dynasty/Run Marriage Parity Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchMarriageParitySmokeValidation()
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
                message = "Marriage parity smoke errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunProposalPendingPhase(out string pendingMessage))
            {
                message = pendingMessage;
                return false;
            }

            if (!RunProposalExpiryPhase(out string expiryMessage))
            {
                message = expiryMessage;
                return false;
            }

            if (!RunGestationPhase(out string gestationMessage))
            {
                message = gestationMessage;
                return false;
            }

            if (!RunDeathDissolutionPhase(out string dissolutionMessage))
            {
                message = dissolutionMessage;
                return false;
            }

            message =
                "Marriage parity smoke validation passed: pendingPhase=True, expiryPhase=True, gestationPhase=True, dissolutionPhase=True. " +
                pendingMessage + " " + expiryMessage + " " + gestationMessage + " " + dissolutionMessage;
            return true;
        }

        private static bool RunProposalPendingPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesMarriageParitySmokeValidation_Pending");
            var em = world.EntityManager;

            CreateClock(em, 89f);
            CreateProposal(em, "proposal-pending", proposedAtInWorldDays: 0f, expiresAtInWorldDays: 90f);

            world.Update();

            var proposal = GetSingleComponent<MarriageProposalComponent>(em);
            if (proposal.Status != MarriageProposalStatus.Pending)
            {
                message = "Marriage parity smoke failed: proposal expired before day 90.";
                return false;
            }

            message = "PendingPhase: status=Pending at day=89.";
            return true;
        }

        private static bool RunProposalExpiryPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesMarriageParitySmokeValidation_Expiry");
            var em = world.EntityManager;

            CreateClock(em, 90f);
            CreateProposal(em, "proposal-expired", proposedAtInWorldDays: 0f, expiresAtInWorldDays: 90f);

            world.Update();

            var proposal = GetSingleComponent<MarriageProposalComponent>(em);
            if (proposal.Status != MarriageProposalStatus.Expired)
            {
                message = "Marriage parity smoke failed: proposal did not expire at day 90.";
                return false;
            }

            message = "ExpiryPhase: status=Expired at day=90.";
            return true;
        }

        private static bool RunGestationPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesMarriageParitySmokeValidation_Gestation");
            var em = world.EntityManager;

            CreateClock(em, 280f);

            var headFaction = CreateFaction(em, "player", "ironmark");
            var spouseFaction = CreateFaction(em, "enemy", "stonehelm");
            AddMember(em, headFaction, "player-bloodline-heir", "Eldest Heir", DynastyRole.HeirDesignate, DynastyMemberStatus.Active);
            AddMember(em, spouseFaction, "enemy-bloodline-heir", "Stone Heir", DynastyRole.HeirDesignate, DynastyMemberStatus.Active);

            var marriageEntity = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(marriageEntity, new MarriageComponent
            {
                MarriageId = "marriage-gestation",
                HeadFactionId = "player",
                HeadMemberId = "player-bloodline-heir",
                SpouseFactionId = "enemy",
                SpouseMemberId = "enemy-bloodline-heir",
                MarriedAtInWorldDays = 0f,
                ExpectedChildAtInWorldDays = 280f,
                IsPrimary = true,
                ChildGenerated = false,
                Dissolved = false,
                DissolvedAtInWorldDays = 0f,
            });

            world.Update();

            var marriage = em.GetComponentData<MarriageComponent>(marriageEntity);
            if (!marriage.ChildGenerated)
            {
                message = "Marriage parity smoke failed: gestation did not mark ChildGenerated.";
                return false;
            }

            if (!em.HasBuffer<MarriageChildElement>(marriageEntity))
            {
                message = "Marriage parity smoke failed: gestation did not record child ids.";
                return false;
            }

            var children = em.GetBuffer<MarriageChildElement>(marriageEntity);
            if (children.Length != 1)
            {
                message = $"Marriage parity smoke failed: expected 1 child id, saw {children.Length}.";
                return false;
            }

            var childEntity = FindMemberEntity(em, headFaction, children[0].ChildMemberId);
            if (childEntity == Entity.Null)
            {
                message = "Marriage parity smoke failed: child member was not added to the head dynasty roster.";
                return false;
            }

            if (!em.HasComponent<DynastyMixedBloodlineComponent>(childEntity))
            {
                message = "Marriage parity smoke failed: child missing mixed-bloodline metadata.";
                return false;
            }

            var mixedBloodline = em.GetComponentData<DynastyMixedBloodlineComponent>(childEntity);
            if (!mixedBloodline.HeadHouseId.Equals(new FixedString32Bytes("ironmark")) ||
                !mixedBloodline.SpouseHouseId.Equals(new FixedString32Bytes("stonehelm")))
            {
                message =
                    "Marriage parity smoke failed: child mixed-bloodline houses drifted. " +
                    $"head={mixedBloodline.HeadHouseId}, spouse={mixedBloodline.SpouseHouseId}.";
                return false;
            }

            var child = em.GetComponentData<DynastyMemberComponent>(childEntity);
            message =
                $"GestationPhase: childId={children[0].ChildMemberId}, childTitle={child.Title}, headHouse={mixedBloodline.HeadHouseId}, spouseHouse={mixedBloodline.SpouseHouseId}.";
            return true;
        }

        private static bool RunDeathDissolutionPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesMarriageParitySmokeValidation_Dissolution");
            var em = world.EntityManager;

            CreateClock(em, 280f);

            var headFaction = CreateFaction(em, "player", "ironmark");
            var spouseFaction = CreateFaction(em, "enemy", "stonehelm");
            AddMember(em, headFaction, "player-bloodline-heir", "Eldest Heir", DynastyRole.HeirDesignate, DynastyMemberStatus.Active);
            AddMember(em, spouseFaction, "enemy-bloodline-heir", "Stone Heir", DynastyRole.HeirDesignate, DynastyMemberStatus.Fallen);

            var primary = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(primary, new MarriageComponent
            {
                MarriageId = "marriage-dissolution",
                HeadFactionId = "player",
                HeadMemberId = "player-bloodline-heir",
                SpouseFactionId = "enemy",
                SpouseMemberId = "enemy-bloodline-heir",
                MarriedAtInWorldDays = 0f,
                ExpectedChildAtInWorldDays = 10f,
                IsPrimary = true,
                ChildGenerated = false,
                Dissolved = false,
                DissolvedAtInWorldDays = 0f,
            });

            var mirror = em.CreateEntity(typeof(MarriageComponent));
            em.SetComponentData(mirror, new MarriageComponent
            {
                MarriageId = "marriage-dissolution",
                HeadFactionId = "enemy",
                HeadMemberId = "enemy-bloodline-heir",
                SpouseFactionId = "player",
                SpouseMemberId = "player-bloodline-heir",
                MarriedAtInWorldDays = 0f,
                ExpectedChildAtInWorldDays = 10f,
                IsPrimary = false,
                ChildGenerated = false,
                Dissolved = false,
                DissolvedAtInWorldDays = 0f,
            });

            world.Update();

            var primaryMarriage = em.GetComponentData<MarriageComponent>(primary);
            var mirrorMarriage = em.GetComponentData<MarriageComponent>(mirror);
            if (!primaryMarriage.Dissolved || !mirrorMarriage.Dissolved)
            {
                message = "Marriage parity smoke failed: death dissolution did not mark both records dissolved.";
                return false;
            }

            if (Math.Abs(primaryMarriage.DissolvedAtInWorldDays - 280f) > 0.01f ||
                Math.Abs(mirrorMarriage.DissolvedAtInWorldDays - 280f) > 0.01f)
            {
                message =
                    "Marriage parity smoke failed: dissolvedAtInWorldDays drifted. " +
                    $"primary={primaryMarriage.DissolvedAtInWorldDays}, mirror={mirrorMarriage.DissolvedAtInWorldDays}.";
                return false;
            }

            var headDynasty = em.GetComponentData<DynastyStateComponent>(headFaction);
            var spouseDynasty = em.GetComponentData<DynastyStateComponent>(spouseFaction);
            if (Math.Abs(headDynasty.Legitimacy - 58f) > 0.01f ||
                Math.Abs(spouseDynasty.Legitimacy - 58f) > 0.01f)
            {
                message =
                    "Marriage parity smoke failed: legitimacy loss drifted. " +
                    $"head={headDynasty.Legitimacy}, spouse={spouseDynasty.Legitimacy}.";
                return false;
            }

            var headConviction = em.GetComponentData<ConvictionComponent>(headFaction);
            var spouseConviction = em.GetComponentData<ConvictionComponent>(spouseFaction);
            if (Math.Abs(headConviction.Oathkeeping - 1f) > 0.01f ||
                Math.Abs(spouseConviction.Oathkeeping - 1f) > 0.01f)
            {
                message =
                    "Marriage parity smoke failed: oathkeeping gain drifted. " +
                    $"head={headConviction.Oathkeeping}, spouse={spouseConviction.Oathkeeping}.";
                return false;
            }

            if (em.HasBuffer<MarriageChildElement>(primary) && em.GetBuffer<MarriageChildElement>(primary).Length > 0)
            {
                message = "Marriage parity smoke failed: dissolved marriage still generated a child.";
                return false;
            }

            if (em.GetBuffer<DynastyMemberRef>(headFaction).Length != 1)
            {
                message = "Marriage parity smoke failed: dissolved marriage altered head dynasty roster size.";
                return false;
            }

            message =
                $"DissolutionPhase: primaryDissolved={primaryMarriage.Dissolved}, mirrorDissolved={mirrorMarriage.Dissolved}, legitimacy=58/58, oathkeeping=1/1.";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageProposalExpirationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageDeathDissolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MarriageGestationSystem>());
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

        private static Entity CreateFaction(
            EntityManager em,
            string factionId,
            string houseId)
        {
            var faction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionHouseComponent),
                typeof(DynastyStateComponent),
                typeof(ConvictionComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = factionId });
            em.SetComponentData(faction, new FactionHouseComponent { HouseId = houseId });
            em.SetComponentData(faction, new DynastyStateComponent
            {
                ActiveMemberCap = 8,
                DormantReserve = 0,
                Legitimacy = 60f,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });
            em.SetComponentData(faction, new ConvictionComponent { Band = ConvictionBand.Neutral });
            em.AddBuffer<DynastyMemberRef>(faction);
            em.AddBuffer<DynastyFallenLedger>(faction);
            return faction;
        }

        private static Entity AddMember(
            EntityManager em,
            Entity factionEntity,
            string memberId,
            string title,
            DynastyRole role,
            DynastyMemberStatus status)
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
                AgeYears = 18f,
                Status = status,
                Renown = 10f,
                Order = em.GetBuffer<DynastyMemberRef>(factionEntity).Length,
                FallenAtWorldSeconds = status == DynastyMemberStatus.Fallen ? 0f : -1f,
            });
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

        private static Entity FindMemberEntity(
            EntityManager em,
            Entity factionEntity,
            FixedString64Bytes memberId)
        {
            var members = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null || !em.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(memberId))
                {
                    return memberEntity;
                }
            }

            return Entity.Null;
        }

        private static void CreateProposal(
            EntityManager em,
            string proposalId,
            float proposedAtInWorldDays,
            float expiresAtInWorldDays)
        {
            var proposal = em.CreateEntity(typeof(MarriageProposalComponent));
            em.SetComponentData(proposal, new MarriageProposalComponent
            {
                ProposalId = proposalId,
                SourceFactionId = "player",
                SourceMemberId = "player-bloodline-heir",
                TargetFactionId = "enemy",
                TargetMemberId = "enemy-bloodline-heir",
                Status = MarriageProposalStatus.Pending,
                ProposedAtInWorldDays = proposedAtInWorldDays,
                ExpiresAtInWorldDays = expiresAtInWorldDays,
            });
        }

        private static T GetSingleComponent<T>(EntityManager em)
            where T : unmanaged, IComponentData
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<T>());
            var component = query.GetSingleton<T>();
            query.Dispose();
            return component;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            string result = "BLOODLINES_MARRIAGE_PARITY_SMOKE " + (success ? "PASS" : "FAIL") + Environment.NewLine + message;

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
