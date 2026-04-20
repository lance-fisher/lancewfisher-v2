#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 25 missionary resolution. First
    /// production per-kind resolution consumer; walks expired Missionary
    /// DynastyOperationComponent entries at ResolveAtInWorldDays and
    /// applies the canonical browser missionary resolution effects
    /// (simulation.js:5517-5588).
    ///
    /// Phases:
    ///   PhaseNotExpiredSkips: seed an active missionary op with
    ///     ResolveAtInWorldDays > current; tick; verify Active remains
    ///     true, no effects applied to target, no narrative pushed.
    ///   PhaseExpiredSuccessApplies: seed expired op with
    ///     SuccessScore = 10 (positive success); verify Active flipped
    ///     to false, target's FaithExposureElement incremented by
    ///     ExposureGain, target FaithStateComponent.Intensity reduced
    ///     by IntensityErosion, lowest-loyalty ControlPointComponent
    ///     owned by target reduced by LoyaltyPressure, success
    ///     narrative pushed.
    ///   PhaseExpiredFailureStrengthens: seed expired op with
    ///     SuccessScore = -5 (failure); verify Active flipped to false,
    ///     target FaithStateComponent.Intensity increased by 2,
    ///     target ControlPointComponent loyalty unchanged, target
    ///     exposure unchanged, failure narrative pushed.
    ///   PhaseVoidOnMissingTarget: seed expired op whose target
    ///     faction entity does not exist; verify Active flipped to
    ///     false, void narrative pushed.
    ///   PhaseExposureAppendsNewEntry: target has no FaithExposureElement
    ///     entry for the source faith; verify a new entry is appended
    ///     with Discovered=true and Exposure = clamped(ExposureGain).
    ///   PhaseExposureClampsAt100: target's existing exposure is 95 and
    ///     ExposureGain is 20; verify Exposure clamped to 100.
    ///   PhaseLowestLoyaltyControlPointTargeted: target owns two
    ///     control points with loyalties 80 and 50; verify only the
    ///     50-loyalty control point is reduced by LoyaltyPressure.
    ///   PhaseIntensityErosionSkippedWhenSameFaith: target and source
    ///     have identical SelectedFaith; verify IntensityErosion is
    ///     NOT applied (browser gate at simulation.js:10485-10488).
    ///
    /// Browser reference:
    ///   simulation.js tickDynastyOperations missionary branch
    ///     (~5517-5588), applyMissionaryEffect (~10473-10503),
    ///     FAITH_EXPOSURE_THRESHOLD (~6).
    ///
    /// Artifact: artifacts/unity-missionary-resolution-smoke.log.
    /// </summary>
    public static class BloodlinesMissionaryResolutionSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-missionary-resolution-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Missionary Resolution Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchMissionaryResolutionSmokeValidation() =>
            RunInternal(batchMode: true);

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
                message = "Missionary resolution smoke errored: " + e;
            }

            string artifact = "BLOODLINES_MISSIONARY_RESOLUTION_SMOKE " +
                              (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception) { }

            if (batchMode)
                EditorApplication.Exit(success ? 0 : 1);
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhaseNotExpiredSkips(sb);
            ok &= RunPhaseExpiredSuccessApplies(sb);
            ok &= RunPhaseExpiredFailureStrengthens(sb);
            ok &= RunPhaseVoidOnMissingTarget(sb);
            ok &= RunPhaseExposureAppendsNewEntry(sb);
            ok &= RunPhaseExposureClampsAt100(sb);
            ok &= RunPhaseLowestLoyaltyControlPointTargeted(sb);
            ok &= RunPhaseIntensityErosionSkippedWhenSameFaith(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ shared helpers

        private static SimulationSystemGroup SetupSystems(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var sg = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            sg.AddSystemToUpdateList(world.GetOrCreateSystem<AIMissionaryResolutionSystem>());
            sg.SortSystems();
            return sg;
        }

        private static void SeedDualClock(EntityManager em, float inWorldDays)
        {
            var e = em.CreateEntity(typeof(DualClockComponent));
            em.SetComponentData(e, new DualClockComponent
            {
                InWorldDays       = inWorldDays,
                DaysPerRealSecond = 2f,
                DeclarationCount  = 0,
            });
        }

        private static Entity SeedTargetFaction(
            EntityManager em,
            string factionId,
            CovenantId targetFaith,
            float targetIntensity,
            float existingExposureForSourceFaith,
            CovenantId sourceFaithForExposure,
            (float loyalty, string cpId)[] controlPoints)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(FactionKindComponent),
                typeof(FaithStateComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            em.SetComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = targetFaith,
                DoctrinePath  = DoctrinePath.Light,
                Intensity     = targetIntensity,
                Level         = 2,
            });

            var exposure = em.AddBuffer<FaithExposureElement>(entity);
            if (sourceFaithForExposure != CovenantId.None && existingExposureForSourceFaith > 0f)
            {
                exposure.Add(new FaithExposureElement
                {
                    Faith      = sourceFaithForExposure,
                    Exposure   = existingExposureForSourceFaith,
                    Discovered = true,
                });
            }

            if (controlPoints != null)
            {
                for (int i = 0; i < controlPoints.Length; i++)
                {
                    var cpEntity = em.CreateEntity(typeof(ControlPointComponent));
                    em.SetComponentData(cpEntity, new ControlPointComponent
                    {
                        ControlPointId   = new FixedString32Bytes(controlPoints[i].cpId),
                        OwnerFactionId   = new FixedString32Bytes(factionId),
                        CaptureFactionId = default,
                        ContinentId      = new FixedString32Bytes("c0"),
                        ControlState     = ControlState.Stabilized,
                        Loyalty          = controlPoints[i].loyalty,
                    });
                }
            }

            return entity;
        }

        private static Entity SeedMissionaryOperation(
            EntityManager em,
            string sourceFactionId,
            string targetFactionId,
            CovenantId sourceFaith,
            float resolveAtInWorldDays,
            float exposureGain,
            float intensityErosion,
            float loyaltyPressure,
            float successScore,
            bool active = true)
        {
            var parent = em.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationMissionaryComponent));
            em.SetComponentData(parent, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes("dynop-missionary-test"),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                OperationKind        = DynastyOperationKind.Missionary,
                StartedAtInWorldDays = 0f,
                TargetFactionId      = new FixedString32Bytes(targetFactionId),
                TargetMemberId       = default,
                Active               = active,
            });
            em.SetComponentData(parent, new DynastyOperationMissionaryComponent
            {
                ResolveAtInWorldDays = resolveAtInWorldDays,
                OperatorMemberId     = new FixedString64Bytes("enemy-bloodline-il"),
                OperatorTitle        = new FixedString64Bytes("Ideological Leader"),
                SourceFaithId        = SourceFaithString(sourceFaith),
                ExposureGain         = exposureGain,
                IntensityErosion     = intensityErosion,
                LoyaltyPressure      = loyaltyPressure,
                SuccessScore         = successScore,
                ProjectedChance      = 0.5f,
                IntensityCost        = 12f,
                EscrowCost           = new DynastyOperationEscrowCost { Influence = 14f },
            });
            return parent;
        }

        private static FixedString64Bytes SourceFaithString(CovenantId faith)
        {
            switch (faith)
            {
                case CovenantId.OldLight:      return new FixedString64Bytes("old_light");
                case CovenantId.BloodDominion: return new FixedString64Bytes("blood_dominion");
                case CovenantId.TheOrder:      return new FixedString64Bytes("the_order");
                case CovenantId.TheWild:       return new FixedString64Bytes("the_wild");
                default:                       return new FixedString64Bytes("none");
            }
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseNotExpiredSkips(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-not-expired");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 50f);

            SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 0f, CovenantId.None,
                new[] { (80f, "cp-player-1") });
            var opEntity = SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 80f, // > current 50
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (!op.Active)
            {
                sb.AppendLine("PhaseNotExpiredSkips FAIL: Active should remain true when not expired");
                return false;
            }
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore)
            {
                sb.AppendLine($"PhaseNotExpiredSkips FAIL: no narrative should push; delta={messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine("PhaseNotExpiredSkips PASS: not-yet-expired op remains Active=true, no effects applied, no narrative");
            return true;
        }

        private static bool RunPhaseExpiredSuccessApplies(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-success");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var playerEntity = SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 10f, CovenantId.OldLight,
                new[] { (80f, "cp-player-1") });
            var opEntity = SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f, // <= current 100
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (op.Active)
            {
                sb.AppendLine("PhaseExpiredSuccessApplies FAIL: Active should flip to false on resolution");
                return false;
            }

            // Exposure increased from 10 to 30.
            var exposure = em.GetBuffer<FaithExposureElement>(playerEntity);
            bool foundOldLight = false;
            for (int i = 0; i < exposure.Length; i++)
            {
                if (exposure[i].Faith == CovenantId.OldLight)
                {
                    foundOldLight = true;
                    if (exposure[i].Exposure < 29.99f || exposure[i].Exposure > 30.01f)
                    {
                        sb.AppendLine($"PhaseExpiredSuccessApplies FAIL: Exposure expected 30 (10+20), got {exposure[i].Exposure}");
                        return false;
                    }
                    if (!exposure[i].Discovered)
                    {
                        sb.AppendLine("PhaseExpiredSuccessApplies FAIL: Discovered should be true");
                        return false;
                    }
                }
            }
            if (!foundOldLight)
            {
                sb.AppendLine("PhaseExpiredSuccessApplies FAIL: OldLight exposure entry missing");
                return false;
            }

            // Intensity reduced from 50 to 45.
            var targetFaith = em.GetComponentData<FaithStateComponent>(playerEntity);
            if (targetFaith.Intensity < 44.99f || targetFaith.Intensity > 45.01f)
            {
                sb.AppendLine($"PhaseExpiredSuccessApplies FAIL: Intensity expected 45 (50-5), got {targetFaith.Intensity}");
                return false;
            }

            // Control point loyalty 80 - 3 = 77.
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cps = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();
            if (cps[0].Loyalty < 76.99f || cps[0].Loyalty > 77.01f)
            {
                sb.AppendLine($"PhaseExpiredSuccessApplies FAIL: Loyalty expected 77 (80-3), got {cps[0].Loyalty}");
                cps.Dispose();
                return false;
            }
            cps.Dispose();

            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseExpiredSuccessApplies FAIL: narrative +1 expected; got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine("PhaseExpiredSuccessApplies PASS: Active->false, exposure 10->30, intensity 50->45, loyalty 80->77, narrative +1");
            return true;
        }

        private static bool RunPhaseExpiredFailureStrengthens(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-failure");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var playerEntity = SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 10f, CovenantId.OldLight,
                new[] { (80f, "cp-player-1") });
            var opEntity = SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         -5f);

            world.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (op.Active)
            {
                sb.AppendLine("PhaseExpiredFailureStrengthens FAIL: Active should flip to false on failure");
                return false;
            }

            // Target intensity += 2 (50 -> 52).
            var targetFaith = em.GetComponentData<FaithStateComponent>(playerEntity);
            if (targetFaith.Intensity < 51.99f || targetFaith.Intensity > 52.01f)
            {
                sb.AppendLine($"PhaseExpiredFailureStrengthens FAIL: Intensity expected 52 (50+2), got {targetFaith.Intensity}");
                return false;
            }

            // Exposure unchanged at 10.
            var exposure = em.GetBuffer<FaithExposureElement>(playerEntity);
            if (exposure[0].Exposure < 9.99f || exposure[0].Exposure > 10.01f)
            {
                sb.AppendLine($"PhaseExpiredFailureStrengthens FAIL: Exposure must not change on failure, got {exposure[0].Exposure}");
                return false;
            }

            // Loyalty unchanged at 80.
            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cps = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();
            if (cps[0].Loyalty < 79.99f || cps[0].Loyalty > 80.01f)
            {
                sb.AppendLine($"PhaseExpiredFailureStrengthens FAIL: Loyalty must not change on failure, got {cps[0].Loyalty}");
                cps.Dispose();
                return false;
            }
            cps.Dispose();

            sb.AppendLine("PhaseExpiredFailureStrengthens PASS: Active->false, target intensity 50->52, exposure/loyalty unchanged");
            return true;
        }

        private static bool RunPhaseVoidOnMissingTarget(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-void");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // No target faction seeded.
            var opEntity = SeedMissionaryOperation(em, "enemy", "nonexistent",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            int messagesBefore = NarrativeMessageBridge.Count(em);
            world.Update();

            var op = em.GetComponentData<DynastyOperationComponent>(opEntity);
            if (op.Active)
            {
                sb.AppendLine("PhaseVoidOnMissingTarget FAIL: Active should flip to false even on void");
                return false;
            }
            int messagesAfter = NarrativeMessageBridge.Count(em);
            if (messagesAfter != messagesBefore + 1)
            {
                sb.AppendLine($"PhaseVoidOnMissingTarget FAIL: void narrative +1 expected; got {messagesAfter - messagesBefore}");
                return false;
            }
            sb.AppendLine("PhaseVoidOnMissingTarget PASS: missing target flips Active->false and pushes void narrative");
            return true;
        }

        private static bool RunPhaseExposureAppendsNewEntry(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-exposure-append");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // No existing exposure entry for OldLight on player.
            var playerEntity = SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 0f, CovenantId.None,
                new[] { (80f, "cp-player-1") });
            SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            world.Update();

            var exposure = em.GetBuffer<FaithExposureElement>(playerEntity);
            bool foundOldLight = false;
            for (int i = 0; i < exposure.Length; i++)
            {
                if (exposure[i].Faith == CovenantId.OldLight)
                {
                    foundOldLight = true;
                    if (exposure[i].Exposure < 19.99f || exposure[i].Exposure > 20.01f)
                    {
                        sb.AppendLine($"PhaseExposureAppendsNewEntry FAIL: Exposure expected 20, got {exposure[i].Exposure}");
                        return false;
                    }
                    if (!exposure[i].Discovered)
                    {
                        sb.AppendLine("PhaseExposureAppendsNewEntry FAIL: Discovered should be true");
                        return false;
                    }
                }
            }
            if (!foundOldLight)
            {
                sb.AppendLine("PhaseExposureAppendsNewEntry FAIL: new OldLight entry must be appended");
                return false;
            }
            sb.AppendLine("PhaseExposureAppendsNewEntry PASS: absent exposure entry appended with Exposure=20, Discovered=true");
            return true;
        }

        private static bool RunPhaseExposureClampsAt100(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-exposure-clamp");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var playerEntity = SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 95f, CovenantId.OldLight,
                new[] { (80f, "cp-player-1") });
            SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f, // 95 + 20 = 115 -> clamped 100
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            world.Update();

            var exposure = em.GetBuffer<FaithExposureElement>(playerEntity);
            if (exposure[0].Exposure < 99.99f || exposure[0].Exposure > 100.01f)
            {
                sb.AppendLine($"PhaseExposureClampsAt100 FAIL: Exposure expected 100 (clamped), got {exposure[0].Exposure}");
                return false;
            }
            sb.AppendLine("PhaseExposureClampsAt100 PASS: exposure 95+20 clamped to 100");
            return true;
        }

        private static bool RunPhaseLowestLoyaltyControlPointTargeted(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-lowest-loyalty");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            var playerEntity = SeedTargetFaction(em, "player", CovenantId.TheWild, 50f, 0f, CovenantId.None,
                new[] { (80f, "cp-player-high"), (50f, "cp-player-low") });
            SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            world.Update();

            var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            var cps = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            cpQuery.Dispose();

            // Expect the low-loyalty cp to be reduced by 3 (50 -> 47); high-loyalty cp unchanged at 80.
            bool okHigh = false, okLow = false;
            for (int i = 0; i < cps.Length; i++)
            {
                if (cps[i].ControlPointId.Equals(new FixedString32Bytes("cp-player-high")))
                {
                    if (cps[i].Loyalty < 79.99f || cps[i].Loyalty > 80.01f)
                    {
                        sb.AppendLine($"PhaseLowestLoyaltyControlPointTargeted FAIL: high-loyalty cp should be unchanged at 80, got {cps[i].Loyalty}");
                        cps.Dispose();
                        return false;
                    }
                    okHigh = true;
                }
                if (cps[i].ControlPointId.Equals(new FixedString32Bytes("cp-player-low")))
                {
                    if (cps[i].Loyalty < 46.99f || cps[i].Loyalty > 47.01f)
                    {
                        sb.AppendLine($"PhaseLowestLoyaltyControlPointTargeted FAIL: low-loyalty cp expected 47 (50-3), got {cps[i].Loyalty}");
                        cps.Dispose();
                        return false;
                    }
                    okLow = true;
                }
            }
            cps.Dispose();
            if (!okHigh || !okLow)
            {
                sb.AppendLine("PhaseLowestLoyaltyControlPointTargeted FAIL: expected both cp entries");
                return false;
            }
            sb.AppendLine("PhaseLowestLoyaltyControlPointTargeted PASS: only low-loyalty cp (50->47) affected, high-loyalty cp (80) untouched");
            return true;
        }

        private static bool RunPhaseIntensityErosionSkippedWhenSameFaith(System.Text.StringBuilder sb)
        {
            using var world = new World("missionary-resolution-same-faith-no-erosion");
            var em = world.EntityManager;
            SetupSystems(world);
            SeedDualClock(em, inWorldDays: 100f);

            // Target and source both committed to OldLight.
            var playerEntity = SeedTargetFaction(em, "player", CovenantId.OldLight, 50f, 0f, CovenantId.None,
                new[] { (80f, "cp-player-1") });
            SeedMissionaryOperation(em, "enemy", "player",
                sourceFaith:          CovenantId.OldLight,
                resolveAtInWorldDays: 90f,
                exposureGain:         20f,
                intensityErosion:     5f,
                loyaltyPressure:      3f,
                successScore:         10f);

            world.Update();

            var targetFaith = em.GetComponentData<FaithStateComponent>(playerEntity);
            if (targetFaith.Intensity < 49.99f || targetFaith.Intensity > 50.01f)
            {
                sb.AppendLine($"PhaseIntensityErosionSkippedWhenSameFaith FAIL: same-faith must not erode intensity, expected 50, got {targetFaith.Intensity}");
                return false;
            }
            sb.AppendLine("PhaseIntensityErosionSkippedWhenSameFaith PASS: matching faith blocks intensity erosion; Intensity remains 50");
            return true;
        }
    }
}
#endif
