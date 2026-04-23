using System.Globalization;
using System.Text;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.HUD;
using Bloodlines.Systems;
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

        public bool TryDebugGetFortificationHUDSnapshot(string settlementId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(settlementId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var settlementEntity = FindSettlementEntity(entityManager, settlementId);
            if (settlementEntity == Entity.Null ||
                !entityManager.HasComponent<FortificationHUDComponent>(settlementEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<FortificationHUDComponent>(settlementEntity);
            var builder = new StringBuilder(256);
            builder.Append("FortificationHUD")
                .Append("|SettlementId=").Append(hud.SettlementId)
                .Append("|OwnerFactionId=").Append(hud.OwnerFactionId)
                .Append("|Tier=").Append(hud.Tier)
                .Append("|OpenBreachCount=").Append(hud.OpenBreachCount)
                .Append("|ReserveFrontage=").Append(hud.ReserveFrontage)
                .Append("|MusteredDefenders=").Append(hud.MusteredDefenderCount)
                .Append("|ReadyReserves=").Append(hud.ReadyReserveCount)
                .Append("|RecoveringReserves=").Append(hud.RecoveringReserveCount)
                .Append("|ThreatActive=").Append(hud.ThreatActive ? "true" : "false")
                .Append("|SealingProgress01=").Append(hud.SealingProgress01.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|RecoveryProgress01=").Append(hud.RecoveryProgress01.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetVictoryReadout(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<VictoryConditionReadoutComponent>(factionEntity))
            {
                return false;
            }

            DynamicBuffer<VictoryConditionReadoutComponent> buffer =
                entityManager.GetBuffer<VictoryConditionReadoutComponent>(factionEntity);
            if (buffer.Length == 0)
            {
                return false;
            }

            var builder = new StringBuilder(384);
            for (int i = 0; i < buffer.Length; i++)
            {
                var entry = buffer[i];
                if (i > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("VictoryReadout")
                    .Append("|FactionId=").Append(factionId)
                    .Append("|ConditionId=").Append(entry.ConditionId)
                    .Append("|ProgressPct=").Append(entry.ProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|IsLeading=").Append(entry.IsLeading ? "true" : "false")
                    .Append("|TimeRemainingEstimateInWorldDays=")
                    .Append(float.IsNaN(entry.TimeRemainingEstimateInWorldDays)
                        ? "NaN"
                        : entry.TimeRemainingEstimateInWorldDays.ToString("0.000", CultureInfo.InvariantCulture));
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetVictoryLeaderboard(out string readout)
        {
            readout = string.Empty;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<VictoryLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<VictoryLeaderboardHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            DynamicBuffer<VictoryLeaderboardHUDComponent> buffer =
                entityManager.GetBuffer<VictoryLeaderboardHUDComponent>(query.GetSingletonEntity());
            if (buffer.Length == 0)
            {
                return false;
            }

            var builder = new StringBuilder(512);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("VictoryLeaderboard")
                    .Append("|Rank=").Append(i + 1)
                    .Append("|FactionId=").Append(buffer[i].FactionId)
                    .Append("|LeadingConditionId=").Append(buffer[i].LeadingConditionId)
                    .Append("|ProgressPct=").Append(buffer[i].ProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|IsHumanPlayer=").Append(buffer[i].IsHumanPlayer ? "true" : "false");
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetDynastyRenownHUDSnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<DynastyRenownHUDComponent>(factionEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<DynastyRenownHUDComponent>(factionEntity);
            var builder = new StringBuilder(384);
            builder.Append("DynastyRenownHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|Score=").Append(hud.RenownScore.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|PeakRenown=").Append(hud.PeakRenown.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|ScoreToPeakRatio=").Append(hud.ScoreToPeakRatio.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|Rank=").Append(hud.RenownRank)
                .Append("|IsLeadingDynasty=").Append(hud.IsLeadingDynasty ? "true" : "false")
                .Append("|RulerMemberId=").Append(hud.RulerMemberId)
                .Append("|RulerTitle=").Append(hud.RulerTitle)
                .Append("|Legitimacy=").Append(hud.Legitimacy.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|Interregnum=").Append(hud.Interregnum ? "true" : "false")
                .Append("|StatusLabel=").Append(hud.StatusLabel)
                .Append("|BandLabel=").Append(hud.RenownBandLabel)
                .Append("|BandColor=").Append(hud.RenownBandColor);

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetDynastyRenownLeaderboard(out string readout)
        {
            readout = string.Empty;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            DynamicBuffer<DynastyRenownLeaderboardHUDComponent> buffer =
                entityManager.GetBuffer<DynastyRenownLeaderboardHUDComponent>(query.GetSingletonEntity());
            if (buffer.Length == 0)
            {
                return false;
            }

            var builder = new StringBuilder(768);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (i > 0)
                {
                    builder.AppendLine();
                }

                builder.Append("DynastyRenownLeaderboard")
                    .Append("|Rank=").Append(i + 1)
                    .Append("|FactionId=").Append(buffer[i].FactionId)
                    .Append("|Score=").Append(buffer[i].RenownScore.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|PeakRenown=").Append(buffer[i].PeakRenown.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|RenownRank=").Append(buffer[i].RenownRank)
                    .Append("|IsHumanPlayer=").Append(buffer[i].IsHumanPlayer ? "true" : "false")
                    .Append("|IsLeadingDynasty=").Append(buffer[i].IsLeadingDynasty ? "true" : "false")
                    .Append("|Interregnum=").Append(buffer[i].Interregnum ? "true" : "false")
                    .Append("|RulerMemberId=").Append(buffer[i].RulerMemberId)
                    .Append("|RulerTitle=").Append(buffer[i].RulerTitle)
                    .Append("|BandLabel=").Append(buffer[i].BandLabel)
                    .Append("|StatusLabel=").Append(buffer[i].StatusLabel);
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetSuccessionCrisisHUDSnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<SuccessionCrisisHUDComponent>(factionEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<SuccessionCrisisHUDComponent>(factionEntity);
            var builder = new StringBuilder(256);
            builder.Append("SuccessionCrisisHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|CrisisActive=").Append(hud.CrisisActive ? "true" : "false")
                .Append("|Severity=").Append(hud.CrisisSeverity)
                .Append("|SeverityLabel=").Append(hud.SeverityLabel)
                .Append("|SeverityColor=").Append(hud.SeverityColor)
                .Append("|RecoveryProgressPct=").Append(hud.RecoveryProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|ResourceTrickleFactor=").Append(hud.ResourceTrickleFactor.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|LegitimacyDrainRatePerDay=").Append(hud.LegitimacyDrainRatePerDay.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPoliticalEventsTraySnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<PoliticalEventsTrayHUDStateComponent>(factionEntity) ||
                !entityManager.HasBuffer<PoliticalEventsTrayHUDComponent>(factionEntity))
            {
                return false;
            }

            var state = entityManager.GetComponentData<PoliticalEventsTrayHUDStateComponent>(factionEntity);
            DynamicBuffer<PoliticalEventsTrayHUDComponent> tray =
                entityManager.GetBuffer<PoliticalEventsTrayHUDComponent>(factionEntity);

            var builder = new StringBuilder(512);
            builder.Append("PoliticalEventsTrayState")
                .Append("|FactionId=").Append(state.FactionId)
                .Append("|LastRefreshInWorldDays=").Append(state.LastRefreshInWorldDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|ActiveEventCount=").Append(state.ActiveEventCount)
                .Append("|DisplayedCount=").Append(tray.Length);

            for (int i = 0; i < tray.Length; i++)
            {
                builder.AppendLine()
                    .Append("PoliticalEventsTrayEntry")
                    .Append("|Index=").Append(i + 1)
                    .Append("|EventType=").Append(tray[i].EventType)
                    .Append("|EventLabel=").Append(tray[i].EventLabel)
                    .Append("|RemainingInWorldDays=")
                    .Append(tray[i].RemainingInWorldDays.ToString("0.000", CultureInfo.InvariantCulture));
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetCovenantTestProgressHUDSnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<CovenantTestProgressHUDComponent>(factionEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<CovenantTestProgressHUDComponent>(factionEntity);
            var builder = new StringBuilder(320);
            builder.Append("CovenantTestProgressHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|LastRefreshInWorldDays=").Append(hud.LastRefreshInWorldDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|FaithId=").Append(hud.FaithId)
                .Append("|TestPhase=").Append(hud.TestPhase)
                .Append("|PhaseLabel=").Append(hud.PhaseLabel)
                .Append("|QualifyingDays=").Append(hud.QualifyingDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|RequiredDays=").Append(hud.RequiredDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|ProgressPct=").Append(hud.ProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|CooldownRemainingInWorldDays=")
                .Append(hud.CooldownRemainingInWorldDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|SuccessCount=").Append(hud.SuccessCount);

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetTruebornRiseHUDSnapshot(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes(factionId));
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<TruebornRiseHUDComponent>(factionEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<TruebornRiseHUDComponent>(factionEntity);
            var builder = new StringBuilder(320);
            builder.Append("TruebornRiseHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|LastRefreshInWorldDays=").Append(hud.LastRefreshInWorldDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|CurrentStage=").Append(hud.CurrentStage)
                .Append("|StageLabel=").Append(hud.StageLabel)
                .Append("|RiseActive=").Append(hud.RiseActive ? "true" : "false")
                .Append("|Recognized=").Append(hud.Recognized ? "true" : "false")
                .Append("|RecognizedFactionCount=").Append(hud.RecognizedFactionCount)
                .Append("|ChallengeLevel=").Append(hud.ChallengeLevel)
                .Append("|GlobalPressurePerDay=").Append(hud.GlobalPressurePerDay.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|LoyaltyErosionPerDay=").Append(hud.LoyaltyErosionPerDay.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPlayerCommandDeckHUDSnapshot(out string readout)
        {
            readout = string.Empty;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var playerEntity = FindFactionRootEntity(entityManager, new FixedString32Bytes("player"));
            if (playerEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCommandDeckHUDComponent>(playerEntity))
            {
                return false;
            }

            var hud = entityManager.GetComponentData<PlayerCommandDeckHUDComponent>(playerEntity);
            var builder = new StringBuilder(512);
            builder.Append("PlayerCommandDeckHUD")
                .Append("|FactionId=").Append(hud.FactionId)
                .Append("|StageLabel=").Append(hud.StageLabel)
                .Append("|PhaseLabel=").Append(hud.PhaseLabel)
                .Append("|WorldPressureLabel=").Append(hud.WorldPressureLabel)
                .Append("|WorldPressureLevel=").Append(hud.WorldPressureLevel)
                .Append("|GreatReckoningActive=").Append(hud.GreatReckoningActive ? "true" : "false")
                .Append("|LeadingVictoryConditionId=").Append(hud.LeadingVictoryConditionId)
                .Append("|LeadingVictoryProgressPct=").Append(hud.LeadingVictoryProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|LeadingVictoryEtaInWorldDays=")
                .Append(float.IsNaN(hud.LeadingVictoryEtaInWorldDays)
                    ? "NaN"
                    : hud.LeadingVictoryEtaInWorldDays.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|VictoryRank=").Append(hud.VictoryRank)
                .Append("|VictoryLeaderFactionId=").Append(hud.VictoryLeaderFactionId)
                .Append("|RenownRank=").Append(hud.RenownRank)
                .Append("|RenownScore=").Append(hud.RenownScore.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|RenownBandLabel=").Append(hud.RenownBandLabel)
                .Append("|PopulationBand=").Append(hud.PopulationBand)
                .Append("|LoyaltyBand=").Append(hud.LoyaltyBand)
                .Append("|FaithBand=").Append(hud.FaithBand)
                .Append("|FortificationThreatActive=").Append(hud.FortificationThreatActive ? "true" : "false")
                .Append("|PrimaryAlertLabel=").Append(hud.PrimaryAlertLabel)
                .Append("|LastRefreshInWorldDays=").Append(hud.LastRefreshInWorldDays.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetBattlefieldCommandDeckSummary(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) ||
                !TryGetEntityManager(out var entityManager) ||
                !TryBuildBattlefieldCommandDeckSummary(entityManager, factionId, out var summary))
            {
                return false;
            }

            var builder = new StringBuilder(512);
            builder.Append("CommandDeckSummary")
                .Append("|FactionId=").Append(summary.FactionId)
                .Append("|StageNumber=").Append(summary.StageNumber)
                .Append("|PhaseLabel=").Append(summary.PhaseLabel)
                .Append("|InWorldYears=").Append(summary.InWorldYears.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|DeclarationCount=").Append(summary.DeclarationCount)
                .Append("|WorldPressureLevel=").Append(summary.WorldPressureLevel)
                .Append("|WorldPressureLabel=").Append(summary.WorldPressureLabel)
                .Append("|GreatReckoningActive=").Append(summary.GreatReckoningActive ? "true" : "false")
                .Append("|GreatReckoningTargetFactionId=").Append(summary.GreatReckoningTargetFactionId)
                .Append("|GreatReckoningShare=").Append(summary.GreatReckoningShare.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|CycleCount=").Append(summary.CycleCount)
                .Append("|PopulationBand=").Append(summary.PopulationBand)
                .Append("|FoodBand=").Append(summary.FoodBand)
                .Append("|WaterBand=").Append(summary.WaterBand)
                .Append("|LoyaltyBand=").Append(summary.LoyaltyBand)
                .Append("|ConvictionLabel=").Append(summary.ConvictionLabel)
                .Append("|FaithLabel=").Append(summary.FaithLabel)
                .Append("|FaithTier=").Append(summary.FaithTier)
                .Append("|FaithBand=").Append(summary.FaithBand)
                .Append("|PlayerVictoryRank=").Append(summary.PlayerVictoryRank)
                .Append("|PlayerVictoryConditionId=").Append(summary.PlayerVictoryConditionId)
                .Append("|PlayerVictoryProgressPct=").Append(summary.PlayerVictoryProgressPct.ToString("0.000", CultureInfo.InvariantCulture))
                .Append("|TopFactionId=").Append(summary.TopFactionId)
                .Append("|TopVictoryConditionId=").Append(summary.TopVictoryConditionId)
                .Append("|TopVictoryProgressPct=").Append(summary.TopVictoryProgressPct.ToString("0.000", CultureInfo.InvariantCulture));

            readout = builder.ToString();
            return true;
        }
    }
}
