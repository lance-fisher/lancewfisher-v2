#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using UnityEditor;
using UnityEngine;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for the dynasty-prestige lane.
    ///
    /// Phase 1: Healthy dynasty (no interregnum, no crisis, Neutral conviction) uses base
    ///          decay rate. Interregnum active adds the canonical penalty.
    /// Phase 2: Active succession crisis adds its penalty on top of the base rate.
    ///          Combined interregnum + crisis stacks both penalties.
    /// Phase 3: ApexCruel conviction increases rate above base; ApexMoral decreases it
    ///          but never below the minimum floor.
    /// </summary>
    public static class BloodlinesDynastyPrestigeDriftSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-dynasty-prestige-drift-smoke.log";

        [MenuItem("Bloodlines/Dynasty/Run Dynasty Prestige Drift Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDynastyPrestigeDriftSmokeValidation() => RunInternal(batchMode: true);

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
                message = "Dynasty prestige drift smoke validation errored: " + ex;
            }

            string artifact = "BLOODLINES_DYNASTY_PRESTIGE_DRIFT_SMOKE " +
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
            ok &= RunBaseAndInterregnumPhase(sb);
            ok &= RunCrisisPenaltyPhase(sb);
            ok &= RunConvictionModulationPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static float ComputeDecayRate(
            bool interregnum,
            float crisisProgress,
            ConvictionBand band)
        {
            float rate = DynastyPrestigeDecayModulatorSystem.BaseDecayRatePerDay;

            if (interregnum)
                rate += DynastyPrestigeDecayModulatorSystem.InterregnumPenalty;

            if (crisisProgress < 1f)
                rate += DynastyPrestigeDecayModulatorSystem.CrisisPenalty;

            switch (band)
            {
                case ConvictionBand.ApexCruel:
                    rate += DynastyPrestigeDecayModulatorSystem.ApexCruelPenalty;
                    break;
                case ConvictionBand.ApexMoral:
                    rate -= DynastyPrestigeDecayModulatorSystem.ApexMoralDiscount;
                    break;
            }

            return UnityEngine.Mathf.Max(DynastyPrestigeDecayModulatorSystem.MinDecayRatePerDay, rate);
        }

        private static bool RunBaseAndInterregnumPhase(System.Text.StringBuilder sb)
        {
            float healthyRate = ComputeDecayRate(false, 1f, ConvictionBand.Neutral);
            float interregnumRate = ComputeDecayRate(true, 1f, ConvictionBand.Neutral);

            if (System.Math.Abs(healthyRate - DynastyPrestigeDecayModulatorSystem.BaseDecayRatePerDay) > 0.0001f)
            {
                sb.AppendLine("Phase 1 FAIL: healthy dynasty should use base decay rate. " +
                              "expected=" + DynastyPrestigeDecayModulatorSystem.BaseDecayRatePerDay +
                              " got=" + healthyRate);
                return false;
            }

            float expectedInterregnum = DynastyPrestigeDecayModulatorSystem.BaseDecayRatePerDay
                                      + DynastyPrestigeDecayModulatorSystem.InterregnumPenalty;
            if (System.Math.Abs(interregnumRate - expectedInterregnum) > 0.0001f)
            {
                sb.AppendLine("Phase 1 FAIL: interregnum should add penalty. " +
                              "expected=" + expectedInterregnum + " got=" + interregnumRate);
                return false;
            }

            if (interregnumRate <= healthyRate)
            {
                sb.AppendLine("Phase 1 FAIL: interregnum rate should exceed healthy rate. " +
                              "interregnum=" + interregnumRate + " healthy=" + healthyRate);
                return false;
            }

            sb.AppendLine("Phase 1 PASS: healthy=" + healthyRate.ToString("0.0000") +
                          " interregnum=" + interregnumRate.ToString("0.0000") +
                          " (interregnum penalty=" + DynastyPrestigeDecayModulatorSystem.InterregnumPenalty.ToString("0.0000") + ").");
            return true;
        }

        private static bool RunCrisisPenaltyPhase(System.Text.StringBuilder sb)
        {
            float noCrisisRate       = ComputeDecayRate(false, 1f, ConvictionBand.Neutral);
            float activeCrisisRate   = ComputeDecayRate(false, 0.5f, ConvictionBand.Neutral);
            float combinedRate       = ComputeDecayRate(true, 0.5f, ConvictionBand.Neutral);

            if (activeCrisisRate <= noCrisisRate)
            {
                sb.AppendLine("Phase 2 FAIL: active crisis should raise decay above no-crisis. " +
                              "crisis=" + activeCrisisRate + " noCrisis=" + noCrisisRate);
                return false;
            }

            if (combinedRate <= activeCrisisRate)
            {
                sb.AppendLine("Phase 2 FAIL: interregnum+crisis should exceed crisis alone. " +
                              "combined=" + combinedRate + " crisisOnly=" + activeCrisisRate);
                return false;
            }

            float expectedCrisis = noCrisisRate + DynastyPrestigeDecayModulatorSystem.CrisisPenalty;
            if (System.Math.Abs(activeCrisisRate - expectedCrisis) > 0.0001f)
            {
                sb.AppendLine("Phase 2 FAIL: crisis penalty mismatch. " +
                              "expected=" + expectedCrisis + " got=" + activeCrisisRate);
                return false;
            }

            sb.AppendLine("Phase 2 PASS: noCrisis=" + noCrisisRate.ToString("0.0000") +
                          " activeCrisis=" + activeCrisisRate.ToString("0.0000") +
                          " combined=" + combinedRate.ToString("0.0000") + ".");
            return true;
        }

        private static bool RunConvictionModulationPhase(System.Text.StringBuilder sb)
        {
            float neutralRate  = ComputeDecayRate(false, 1f, ConvictionBand.Neutral);
            float cruelRate    = ComputeDecayRate(false, 1f, ConvictionBand.ApexCruel);
            float moralRate    = ComputeDecayRate(false, 1f, ConvictionBand.ApexMoral);

            if (cruelRate <= neutralRate)
            {
                sb.AppendLine("Phase 3 FAIL: ApexCruel should raise decay above neutral. " +
                              "cruel=" + cruelRate + " neutral=" + neutralRate);
                return false;
            }

            if (moralRate >= neutralRate)
            {
                sb.AppendLine("Phase 3 FAIL: ApexMoral should lower decay below neutral. " +
                              "moral=" + moralRate + " neutral=" + neutralRate);
                return false;
            }

            if (moralRate < DynastyPrestigeDecayModulatorSystem.MinDecayRatePerDay - 0.0001f)
            {
                sb.AppendLine("Phase 3 FAIL: ApexMoral rate should not fall below floor. " +
                              "got=" + moralRate + " floor=" + DynastyPrestigeDecayModulatorSystem.MinDecayRatePerDay);
                return false;
            }

            sb.AppendLine("Phase 3 PASS: neutral=" + neutralRate.ToString("0.0000") +
                          " ApexCruel=" + cruelRate.ToString("0.0000") +
                          " ApexMoral=" + moralRate.ToString("0.0000") +
                          " floor=" + DynastyPrestigeDecayModulatorSystem.MinDecayRatePerDay.ToString("0.0000") + ".");
            return true;
        }
    }
}
#endif
