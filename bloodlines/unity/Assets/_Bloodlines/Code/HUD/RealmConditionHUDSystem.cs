using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Faith;
using Bloodlines.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects the current faction realm state into a player-facing HUD snapshot that
    /// mirrors the browser runtime's realm-condition legibility priorities:
    /// cycle, population pressure, food, water, loyalty, conviction, and faith.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ConvictionScoringSystem))]
    [UpdateAfter(typeof(FaithIntensityResolveSystem))]
    [UpdateAfter(typeof(StarvationResponseSystem))]
    [UpdateAfter(typeof(CapPressureResponseSystem))]
    [UpdateAfter(typeof(StabilitySurplusResponseSystem))]
    public partial struct RealmConditionHUDSystem : ISystem
    {
        private const float DefaultCycleSeconds = 90f;
        private const float DefaultFoodGreenRatio = 1.35f;
        private const float DefaultFoodYellowRatio = 1.05f;
        private const float DefaultWaterGreenRatio = 1.35f;
        private const float DefaultWaterYellowRatio = 1.05f;
        private const float DefaultLoyaltyGreenFloor = 62f;
        private const float DefaultLoyaltyYellowFloor = 32f;
        private const float DefaultPopulationGreenCapRatio = 0.75f;
        private const float DefaultPopulationYellowCapRatio = 0.92f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RealmCycleConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (faction, entity) in SystemAPI.Query<RefRO<FactionComponent>>()
                         .WithAll<
                             PopulationComponent,
                             ResourceStockpileComponent,
                             RealmConditionComponent>()
                         .WithAll<
                             FactionLoyaltyComponent,
                             FaithStateComponent,
                             ConvictionComponent>()
                         .WithNone<RealmConditionHUDComponent>()
                         .WithEntityAccess())
            {
                ecb.AddComponent(entity, new RealmConditionHUDComponent
                {
                    FactionId = faction.ValueRO.FactionId,
                });
            }

            ecb.Playback(entityManager);

            if (!SystemAPI.TryGetSingleton(out RealmCycleConfig config))
            {
                return;
            }

            float cycleSeconds = config.CycleSeconds > 0f
                ? config.CycleSeconds
                : DefaultCycleSeconds;
            float foodGreen = config.FoodGreenRatio > 0f
                ? config.FoodGreenRatio
                : DefaultFoodGreenRatio;
            float foodYellow = config.FoodYellowRatio > 0f
                ? config.FoodYellowRatio
                : DefaultFoodYellowRatio;
            float waterGreen = config.WaterGreenRatio > 0f
                ? config.WaterGreenRatio
                : DefaultWaterGreenRatio;
            float waterYellow = config.WaterYellowRatio > 0f
                ? config.WaterYellowRatio
                : DefaultWaterYellowRatio;
            float loyaltyGreen = config.LoyaltyGreenFloor > 0f
                ? config.LoyaltyGreenFloor
                : DefaultLoyaltyGreenFloor;
            float loyaltyYellow = config.LoyaltyYellowFloor > 0f
                ? config.LoyaltyYellowFloor
                : DefaultLoyaltyYellowFloor;
            float populationGreen = config.PopulationGreenCapRatio > 0f
                ? config.PopulationGreenCapRatio
                : DefaultPopulationGreenCapRatio;
            float populationYellow = config.PopulationYellowCapRatio > 0f
                ? config.PopulationYellowCapRatio
                : DefaultPopulationYellowCapRatio;

            foreach (var (hudRw, realm, population, resources, loyalty, faith, conviction) in
                SystemAPI.Query<
                    RefRW<RealmConditionHUDComponent>,
                    RefRO<RealmConditionComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<ResourceStockpileComponent>,
                    RefRO<FactionLoyaltyComponent>,
                    RefRO<FaithStateComponent>,
                    RefRO<ConvictionComponent>>())
            {
                ref var hud = ref hudRw.ValueRW;
                var pop = population.ValueRO;
                var stockpile = resources.ValueRO;
                var realmState = realm.ValueRO;
                var loyaltyState = loyalty.ValueRO;
                var faithState = faith.ValueRO;
                var convictionState = conviction.ValueRO;

                float populationRatio = pop.Cap > 0
                    ? (float)pop.Total / pop.Cap
                    : 0f;
                float foodNeed = math.max(0f, pop.Total);
                float waterNeed = math.max(0f, pop.Total);
                float foodRatio = pop.Total > 0
                    ? stockpile.Food / pop.Total
                    : 99f;
                float waterRatio = pop.Total > 0
                    ? stockpile.Water / pop.Total
                    : 99f;
                float loyaltyMax = loyaltyState.Max > 0f ? loyaltyState.Max : 100f;

                hud.CycleCount = realmState.CycleCount;
                hud.CycleProgress = math.saturate(realmState.CycleAccumulator / cycleSeconds);

                hud.Population = pop.Total;
                hud.PopulationCap = pop.Cap;
                hud.PopulationRatio = populationRatio;
                hud.PopulationBand = ResolveLowerIsBetterBand(
                    populationRatio,
                    populationGreen,
                    populationYellow);

                hud.FoodStock = stockpile.Food;
                hud.FoodNeed = foodNeed;
                hud.FoodRatio = foodRatio;
                hud.FoodBand = ResolveHigherIsBetterBand(foodRatio, foodGreen, foodYellow);
                hud.FoodStrainStreak = realmState.FoodStrainStreak;

                hud.WaterStock = stockpile.Water;
                hud.WaterNeed = waterNeed;
                hud.WaterRatio = waterRatio;
                hud.WaterBand = ResolveHigherIsBetterBand(waterRatio, waterGreen, waterYellow);
                hud.WaterStrainStreak = realmState.WaterStrainStreak;

                hud.LoyaltyCurrent = loyaltyState.Current;
                hud.LoyaltyMax = loyaltyMax;
                hud.LoyaltyFloor = loyaltyState.Floor;
                hud.LoyaltyBand = ResolveHigherIsBetterBand(
                    loyaltyState.Current,
                    loyaltyGreen,
                    loyaltyYellow);

                hud.ConvictionScore = convictionState.Score;
                hud.ConvictionBand = convictionState.Band;
                hud.ConvictionBandLabel = ResolveConvictionBandLabel(convictionState.Band);
                hud.ConvictionBandColor = ResolveConvictionBandColor(convictionState.Band);

                hud.FaithId = faithState.SelectedFaith;
                hud.DoctrinePath = faithState.DoctrinePath;
                hud.FaithIntensity = faithState.Intensity;
                hud.FaithLevel = faithState.Level;
                hud.FaithLabel = ResolveFaithLabel(faithState.SelectedFaith);
                hud.FaithTierLabel = ResolveFaithTierLabel(faithState.Level);
                hud.DoctrineLabel = ResolveDoctrineLabel(faithState.DoctrinePath);
                hud.FaithCommitted = faithState.SelectedFaith != CovenantId.None;
                hud.FaithBand = ResolveFaithBand(faithState);
            }
        }

        private static FixedString32Bytes ResolveHigherIsBetterBand(
            float value,
            float greenThreshold,
            float yellowThreshold)
        {
            if (value >= greenThreshold)
            {
                return new FixedString32Bytes("green");
            }

            if (value >= yellowThreshold)
            {
                return new FixedString32Bytes("yellow");
            }

            return new FixedString32Bytes("red");
        }

        private static FixedString32Bytes ResolveLowerIsBetterBand(
            float value,
            float greenThreshold,
            float yellowThreshold)
        {
            if (value <= greenThreshold)
            {
                return new FixedString32Bytes("green");
            }

            if (value <= yellowThreshold)
            {
                return new FixedString32Bytes("yellow");
            }

            return new FixedString32Bytes("red");
        }

        private static FixedString32Bytes ResolveFaithBand(in FaithStateComponent faith)
        {
            if (faith.SelectedFaith == CovenantId.None)
            {
                return new FixedString32Bytes("red");
            }

            if (faith.Intensity >= FaithIntensityTiers.FerventMin)
            {
                return new FixedString32Bytes("green");
            }

            return new FixedString32Bytes("yellow");
        }

        private static FixedString32Bytes ResolveConvictionBandLabel(ConvictionBand band)
        {
            switch (band)
            {
                case ConvictionBand.ApexMoral:
                    return new FixedString32Bytes("Apex Moral");
                case ConvictionBand.Moral:
                    return new FixedString32Bytes("Moral");
                case ConvictionBand.Cruel:
                    return new FixedString32Bytes("Cruel");
                case ConvictionBand.ApexCruel:
                    return new FixedString32Bytes("Apex Cruel");
                default:
                    return new FixedString32Bytes("Neutral");
            }
        }

        private static FixedString32Bytes ResolveConvictionBandColor(ConvictionBand band)
        {
            switch (band)
            {
                case ConvictionBand.ApexMoral:
                case ConvictionBand.Moral:
                    return new FixedString32Bytes("green");
                case ConvictionBand.Cruel:
                case ConvictionBand.ApexCruel:
                    return new FixedString32Bytes("red");
                default:
                    return new FixedString32Bytes("yellow");
            }
        }

        private static FixedString32Bytes ResolveFaithLabel(CovenantId faith)
        {
            switch (faith)
            {
                case CovenantId.OldLight:
                    return new FixedString32Bytes("Old Light");
                case CovenantId.BloodDominion:
                    return new FixedString32Bytes("Blood Dominion");
                case CovenantId.TheOrder:
                    return new FixedString32Bytes("The Order");
                case CovenantId.TheWild:
                    return new FixedString32Bytes("The Wild");
                default:
                    return new FixedString32Bytes("Uncommitted");
            }
        }

        private static FixedString32Bytes ResolveFaithTierLabel(int level)
        {
            switch (level)
            {
                case FaithIntensityTiers.LatentLevel:
                    return new FixedString32Bytes("Latent");
                case FaithIntensityTiers.ActiveLevel:
                    return new FixedString32Bytes("Active");
                case FaithIntensityTiers.DevoutLevel:
                    return new FixedString32Bytes("Devout");
                case FaithIntensityTiers.FerventLevel:
                    return new FixedString32Bytes("Fervent");
                case FaithIntensityTiers.ApexLevel:
                    return new FixedString32Bytes("Apex");
                default:
                    return new FixedString32Bytes("Unawakened");
            }
        }

        private static FixedString32Bytes ResolveDoctrineLabel(DoctrinePath doctrinePath)
        {
            switch (doctrinePath)
            {
                case DoctrinePath.Light:
                    return new FixedString32Bytes("Light");
                case DoctrinePath.Dark:
                    return new FixedString32Bytes("Dark");
                default:
                    return new FixedString32Bytes("Unassigned");
            }
        }
    }
}
