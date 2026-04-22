using System.Globalization;
using System.Text;
using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetRealmConditionHUDSnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var factionEntity = FindFactionRootEntity(entityManager, factionKey);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<RealmConditionHUDComponent>(factionEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<RealmConditionHUDComponent>(factionEntity);
            var builder = new StringBuilder(512);
            builder.Append("RealmHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|CycleCount=").Append(hud.CycleCount)
                .Append("|CycleProgress=").Append(hud.CycleProgress.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|Population=").Append(hud.Population)
                .Append("|PopulationCap=").Append(hud.PopulationCap)
                .Append("|PopulationRatio=").Append(hud.PopulationRatio.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|PopulationBand=").Append(hud.PopulationBand)
                .Append("|FoodStock=").Append(hud.FoodStock.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|FoodNeed=").Append(hud.FoodNeed.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|FoodRatio=").Append(hud.FoodRatio.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|FoodBand=").Append(hud.FoodBand)
                .Append("|FoodStrainStreak=").Append(hud.FoodStrainStreak)
                .Append("|WaterStock=").Append(hud.WaterStock.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|WaterNeed=").Append(hud.WaterNeed.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|WaterRatio=").Append(hud.WaterRatio.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|WaterBand=").Append(hud.WaterBand)
                .Append("|WaterStrainStreak=").Append(hud.WaterStrainStreak)
                .Append("|LoyaltyCurrent=").Append(hud.LoyaltyCurrent.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|LoyaltyMax=").Append(hud.LoyaltyMax.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|LoyaltyFloor=").Append(hud.LoyaltyFloor.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|LoyaltyBand=").Append(hud.LoyaltyBand)
                .Append("|ConvictionScore=").Append(hud.ConvictionScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|ConvictionBand=").Append(hud.ConvictionBand)
                .Append("|ConvictionLabel=").Append(hud.ConvictionBandLabel)
                .Append("|ConvictionColor=").Append(hud.ConvictionBandColor)
                .Append("|FaithId=").Append(hud.FaithId)
                .Append("|FaithLabel=").Append(hud.FaithLabel)
                .Append("|FaithCommitted=").Append(hud.FaithCommitted ? "true" : "false")
                .Append("|DoctrinePath=").Append(hud.DoctrinePath)
                .Append("|DoctrineLabel=").Append(hud.DoctrineLabel)
                .Append("|FaithIntensity=").Append(hud.FaithIntensity.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|FaithLevel=").Append(hud.FaithLevel)
                .Append("|FaithTier=").Append(hud.FaithTierLabel)
                .Append("|FaithBand=").Append(hud.FaithBand);

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetMatchHUDSnapshot(out string readout)
        {
            readout = string.Empty;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            var hud = query.GetSingleton<MatchProgressionHUDComponent>();
            var builder = new StringBuilder(384);
            builder.Append("MatchHUD")
                .Append("|StageNumber=").Append(hud.StageNumber)
                .Append("|StageId=").Append(hud.StageId)
                .Append("|StageLabel=").Append(hud.StageLabel)
                .Append("|PhaseId=").Append(hud.PhaseId)
                .Append("|PhaseLabel=").Append(hud.PhaseLabel)
                .Append("|StageReadiness=").Append(hud.StageReadiness.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|NextStageId=").Append(hud.NextStageId)
                .Append("|NextStageLabel=").Append(hud.NextStageLabel)
                .Append("|InWorldDays=").Append(hud.InWorldDays.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("|InWorldYears=").Append(hud.InWorldYears.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|DeclarationCount=").Append(hud.DeclarationCount)
                .Append("|WorldPressureLevel=").Append(hud.WorldPressureLevel)
                .Append("|WorldPressureLabel=").Append(hud.WorldPressureLabel)
                .Append("|WorldPressureScore=").Append(hud.WorldPressureScore)
                .Append("|WorldPressureTargeted=").Append(hud.WorldPressureTargeted ? "true" : "false")
                .Append("|DominantLeaderFactionId=").Append(hud.DominantLeaderFactionId)
                .Append("|DominantTerritoryShare=").Append(hud.DominantLeaderTerritoryShare.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|GreatReckoningActive=").Append(hud.GreatReckoningActive ? "true" : "false")
                .Append("|GreatReckoningTargetFactionId=").Append(hud.GreatReckoningTargetFactionId)
                .Append("|GreatReckoningShare=").Append(hud.GreatReckoningShare.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }
    }
}
