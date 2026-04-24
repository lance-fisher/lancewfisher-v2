using System.Globalization;
using System.Text;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Pure presentation helper for the player command-deck overlay. Keeps
    /// visible copy deterministic for both the IMGUI shell and smoke validation.
    /// </summary>
    public static class PlayerCommandDeckOverlayPresenter
    {
        public static string BuildTitle(in PlayerCommandDeckHUDComponent snapshot)
        {
            string stageLabel = FormatLabel(snapshot.StageLabel);
            return stageLabel == "-"
                ? "Player Command Deck"
                : "Player Command Deck | " + stageLabel;
        }

        public static string BuildBody(in PlayerCommandDeckHUDComponent snapshot)
        {
            var builder = new StringBuilder(512);
            builder.AppendLine(BuildAlertLine(in snapshot));
            builder.Append("Stage: ").Append(BuildStageLine(in snapshot)).AppendLine();
            builder.Append("Victory: ").Append(BuildVictoryLine(in snapshot)).AppendLine();
            builder.Append("Dynasty: ").Append(BuildDynastyLine(in snapshot)).AppendLine();
            builder.Append("Pressure: ").Append(BuildPressureLine(in snapshot)).AppendLine();
            builder.Append("Fortification: ")
                .Append(snapshot.FortificationThreatActive ? "Active" : "Stable");

            if (snapshot.GreatReckoningActive)
            {
                builder.AppendLine();
                builder.Append("Great Reckoning: Active");
            }

            return builder.ToString();
        }

        public static string BuildStageLine(in PlayerCommandDeckHUDComponent snapshot)
        {
            return FormatLabel(snapshot.StageLabel) + " / " + FormatLabel(snapshot.PhaseLabel);
        }

        public static string BuildAlertLine(in PlayerCommandDeckHUDComponent snapshot)
        {
            return snapshot.PrimaryAlertLabel.ToString() switch
            {
                "great_reckoning" => "Alert: great reckoning active",
                "fortification_threat" => "Alert: fortifications threatened",
                "loyalty_crisis" => "Alert: loyalty crisis",
                "victory_imminent" => "Alert: victory imminent",
                "world_pressure" => "Alert: world pressure rising",
                _ => "Alert: stable",
            };
        }

        public static string BuildVictoryLine(in PlayerCommandDeckHUDComponent snapshot)
        {
            var builder = new StringBuilder(128);
            builder.Append(FormatLabel(snapshot.LeadingVictoryConditionId))
                .Append(' ')
                .Append((snapshot.LeadingVictoryProgressPct * 100f).ToString("0.0", CultureInfo.InvariantCulture))
                .Append("% rank ")
                .Append(snapshot.VictoryRank <= 0
                    ? "-"
                    : snapshot.VictoryRank.ToString(CultureInfo.InvariantCulture));

            if (!float.IsNaN(snapshot.LeadingVictoryEtaInWorldDays))
            {
                builder.Append(" ETA ")
                    .Append(snapshot.LeadingVictoryEtaInWorldDays.ToString("0.0", CultureInfo.InvariantCulture))
                    .Append('d');
            }

            return builder.ToString();
        }

        public static string BuildDynastyLine(in PlayerCommandDeckHUDComponent snapshot)
        {
            return "Renown " +
                   snapshot.RenownScore.ToString("0.0", CultureInfo.InvariantCulture) +
                   " rank " +
                   (snapshot.RenownRank <= 0
                       ? "-"
                       : snapshot.RenownRank.ToString(CultureInfo.InvariantCulture)) +
                   ' ' +
                   FormatLabel(snapshot.RenownBandLabel);
        }

        public static string BuildPressureLine(in PlayerCommandDeckHUDComponent snapshot)
        {
            return "Pressure " +
                   FormatLabel(snapshot.WorldPressureLabel) +
                   ' ' +
                   snapshot.WorldPressureLevel.ToString(CultureInfo.InvariantCulture) +
                   "; pop " +
                   FormatLabel(snapshot.PopulationBand) +
                   "; loyalty " +
                   FormatLabel(snapshot.LoyaltyBand) +
                   "; faith " +
                   FormatLabel(snapshot.FaithBand);
        }

        public static string ResolveAlertColorKey(in PlayerCommandDeckHUDComponent snapshot)
        {
            return snapshot.PrimaryAlertLabel.ToString();
        }

        private static string FormatLabel<T>(T value) where T : struct
        {
            string text = value.ToString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return "-";
            }

            return text.Replace('_', ' ');
        }
    }
}
