using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Auto-selects a covenant faith for AI factions that have not yet committed.
    ///
    /// When an AI faction has FaithStateComponent.SelectedFaith == None and its
    /// FaithExposureElement buffer contains at least one entry where Discovered is
    /// true and Exposure >= 100, this system commits to the highest-exposure
    /// qualifying faith automatically.
    ///
    /// Effects on commit:
    ///   - SelectedFaith = highest-exposure qualifying CovenantId
    ///   - DoctrinePath = Light
    ///   - Intensity = 20
    ///   - Level = 1
    ///   - Oathkeeping +2 via ConvictionScoring
    ///   - Info narrative pushed
    ///
    /// This is naturally one-shot per faction: once SelectedFaith is set the gate
    /// prevents re-entry and no timer component is needed.
    ///
    /// Browser reference: ai.js updateEnemyAi lines 1253-1260.
    /// Simulation function: chooseFaithCommitment at simulation.js line 9694.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AISuccessionCrisisConsolidationSystem))]
    public partial struct AIFaithCommitmentSystem : ISystem
    {
        private const float FaithExposureThreshold = 100f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FaithStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(FaithStateComponent));
            using var entities   = factionQuery.ToEntityArray(Allocator.Temp);
            using var factions   = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var faithStates = factionQuery.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (faithStates[i].SelectedFaith != CovenantId.None)
                    continue;
                if (!em.HasBuffer<FaithExposureElement>(entities[i]))
                    continue;

                var buffer = em.GetBuffer<FaithExposureElement>(entities[i]);

                CovenantId bestFaith   = CovenantId.None;
                float      bestExposure = 0f;
                for (int j = 0; j < buffer.Length; j++)
                {
                    var elem = buffer[j];
                    if (elem.Discovered && elem.Exposure >= FaithExposureThreshold && elem.Exposure > bestExposure)
                    {
                        bestExposure = elem.Exposure;
                        bestFaith    = elem.Faith;
                    }
                }

                if (bestFaith == CovenantId.None)
                    continue;

                var faith = faithStates[i];
                faith.SelectedFaith = bestFaith;
                faith.DoctrinePath  = DoctrinePath.Light;
                faith.Intensity     = 20f;
                faith.Level         = 1;
                em.SetComponentData(entities[i], faith);

                if (em.HasComponent<ConvictionComponent>(entities[i]))
                {
                    var conviction = em.GetComponentData<ConvictionComponent>(entities[i]);
                    ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, 2f);
                    em.SetComponentData(entities[i], conviction);
                }

                var factionId = factions[i].FactionId;
                var msg = new FixedString128Bytes();
                msg.Append(factionId);
                msg.Append((FixedString32Bytes)" aligned with a covenant faith.");
                NarrativeMessageBridge.Push(em, msg, NarrativeMessageTone.Info);
            }
        }
    }
}
