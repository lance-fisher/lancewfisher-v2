using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Resolves DynastyOperationKind.CounterIntelligence operations that have reached
    /// their ResolveAtInWorldDays threshold.
    ///
    /// Browser counterpart: tickDynastyOperations counter_intelligence branch
    /// (simulation.js:5650-5679).
    ///
    /// Resolution is deterministic (no success roll):
    ///   - Adds a DynastyCounterIntelligenceWatchElement to the source faction entity
    ///     (expires at elapsed + 150 real-seconds; capped at 2 simultaneous watches).
    ///   - Conviction: Stewardship +1 on source.
    ///   - Narrative: "[source] establishes a counter-intelligence watch."
    ///   - Voids silently if source faction entity is not found.
    ///
    /// Always flips op.Active = false after resolving.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICounterIntelligenceExecutionSystem))]
    public partial struct AICounterIntelligenceResolutionSystem : ISystem
    {
        private const float WatchDurationSeconds = 150f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyOperationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            double elapsed    = SystemAPI.Time.ElapsedTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                var q = em.CreateEntityQuery(
                    ComponentType.ReadOnly<DynastyOperationComponent>(),
                    ComponentType.ReadOnly<DynastyOperationCounterIntelligenceComponent>());
                var opEntities = q.ToEntityArray(Allocator.Temp);
                var ops        = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
                var cis        = q.ToComponentDataArray<DynastyOperationCounterIntelligenceComponent>(Allocator.Temp);
                q.Dispose();

                for (int i = 0; i < opEntities.Length; i++)
                {
                    if (!ops[i].Active) continue;
                    if (ops[i].OperationKind != DynastyOperationKind.CounterIntelligence) continue;
                    if (cis[i].ResolveAtInWorldDays > inWorldDays) continue;

                    ResolveOperation(em, ref ecb, opEntities[i], ops[i], cis[i], elapsed);
                }

                opEntities.Dispose();
                ops.Dispose();
                cis.Dispose();

                ecb.Playback(em);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        // ------------------------------------------------------------------ resolution

        private static void ResolveOperation(
            EntityManager em,
            ref EntityCommandBuffer ecb,
            Entity opEntity,
            in DynastyOperationComponent op,
            in DynastyOperationCounterIntelligenceComponent ci,
            double elapsed)
        {
            var sourceEntity = FindFactionEntity(em, ci.SourceFactionId);

            var finalOp = op;
            finalOp.Active = false;
            ecb.SetComponent(opEntity, finalOp);

            if (sourceEntity == Entity.Null) return;

            // ------------------------------------------------------------------ establish watch

            if (!em.HasBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity))
                em.AddBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity);

            var watches = em.GetBuffer<DynastyCounterIntelligenceWatchElement>(sourceEntity);

            // Prune expired entries (browser: filters by expiresAt > elapsed).
            for (int w = watches.Length - 1; w >= 0; w--)
            {
                if (watches[w].WatchExpiresAtElapsed <= (float)elapsed)
                    watches.RemoveAt(w);
            }

            // Cap at 2 (browser: watches.slice(0, 2)).
            if (watches.Length < 2)
            {
                watches.Insert(0, new DynastyCounterIntelligenceWatchElement
                {
                    TargetFactionId       = ci.TargetFactionId,
                    WatchExpiresAtElapsed = (float)elapsed + WatchDurationSeconds,
                });
            }

            // ------------------------------------------------------------------ conviction: Stewardship +1

            if (em.HasComponent<ConvictionComponent>(sourceEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(sourceEntity);
                ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Stewardship, 1f);
                em.SetComponentData(sourceEntity, conviction);
            }

            // ------------------------------------------------------------------ narrative

            var message = new FixedString128Bytes();
            message.Append(ci.SourceFactionId);
            message.Append((FixedString32Bytes)" establishes a counter-intelligence watch.");
            NarrativeMessageBridge.Push(em, message, NarrativeMessageTone.Info);
        }

        // ------------------------------------------------------------------ helpers

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            Entity result = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    result = entities[i];
                    break;
                }
            }
            entities.Dispose();
            factions.Dispose();
            return result;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
