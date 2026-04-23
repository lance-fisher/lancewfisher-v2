#if UNITY_EDITOR
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesPlayerSuccessionInfluenceSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-player-succession-influence-smoke.log";

        [MenuItem("Bloodlines/Player Diplomacy/Run Player Succession Influence Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchPlayerSuccessionInfluenceSmokeValidation() =>
            RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception exception)
            {
                success = false;
                message = "Player succession influence smoke validation errored: " + exception;
            }

            string artifact = "BLOODLINES_PLAYER_SUCCESSION_INFLUENCE_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine +
                              message;
            UnityDebug.Log(artifact);
            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var lines = new StringBuilder();
            bool ok = true;
            ok &= RunDesignationPhase(lines);
            ok &= RunPreferredSuccessorPhase(lines);
            ok &= RunFallbackPhase(lines);
            report = lines.ToString();
            return ok;
        }

        private static bool RunDesignationPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-succession-influence-designation");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            Entity clockEntity = SeedDualClock(entityManager, 12f);
            Entity factionEntity = SeedFaction(entityManager, "player", 160f, 58f);
            Entity preferredEntity = GetMemberEntityByRole(entityManager, factionEntity, DynastyRole.Governor);
            var preferredMember = entityManager.GetComponentData<DynastyMemberComponent>(preferredEntity);

            if (!debugScope.CommandSurface.TryDebugSetSuccessionPreference(
                    "player",
                    preferredMember.MemberId.ToString()))
            {
                lines.AppendLine("Phase 1 FAIL: debug surface could not queue the succession preference request.");
                return false;
            }

            Tick(world, entityManager, clockEntity, 12f);

            if (!entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
            {
                lines.AppendLine("Phase 1 FAIL: preference component was not written to the faction root.");
                return false;
            }

            var resources = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            var preference = entityManager.GetComponentData<SuccessionPreferenceComponent>(factionEntity);
            if (!debugScope.CommandSurface.TryDebugGetSuccessionPreferenceState("player", out var readout) ||
                !readout.Contains("Active=True", StringComparison.Ordinal) ||
                !preference.PreferredHeirMemberId.Equals(preferredMember.MemberId) ||
                preference.PreferredHeirEntity != preferredEntity ||
                preference.DesignationCostPaid == 0 ||
                !Approximately(resources.Gold, 110f) ||
                !Approximately(dynasty.Legitimacy, 54f))
            {
                lines.AppendLine(
                    $"Phase 1 FAIL: preference designation did not deduct the expected costs or expose the expected state. gold={resources.Gold:0.000}, legitimacy={dynasty.Legitimacy:0.000}, readout={readout ?? "<null>"}.");
                return false;
            }

            lines.AppendLine("Phase 1 PASS: designation wrote the preference component and deducted 50 gold plus 4 legitimacy.");
            return true;
        }

        private static bool RunPreferredSuccessorPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-succession-influence-preferred");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            Entity clockEntity = SeedDualClock(entityManager, 20f);
            Entity factionEntity = SeedFaction(entityManager, "player", 160f, 58f);
            Entity preferredEntity = GetMemberEntityByRole(entityManager, factionEntity, DynastyRole.Governor);
            var preferredMember = entityManager.GetComponentData<DynastyMemberComponent>(preferredEntity);

            if (!debugScope.CommandSurface.TryDebugSetSuccessionPreference(
                    "player",
                    preferredMember.MemberId.ToString()))
            {
                lines.AppendLine("Phase 2 FAIL: could not queue the preferred-heir designation.");
                return false;
            }

            Tick(world, entityManager, clockEntity, 20f);
            SetMemberStatus(entityManager, factionEntity, DynastyRole.HeadOfBloodline, DynastyMemberStatus.Fallen);
            Tick(world, entityManager, clockEntity, 21f);

            Entity rulingEntity = GetRulingMemberEntity(entityManager, factionEntity);
            if (rulingEntity != preferredEntity ||
                entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: preferred heir did not ascend or the preference component lingered. rulingEntity={rulingEntity.Index}, preferredEntity={preferredEntity.Index}.");
                return false;
            }

            var rulingMember = entityManager.GetComponentData<DynastyMemberComponent>(rulingEntity);
            if (rulingMember.Role != DynastyRole.HeadOfBloodline ||
                rulingMember.Status != DynastyMemberStatus.Ruling)
            {
                lines.AppendLine(
                    $"Phase 2 FAIL: preferred heir ascended without becoming the ruling head. role={rulingMember.Role}, status={rulingMember.Status}.");
                return false;
            }

            lines.AppendLine("Phase 2 PASS: valid preference overrode default succession order and promoted the preferred governor.");
            return true;
        }

        private static bool RunFallbackPhase(StringBuilder lines)
        {
            using var world = CreateValidationWorld("player-succession-influence-fallback");
            using var debugScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            Entity clockEntity = SeedDualClock(entityManager, 30f);
            Entity factionEntity = SeedFaction(entityManager, "player", 160f, 58f);
            Entity preferredEntity = GetMemberEntityByRole(entityManager, factionEntity, DynastyRole.Governor);
            Entity defaultHeirEntity = GetMemberEntityByRole(entityManager, factionEntity, DynastyRole.HeirDesignate);
            var preferredMember = entityManager.GetComponentData<DynastyMemberComponent>(preferredEntity);

            if (!debugScope.CommandSurface.TryDebugSetSuccessionPreference(
                    "player",
                    preferredMember.MemberId.ToString()))
            {
                lines.AppendLine("Phase 3 FAIL: could not queue the fallback test designation.");
                return false;
            }

            Tick(world, entityManager, clockEntity, 30f);
            SetMemberStatus(entityManager, factionEntity, DynastyRole.Governor, DynastyMemberStatus.Fallen);
            SetMemberStatus(entityManager, factionEntity, DynastyRole.HeadOfBloodline, DynastyMemberStatus.Fallen);
            Tick(world, entityManager, clockEntity, 31f);

            Entity rulingEntity = GetRulingMemberEntity(entityManager, factionEntity);
            if (!debugScope.CommandSurface.TryDebugGetSuccessionPreferenceState("player", out var readout) ||
                rulingEntity != defaultHeirEntity ||
                entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity) ||
                !readout.Contains("Active=False", StringComparison.Ordinal))
            {
                lines.AppendLine(
                    $"Phase 3 FAIL: invalid preference did not fall back to the default heir. rulingEntity={rulingEntity.Index}, defaultHeirEntity={defaultHeirEntity.Index}, readout={readout ?? "<null>"}.");
                return false;
            }

            lines.AppendLine("Phase 3 PASS: fallen preferred heir was discarded and succession fell back to the default heir-designate order.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SuccessionPreferenceResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DynastySuccessionSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static Entity SeedDualClock(EntityManager entityManager, float inWorldDays)
        {
            var clockEntity = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clockEntity, new DualClockComponent
            {
                InWorldDays = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount = 0,
            });
            entityManager.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
            return clockEntity;
        }

        private static Entity SeedFaction(
            EntityManager entityManager,
            string factionId,
            float gold,
            float legitimacy)
        {
            var factionEntity = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(ResourceStockpileComponent));
            entityManager.SetComponentData(factionEntity, new FactionComponent
            {
                FactionId = new FixedString32Bytes(factionId),
            });
            entityManager.SetComponentData(factionEntity, new FactionKindComponent
            {
                Kind = FactionKind.Kingdom,
            });
            entityManager.SetComponentData(factionEntity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = 120f,
                Water = 120f,
                Wood = 40f,
                Stone = 25f,
                Iron = 15f,
                Influence = 12f,
            });

            DynastyBootstrap.AttachDynasty(entityManager, factionEntity, new FixedString32Bytes(factionId));
            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = legitimacy;
            entityManager.SetComponentData(factionEntity, dynasty);
            return factionEntity;
        }

        private static void Tick(
            World world,
            EntityManager entityManager,
            Entity clockEntity,
            float inWorldDays)
        {
            var dualClock = entityManager.GetComponentData<DualClockComponent>(clockEntity);
            dualClock.InWorldDays = inWorldDays;
            entityManager.SetComponentData(clockEntity, dualClock);
            world.SetTime(new TimeData(0d, 0.05f));
            world.Update();
        }

        private static Entity GetMemberEntityByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role)
        {
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Role == role)
                {
                    return memberEntity;
                }
            }

            throw new InvalidOperationException($"Could not find dynasty member with role {role}.");
        }

        private static Entity GetRulingMemberEntity(EntityManager entityManager, Entity factionEntity)
        {
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status == DynastyMemberStatus.Ruling)
                {
                    return memberEntity;
                }
            }

            return Entity.Null;
        }

        private static void SetMemberStatus(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role,
            DynastyMemberStatus status)
        {
            Entity memberEntity = GetMemberEntityByRole(entityManager, factionEntity, role);
            var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
            member.Status = status;
            entityManager.SetComponentData(memberEntity, member);
        }

        private static bool Approximately(float actual, float expected)
        {
            return Mathf.Abs(actual - expected) <= 0.001f;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;

                hostObject = new GameObject("BloodlinesPlayerSuccessionInfluenceSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
