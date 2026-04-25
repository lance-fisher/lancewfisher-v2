using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active AI espionage operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetAIEspionageOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out float reportExpiresAtInWorldDays,
            out float successScore,
            out float projectedChance,
            out float escrowGold)
        {
            hasActiveOperation           = false;
            resolveAtInWorldDays         = 0f;
            reportExpiresAtInWorldDays   = 0f;
            successScore                 = 0f;
            projectedChance              = 0f;
            escrowGold                   = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationEspionageComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var ops        = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var espionages = q.ToComponentDataArray<DynastyOperationEspionageComponent>(Allocator.Temp);
            q.Dispose();

            var sourceId = new FixedString32Bytes(sourceFactionId);
            bool found   = false;

            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceId)) continue;
                if (ops[i].OperationKind != DynastyOperationKind.Espionage) continue;

                hasActiveOperation         = true;
                resolveAtInWorldDays       = espionages[i].ResolveAtInWorldDays;
                reportExpiresAtInWorldDays = espionages[i].ReportExpiresAtInWorldDays;
                successScore               = espionages[i].SuccessScore;
                projectedChance            = espionages[i].ProjectedChance;
                escrowGold                 = espionages[i].EscrowGold;
                found = true;
                break;
            }

            ops.Dispose();
            espionages.Dispose();
            return found;
        }

        /// <summary>Batch entry point used by BloodlinesEspionageResolutionSmokeValidation.</summary>
        public static void RunBatchEspionageResolutionSmokeValidation()
        {
            var surface = new BloodlinesDebugCommandSurface();
            surface.TryDebugGetAIEspionageOperationState("enemy", out _, out _, out _, out _, out _, out _);
        }
    }
}
