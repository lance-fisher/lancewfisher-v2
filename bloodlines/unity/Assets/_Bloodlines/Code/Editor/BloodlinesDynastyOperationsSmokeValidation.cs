#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Smoke validator for sub-slice 18 dynasty operations foundation.
    /// Five phases cover the component shape, the BeginOperation helper,
    /// the HasCapacity per-faction gate, multi-faction cap independence,
    /// and the inactive-entity exclusion rule:
    ///
    ///   PhaseBeginOperation: call BeginOperation for source "enemy"
    ///     with kind Missionary; verify one entity exists with correct
    ///     fields (operation id, source, kind, target, member, active,
    ///     and StartedAtInWorldDays stamped from DualClock).
    ///
    ///   PhaseMultipleOperationsUnderCap: call BeginOperation four times
    ///     for "enemy" with four different kinds; verify four entities
    ///     exist and HasCapacity still returns true (4 &lt; 6).
    ///
    ///   PhaseCapReached: seed DYNASTY_OPERATION_ACTIVE_LIMIT (6) active
    ///     operations for "enemy"; verify HasCapacity returns false.
    ///
    ///   PhasePerFactionCap: seed cap-minus-one (5) active for "enemy"
    ///     and one active for "player"; verify HasCapacity("player")
    ///     returns true and CountActiveForFaction("enemy") returns 5
    ///     (cap is per-faction, not global).
    ///
    ///   PhaseInactiveExcluded: seed cap worth of entities with
    ///     Active=false; verify HasCapacity returns true (inactive
    ///     operations do not consume capacity).
    ///
    /// Browser reference: simulation.js DYNASTY_OPERATION_ACTIVE_LIMIT
    /// at line 17 plus the seven dispatch sites listed in
    /// DynastyOperationLimits.
    /// Artifact: artifacts/unity-dynasty-operations-smoke.log.
    /// </summary>
    public static class BloodlinesDynastyOperationsSmokeValidation
    {
        private const string ArtifactPath =
            "../artifacts/unity-dynasty-operations-smoke.log";

        [UnityEditor.MenuItem("Bloodlines/AI/Run Dynasty Operations Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchDynastyOperationsSmokeValidation() =>
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
                message = "Dynasty operations smoke errored: " + e;
            }

            string artifact = "BLOODLINES_DYNASTY_OPERATIONS_SMOKE " +
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
            ok &= RunPhaseBeginOperation(sb);
            ok &= RunPhaseMultipleOperationsUnderCap(sb);
            ok &= RunPhaseCapReached(sb);
            ok &= RunPhasePerFactionCap(sb);
            ok &= RunPhaseInactiveExcluded(sb);
            report = sb.ToString();
            return ok;
        }

        // ------------------------------------------------------------------ helpers

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

        private static void SeedRawOperation(
            EntityManager em,
            string operationId,
            string sourceFactionId,
            DynastyOperationKind kind,
            bool active)
        {
            var e = em.CreateEntity(typeof(DynastyOperationComponent));
            em.SetComponentData(e, new DynastyOperationComponent
            {
                OperationId          = new FixedString64Bytes(operationId),
                SourceFactionId      = new FixedString32Bytes(sourceFactionId),
                OperationKind        = kind,
                StartedAtInWorldDays = 0f,
                TargetFactionId      = default,
                TargetMemberId       = default,
                Active               = active,
            });
        }

        // ------------------------------------------------------------------ phases

        private static bool RunPhaseBeginOperation(System.Text.StringBuilder sb)
        {
            using var world = new World("dynasty-operations-begin");
            var em = world.EntityManager;

            SeedDualClock(em, inWorldDays: 12f);

            var entity = DynastyOperationLimits.BeginOperation(
                em,
                operationId:     new FixedString64Bytes("dynastyOperation:1"),
                sourceFactionId: new FixedString32Bytes("enemy"),
                kind:            DynastyOperationKind.Missionary,
                targetFactionId: new FixedString32Bytes("player"),
                targetMemberId:  default);

            if (entity == Entity.Null)
            {
                sb.AppendLine("PhaseBeginOperation FAIL: BeginOperation returned Entity.Null");
                return false;
            }
            if (!em.HasComponent<DynastyOperationComponent>(entity))
            {
                sb.AppendLine("PhaseBeginOperation FAIL: created entity missing DynastyOperationComponent");
                return false;
            }
            var op = em.GetComponentData<DynastyOperationComponent>(entity);
            if (!op.OperationId.Equals(new FixedString64Bytes("dynastyOperation:1")))
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: OperationId mismatch, got '{op.OperationId}'");
                return false;
            }
            if (!op.SourceFactionId.Equals(new FixedString32Bytes("enemy")))
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: SourceFactionId mismatch, got '{op.SourceFactionId}'");
                return false;
            }
            if (op.OperationKind != DynastyOperationKind.Missionary)
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: OperationKind mismatch, got {op.OperationKind}");
                return false;
            }
            if (!op.TargetFactionId.Equals(new FixedString32Bytes("player")))
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: TargetFactionId mismatch, got '{op.TargetFactionId}'");
                return false;
            }
            if (!op.Active)
            {
                sb.AppendLine("PhaseBeginOperation FAIL: expected Active=true");
                return false;
            }
            if (op.StartedAtInWorldDays < 11.99f || op.StartedAtInWorldDays > 12.01f)
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: expected StartedAtInWorldDays=12, got {op.StartedAtInWorldDays}");
                return false;
            }

            int count = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("enemy"));
            if (count != 1)
            {
                sb.AppendLine($"PhaseBeginOperation FAIL: CountActiveForFaction expected 1, got {count}");
                return false;
            }
            if (!DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("enemy")))
            {
                sb.AppendLine("PhaseBeginOperation FAIL: HasCapacity should be true at count 1");
                return false;
            }
            sb.AppendLine("PhaseBeginOperation PASS: entity created with correct fields, HasCapacity=true at count 1");
            return true;
        }

        private static bool RunPhaseMultipleOperationsUnderCap(System.Text.StringBuilder sb)
        {
            using var world = new World("dynasty-operations-multiple-under-cap");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 5f);

            DynastyOperationLimits.BeginOperation(em,
                new FixedString64Bytes("dynastyOperation:1"),
                new FixedString32Bytes("enemy"),
                DynastyOperationKind.Missionary);
            DynastyOperationLimits.BeginOperation(em,
                new FixedString64Bytes("dynastyOperation:2"),
                new FixedString32Bytes("enemy"),
                DynastyOperationKind.HolyWar);
            DynastyOperationLimits.BeginOperation(em,
                new FixedString64Bytes("dynastyOperation:3"),
                new FixedString32Bytes("enemy"),
                DynastyOperationKind.CaptiveRescue);
            DynastyOperationLimits.BeginOperation(em,
                new FixedString64Bytes("dynastyOperation:4"),
                new FixedString32Bytes("enemy"),
                DynastyOperationKind.CaptiveRansom);

            int count = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("enemy"));
            if (count != 4)
            {
                sb.AppendLine($"PhaseMultipleOperationsUnderCap FAIL: expected count=4, got {count}");
                return false;
            }
            if (!DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("enemy")))
            {
                sb.AppendLine("PhaseMultipleOperationsUnderCap FAIL: HasCapacity should be true at count 4");
                return false;
            }
            sb.AppendLine("PhaseMultipleOperationsUnderCap PASS: 4 entities created, HasCapacity=true (4 < 6)");
            return true;
        }

        private static bool RunPhaseCapReached(System.Text.StringBuilder sb)
        {
            using var world = new World("dynasty-operations-cap-reached");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 0f);

            for (int i = 0; i < DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT; i++)
            {
                SeedRawOperation(em,
                    $"dynastyOperation:{i}",
                    "enemy",
                    DynastyOperationKind.Missionary,
                    active: true);
            }

            int count = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("enemy"));
            if (count != DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT)
            {
                sb.AppendLine($"PhaseCapReached FAIL: expected count={DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT}, got {count}");
                return false;
            }
            if (DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("enemy")))
            {
                sb.AppendLine("PhaseCapReached FAIL: HasCapacity should be false at cap");
                return false;
            }
            sb.AppendLine($"PhaseCapReached PASS: {count} entities seeded, HasCapacity=false at cap");
            return true;
        }

        private static bool RunPhasePerFactionCap(System.Text.StringBuilder sb)
        {
            using var world = new World("dynasty-operations-per-faction-cap");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 0f);

            int enemyCount = DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT - 1;
            for (int i = 0; i < enemyCount; i++)
            {
                SeedRawOperation(em,
                    $"dynastyOperation:enemy-{i}",
                    "enemy",
                    DynastyOperationKind.Missionary,
                    active: true);
            }
            SeedRawOperation(em,
                "dynastyOperation:player-0",
                "player",
                DynastyOperationKind.HolyWar,
                active: true);

            int enemyActive = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("enemy"));
            int playerActive = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("player"));
            if (enemyActive != enemyCount)
            {
                sb.AppendLine($"PhasePerFactionCap FAIL: enemy count expected {enemyCount}, got {enemyActive}");
                return false;
            }
            if (playerActive != 1)
            {
                sb.AppendLine($"PhasePerFactionCap FAIL: player count expected 1, got {playerActive}");
                return false;
            }
            if (!DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("player")))
            {
                sb.AppendLine("PhasePerFactionCap FAIL: HasCapacity(player) should be true at count 1");
                return false;
            }
            if (!DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("enemy")))
            {
                sb.AppendLine($"PhasePerFactionCap FAIL: HasCapacity(enemy) should be true at count {enemyCount} (< cap {DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT})");
                return false;
            }
            sb.AppendLine($"PhasePerFactionCap PASS: enemy={enemyActive}, player={playerActive}, both below cap, per-faction scoping holds");
            return true;
        }

        private static bool RunPhaseInactiveExcluded(System.Text.StringBuilder sb)
        {
            using var world = new World("dynasty-operations-inactive-excluded");
            var em = world.EntityManager;
            SeedDualClock(em, inWorldDays: 0f);

            for (int i = 0; i < DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT; i++)
            {
                SeedRawOperation(em,
                    $"dynastyOperation:inactive-{i}",
                    "enemy",
                    DynastyOperationKind.Missionary,
                    active: false);
            }

            int count = DynastyOperationLimits.CountActiveForFaction(
                em, new FixedString32Bytes("enemy"));
            if (count != 0)
            {
                sb.AppendLine($"PhaseInactiveExcluded FAIL: expected count=0 (all inactive), got {count}");
                return false;
            }
            if (!DynastyOperationLimits.HasCapacity(em, new FixedString32Bytes("enemy")))
            {
                sb.AppendLine("PhaseInactiveExcluded FAIL: HasCapacity should be true with all entries inactive");
                return false;
            }
            sb.AppendLine("PhaseInactiveExcluded PASS: cap-worth of inactive entities excluded, HasCapacity=true");
            return true;
        }
    }
}
#endif
