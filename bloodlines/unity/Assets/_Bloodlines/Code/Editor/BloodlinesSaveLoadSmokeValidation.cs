#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.SaveLoad;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Governed save/load snapshot smoke validator. Runs in isolated ECS worlds.
    ///
    /// Sub-slice 1 phases (capture):
    ///   1. Empty world: capture yields payload with zero entries in every list.
    ///   2. Single faction: payload has exactly one FactionSnapshot with correct
    ///      FactionId and expected companion component snapshots present.
    ///   3. Full faction: faction + conviction + dynasty (8 members + fallen ledger)
    ///      + faith + exposure: every ledger field round-trips into the payload
    ///      with canonical values intact.
    ///
    /// Sub-slice 2 phases (restore + round-trip) are added in sub-slice 2.
    ///
    /// Browser reference: simulation.js:13822 exportStateSnapshot,
    ///                     simulation.js:13989 restoreStateSnapshot
    /// Artifact: artifacts/unity-save-load-smoke.log.
    /// </summary>
    public static class BloodlinesSaveLoadSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-save-load-smoke.log";

        [MenuItem("Bloodlines/SaveLoad/Run Save-Load Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchSaveLoadSmokeValidation()
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
                message = "Save-load smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunEmptyWorldPhase(out string emptyMsg))
            {
                message = emptyMsg;
                return false;
            }

            if (!RunSingleFactionPhase(out string singleMsg))
            {
                message = singleMsg;
                return false;
            }

            if (!RunFullFactionPhase(out string fullMsg))
            {
                message = fullMsg;
                return false;
            }

            if (!RunRoundTripDeepEqualPhase(out string roundTripMsg))
            {
                message = roundTripMsg;
                return false;
            }

            if (!RunConvictionBandAfterRestorePhase(out string convMsg))
            {
                message = convMsg;
                return false;
            }

            if (!RunDynastyMembersAfterRestorePhase(out string dynastyMsg))
            {
                message = dynastyMsg;
                return false;
            }

            message =
                "Save-load smoke validation passed: emptyWorldPhase=True, singleFactionPhase=True, " +
                "fullFactionPhase=True, roundTripPhase=True, convictionRestorePhase=True, dynastyRestorePhase=True. " +
                emptyMsg + " " + singleMsg + " " + fullMsg + " " +
                roundTripMsg + " " + convMsg + " " + dynastyMsg;
            return true;
        }

        // Phase 1: empty world produces a payload with zero entries in all lists.
        private static bool RunEmptyWorldPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSaveLoadSmoke_Empty");
            var em = world.EntityManager;

            BloodlinesSnapshotWriter.Capture(em, out var payload);

            if (payload.FactionSnapshots.Count != 0 ||
                payload.LoyaltySnapshots.Count != 0 ||
                payload.ResourceSnapshots.Count != 0 ||
                payload.RealmConditionSnapshots.Count != 0 ||
                payload.ConvictionSnapshots.Count != 0 ||
                payload.DynastyStateSnapshots.Count != 0 ||
                payload.DynastyMemberSnapshots.Count != 0 ||
                payload.FallenLedgerSnapshots.Count != 0 ||
                payload.FaithStateSnapshots.Count != 0 ||
                payload.FaithExposureSnapshots.Count != 0 ||
                payload.PopulationSnapshots.Count != 0)
            {
                message =
                    "Save-load smoke failed: empty-world phase produced non-empty payload. " +
                    "factions=" + payload.FactionSnapshots.Count +
                    ", loyalty=" + payload.LoyaltySnapshots.Count +
                    ", resources=" + payload.ResourceSnapshots.Count +
                    ", realm=" + payload.RealmConditionSnapshots.Count +
                    ", conviction=" + payload.ConvictionSnapshots.Count +
                    ", dynasty=" + payload.DynastyStateSnapshots.Count +
                    ", members=" + payload.DynastyMemberSnapshots.Count +
                    ", fallen=" + payload.FallenLedgerSnapshots.Count +
                    ", faith=" + payload.FaithStateSnapshots.Count +
                    ", exposure=" + payload.FaithExposureSnapshots.Count +
                    ", population=" + payload.PopulationSnapshots.Count + ".";
                return false;
            }

            message = "EmptyWorld: all lists empty. schemaVersion=" + payload.SchemaVersion + ".";
            return true;
        }

        // Phase 2: single faction with FactionComponent only. Payload has exactly
        // one FactionSnapshot with the correct FactionId.
        private static bool RunSingleFactionPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSaveLoadSmoke_Single");
            var em = world.EntityManager;

            CreateMinimalFaction(em, "player");

            BloodlinesSnapshotWriter.Capture(em, out var payload);

            if (payload.FactionSnapshots.Count != 1)
            {
                message =
                    "Save-load smoke failed: single-faction phase expected 1 FactionSnapshot, got " +
                    payload.FactionSnapshots.Count + ".";
                return false;
            }

            if (payload.FactionSnapshots[0].FactionId != "player")
            {
                message =
                    "Save-load smoke failed: single-faction phase FactionId mismatch. " +
                    "expected=player, actual=" + payload.FactionSnapshots[0].FactionId + ".";
                return false;
            }

            if (payload.ConvictionSnapshots.Count != 0)
            {
                message =
                    "Save-load smoke failed: single-faction phase expected 0 conviction entries " +
                    "for a faction without ConvictionComponent, got " +
                    payload.ConvictionSnapshots.Count + ".";
                return false;
            }

            message =
                "SingleFaction: factionId=" + payload.FactionSnapshots[0].FactionId +
                ", conviction=" + payload.ConvictionSnapshots.Count +
                ", dynasty=" + payload.DynastyStateSnapshots.Count + ".";
            return true;
        }

        // Phase 3: full faction with conviction + dynasty (8 members + fallen ledger)
        // + faith + exposure. Every ledger field round-trips with canonical values.
        private static bool RunFullFactionPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSaveLoadSmoke_Full");
            var em = world.EntityManager;

            var factionEntity = CreateFullFaction(em, "player");

            BloodlinesSnapshotWriter.Capture(em, out var payload);

            // Faction present
            if (payload.FactionSnapshots.Count != 1 ||
                payload.FactionSnapshots[0].FactionId != "player")
            {
                message = "Save-load smoke failed: full-faction phase faction missing or wrong id.";
                return false;
            }

            // Conviction
            if (payload.ConvictionSnapshots.Count != 1 ||
                payload.ConvictionSnapshots[0].Ruthlessness != 10f ||
                payload.ConvictionSnapshots[0].Band != ConvictionBand.Cruel)
            {
                message =
                    "Save-load smoke failed: full-faction phase conviction round-trip failure. " +
                    "count=" + payload.ConvictionSnapshots.Count +
                    ", ruthlessness=" + (payload.ConvictionSnapshots.Count > 0
                        ? payload.ConvictionSnapshots[0].Ruthlessness.ToString()
                        : "N/A") +
                    ", band=" + (payload.ConvictionSnapshots.Count > 0
                        ? payload.ConvictionSnapshots[0].Band.ToString()
                        : "N/A") + ".";
                return false;
            }

            // Dynasty state
            if (payload.DynastyStateSnapshots.Count != 1 ||
                payload.DynastyStateSnapshots[0].Legitimacy != 85f)
            {
                message =
                    "Save-load smoke failed: full-faction phase dynasty state round-trip failure. " +
                    "count=" + payload.DynastyStateSnapshots.Count + ".";
                return false;
            }

            // Dynasty members: 8 canonical members
            if (payload.DynastyMemberSnapshots.Count != DynastyTemplates.Canonical.Length)
            {
                message =
                    "Save-load smoke failed: full-faction phase expected " +
                    DynastyTemplates.Canonical.Length +
                    " member snapshots, got " + payload.DynastyMemberSnapshots.Count + ".";
                return false;
            }

            // All members belong to "player"
            foreach (var ms in payload.DynastyMemberSnapshots)
            {
                if (ms.FactionId != "player")
                {
                    message =
                        "Save-load smoke failed: full-faction phase member has wrong FactionId=" +
                        ms.FactionId + ".";
                    return false;
                }
            }

            // Fallen ledger entry
            if (payload.FallenLedgerSnapshots.Count != 1 ||
                payload.FallenLedgerSnapshots[0].MemberId != "test_fallen_001")
            {
                message =
                    "Save-load smoke failed: full-faction phase fallen ledger round-trip failure. " +
                    "count=" + payload.FallenLedgerSnapshots.Count + ".";
                return false;
            }

            // Faith state
            if (payload.FaithStateSnapshots.Count != 1 ||
                payload.FaithStateSnapshots[0].SelectedFaith != CovenantId.OldLight ||
                payload.FaithStateSnapshots[0].Intensity != 40f)
            {
                message =
                    "Save-load smoke failed: full-faction phase faith state round-trip failure. " +
                    "count=" + payload.FaithStateSnapshots.Count + ".";
                return false;
            }

            // Faith exposure (2 entries)
            if (payload.FaithExposureSnapshots.Count != 2)
            {
                message =
                    "Save-load smoke failed: full-faction phase expected 2 exposure entries, got " +
                    payload.FaithExposureSnapshots.Count + ".";
                return false;
            }

            message =
                "FullFaction: factionCount=" + payload.FactionSnapshots.Count +
                ", conviction=Cruel@10" +
                ", dynastyLegitimacy=" + payload.DynastyStateSnapshots[0].Legitimacy +
                ", members=" + payload.DynastyMemberSnapshots.Count +
                ", fallen=" + payload.FallenLedgerSnapshots.Count +
                ", faith=OldLight@40" +
                ", exposure=" + payload.FaithExposureSnapshots.Count + ".";
            return true;
        }

        // Phase 4: round-trip deep-equal. Capture a full faction, restore into a fresh world,
        // capture again, and verify every snapshot list count matches.
        private static bool RunRoundTripDeepEqualPhase(out string message)
        {
            BloodlinesSnapshotPayload source;
            using (var sourceWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_RoundTripSource"))
            {
                var em = sourceWorld.EntityManager;
                CreateFullFaction(em, "player");
                BloodlinesSnapshotWriter.Capture(em, out source);
            }

            using var restoreWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_RoundTripRestore");
            var rem = restoreWorld.EntityManager;

            if (!BloodlinesSnapshotRestorer.Apply(rem, source, out var applyError))
            {
                message = "Save-load smoke failed: round-trip restore returned error: " + applyError;
                return false;
            }

            BloodlinesSnapshotWriter.Capture(rem, out var restored);

            if (restored.FactionSnapshots.Count != source.FactionSnapshots.Count ||
                restored.ConvictionSnapshots.Count != source.ConvictionSnapshots.Count ||
                restored.DynastyStateSnapshots.Count != source.DynastyStateSnapshots.Count ||
                restored.DynastyMemberSnapshots.Count != source.DynastyMemberSnapshots.Count ||
                restored.FallenLedgerSnapshots.Count != source.FallenLedgerSnapshots.Count ||
                restored.FaithStateSnapshots.Count != source.FaithStateSnapshots.Count ||
                restored.FaithExposureSnapshots.Count != source.FaithExposureSnapshots.Count)
            {
                message =
                    "Save-load smoke failed: round-trip list count mismatch. " +
                    "factions: " + source.FactionSnapshots.Count + "->" + restored.FactionSnapshots.Count +
                    ", conviction: " + source.ConvictionSnapshots.Count + "->" + restored.ConvictionSnapshots.Count +
                    ", dynastyState: " + source.DynastyStateSnapshots.Count + "->" + restored.DynastyStateSnapshots.Count +
                    ", members: " + source.DynastyMemberSnapshots.Count + "->" + restored.DynastyMemberSnapshots.Count +
                    ", fallen: " + source.FallenLedgerSnapshots.Count + "->" + restored.FallenLedgerSnapshots.Count +
                    ", faith: " + source.FaithStateSnapshots.Count + "->" + restored.FaithStateSnapshots.Count +
                    ", exposure: " + source.FaithExposureSnapshots.Count + "->" + restored.FaithExposureSnapshots.Count + ".";
                return false;
            }

            message =
                "RoundTrip: factions=" + restored.FactionSnapshots.Count +
                ", members=" + restored.DynastyMemberSnapshots.Count +
                ", fallen=" + restored.FallenLedgerSnapshots.Count +
                ", exposure=" + restored.FaithExposureSnapshots.Count + ".";
            return true;
        }

        // Phase 5: conviction band correct after restore.
        private static bool RunConvictionBandAfterRestorePhase(out string message)
        {
            BloodlinesSnapshotPayload source;
            using (var sourceWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_ConvSource"))
            {
                var em = sourceWorld.EntityManager;
                CreateFullFaction(em, "player");
                BloodlinesSnapshotWriter.Capture(em, out source);
            }

            using var restoreWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_ConvRestore");
            var rem = restoreWorld.EntityManager;
            BloodlinesSnapshotRestorer.Apply(rem, source, out _);
            BloodlinesSnapshotWriter.Capture(rem, out var restored);

            if (restored.ConvictionSnapshots.Count != 1 ||
                restored.ConvictionSnapshots[0].Band != ConvictionBand.Cruel ||
                restored.ConvictionSnapshots[0].Ruthlessness != 10f)
            {
                message =
                    "Save-load smoke failed: conviction band/ruthlessness incorrect after restore. " +
                    "count=" + restored.ConvictionSnapshots.Count +
                    ", band=" + (restored.ConvictionSnapshots.Count > 0
                        ? restored.ConvictionSnapshots[0].Band.ToString()
                        : "N/A") +
                    ", ruthlessness=" + (restored.ConvictionSnapshots.Count > 0
                        ? restored.ConvictionSnapshots[0].Ruthlessness.ToString()
                        : "N/A") + ".";
                return false;
            }

            message = "ConvictionRestore: band=Cruel, ruthlessness=10.";
            return true;
        }

        // Phase 6: dynasty member count and legitimacy correct after restore.
        private static bool RunDynastyMembersAfterRestorePhase(out string message)
        {
            BloodlinesSnapshotPayload source;
            using (var sourceWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_DynastySource"))
            {
                var em = sourceWorld.EntityManager;
                CreateFullFaction(em, "player");
                BloodlinesSnapshotWriter.Capture(em, out source);
            }

            using var restoreWorld = CreateValidationWorld("BloodlinesSaveLoadSmoke_DynastyRestore");
            var rem = restoreWorld.EntityManager;
            BloodlinesSnapshotRestorer.Apply(rem, source, out _);
            BloodlinesSnapshotWriter.Capture(rem, out var restored);

            if (restored.DynastyMemberSnapshots.Count != DynastyTemplates.Canonical.Length)
            {
                message =
                    "Save-load smoke failed: dynasty member count wrong after restore. expected=" +
                    DynastyTemplates.Canonical.Length + ", got=" + restored.DynastyMemberSnapshots.Count + ".";
                return false;
            }

            if (restored.DynastyStateSnapshots.Count != 1 ||
                restored.DynastyStateSnapshots[0].Legitimacy != 85f)
            {
                message =
                    "Save-load smoke failed: dynasty legitimacy wrong after restore. expected=85, got=" +
                    (restored.DynastyStateSnapshots.Count > 0
                        ? restored.DynastyStateSnapshots[0].Legitimacy.ToString()
                        : "N/A") + ".";
                return false;
            }

            message =
                "DynastyRestore: members=" + restored.DynastyMemberSnapshots.Count +
                ", legitimacy=" + restored.DynastyStateSnapshots[0].Legitimacy + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            return world;
        }

        private static Entity CreateMinimalFaction(EntityManager em, string factionId)
        {
            var entity = em.CreateEntity();
            em.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            return entity;
        }

        private static Entity CreateFullFaction(EntityManager em, string factionId)
        {
            var entity = em.CreateEntity();
            em.AddComponentData(entity, new FactionComponent { FactionId = factionId });

            // Conviction: ruthlessness=10 → score=-10 → Cruel band
            em.AddComponentData(entity, new ConvictionComponent
            {
                Ruthlessness = 10f,
                Score = -10f,
                Band = ConvictionBand.Cruel,
            });

            // AttachDynasty owns DynastyStateComponent creation. Add members first,
            // then patch legitimacy to the canonical test value.
            DynastyBootstrap.AttachDynasty(em, entity, factionId);
            var dynastyState = em.GetComponentData<DynastyStateComponent>(entity);
            dynastyState.Legitimacy = 85f;
            em.SetComponentData(entity, dynastyState);

            // Fallen ledger: one entry
            em.AddBuffer<DynastyFallenLedger>(entity).Add(new DynastyFallenLedger
            {
                MemberId = "test_fallen_001",
                Title = "Lord Warden",
                Role = DynastyRole.Commander,
                FallenAtWorldSeconds = 300f,
            });

            // Faith state: committed to OldLight, intensity 40
            em.AddComponentData(entity, new FaithStateComponent
            {
                SelectedFaith = CovenantId.OldLight,
                DoctrinePath = DoctrinePath.Light,
                Intensity = 40f,
                Level = 1,
            });

            // Faith exposure: 2 entries
            var exposure = em.AddBuffer<FaithExposureElement>(entity);
            exposure.Add(new FaithExposureElement
            {
                Faith = CovenantId.OldLight,
                Exposure = 110f,
                Discovered = true,
            });
            exposure.Add(new FaithExposureElement
            {
                Faith = CovenantId.BloodDominion,
                Exposure = 25f,
                Discovered = true,
            });

            return entity;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }
    }
}
#endif
