#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Faith;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the faith-combat-doctrine lane.
    ///
    /// Phase 1: FaithDoctrineCanon returns non-default effects for a committed covenant+path.
    /// Phase 2: Light doctrine paths produce higher stabilizationMultiplier than dark for
    ///          the same covenant (canonical design: light = territorial, dark = aggressive).
    /// Phase 3: Dark doctrine paths produce higher captureMultiplier than light for the
    ///          same covenant (canonical design: dark doctrine prioritizes conquest).
    /// </summary>
    public static class BloodlinesFaithDoctrineCombatWiringSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-faith-doctrine-combat-wiring-smoke.log";

        [MenuItem("Bloodlines/Faith/Run Faith Doctrine Combat Wiring Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchFaithDoctrineCombatWiringSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception ex)
            {
                success = false;
                message = "Faith doctrine combat wiring smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_FAITH_DOCTRINE_COMBAT_WIRING_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine + message;
            UnityEngine.Debug.Log(artifact);
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
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunCommittedFactionPhase(sb);
            ok &= RunLightStabilizationPhase(sb);
            ok &= RunDarkCapturePhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunCommittedFactionPhase(System.Text.StringBuilder sb)
        {
            // Committed factions get non-default effects.
            var committed = FaithDoctrineCanon.Resolve(CovenantId.OldLight, DoctrinePath.Light);
            var defaults = FaithDoctrineEffectsComponent.Defaults();

            if (committed.StabilizationMultiplier == defaults.StabilizationMultiplier &&
                committed.AuraAttackMultiplier == defaults.AuraAttackMultiplier)
            {
                sb.AppendLine("Phase 1 FAIL: committed faction should have non-default doctrine effects.");
                return false;
            }

            if (committed.StabilizationMultiplier <= 0f)
            {
                sb.AppendLine("Phase 1 FAIL: StabilizationMultiplier must be positive. got=" + committed.StabilizationMultiplier);
                return false;
            }

            var unbound = FaithDoctrineCanon.Resolve(CovenantId.None, DoctrinePath.Unassigned);
            if (unbound.StabilizationMultiplier != 1f || unbound.CaptureMultiplier != 1f)
            {
                sb.AppendLine("Phase 1 FAIL: unbound faction should return identity multipliers. got stabilization=" +
                              unbound.StabilizationMultiplier + " capture=" + unbound.CaptureMultiplier);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: committed faction receives non-default doctrine effects " +
                          "(OldLight/Light stabilization=" + committed.StabilizationMultiplier + " vs default=1.0).");
            return true;
        }

        private static bool RunLightStabilizationPhase(System.Text.StringBuilder sb)
        {
            // Light doctrine produces higher stabilizationMultiplier than dark across all covenants.
            var covenants = new[] { CovenantId.OldLight, CovenantId.BloodDominion, CovenantId.TheOrder, CovenantId.TheWild };

            foreach (var covenant in covenants)
            {
                var light = FaithDoctrineCanon.Resolve(covenant, DoctrinePath.Light);
                var dark = FaithDoctrineCanon.Resolve(covenant, DoctrinePath.Dark);

                if (light.StabilizationMultiplier <= dark.StabilizationMultiplier)
                {
                    sb.AppendLine("Phase 2 FAIL: " + covenant + " light stabilization should exceed dark. " +
                                  "light=" + light.StabilizationMultiplier + " dark=" + dark.StabilizationMultiplier);
                    return false;
                }
            }

            sb.AppendLine("Phase 2 PASS: light doctrine stabilizationMultiplier exceeds dark across all 4 covenants.");
            return true;
        }

        private static bool RunDarkCapturePhase(System.Text.StringBuilder sb)
        {
            // Dark doctrine produces higher captureMultiplier than light across all covenants.
            var covenants = new[] { CovenantId.OldLight, CovenantId.BloodDominion, CovenantId.TheOrder, CovenantId.TheWild };

            foreach (var covenant in covenants)
            {
                var light = FaithDoctrineCanon.Resolve(covenant, DoctrinePath.Light);
                var dark = FaithDoctrineCanon.Resolve(covenant, DoctrinePath.Dark);

                if (dark.CaptureMultiplier <= light.CaptureMultiplier)
                {
                    sb.AppendLine("Phase 3 FAIL: " + covenant + " dark captureMultiplier should exceed light. " +
                                  "dark=" + dark.CaptureMultiplier + " light=" + light.CaptureMultiplier);
                    return false;
                }
            }

            sb.AppendLine("Phase 3 PASS: dark doctrine captureMultiplier exceeds light across all 4 covenants.");
            return true;
        }
    }
}
#endif
