using System.Globalization;
using System.Text;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Pure presentation helper for the on-screen player command-deck overlay.
    /// Keeps layout copy deterministic for both the IMGUI shell and editor smoke validation.
    /// </summary>
    public static class PlayerCommandDeckOverlayPresenter
    {
        public static string BuildTitle(in PlayerCommandDeckHUDComponent snapshot)
        {
            return string.IsNullOrWhiteSpace(snapshot.StageLabel.ToString())
                ? "Player Command Deck"
                : "Player Command Deck | " + snapshot.StageLabel;
        }

        public static string BuildBody(in PlayerCommandDeckHUDComponent snapshot)
        {
            var builder = new StringBuilder(512);
            builder.Append("Alert: ").Append(FormatLabel(snapshot.PrimaryAlertLabel)).AppendLine();
            builder.Append("Phase: ").Append(FormatLabel(snapshot.PhaseLabel))
                .Append("  |  World Pressure: ")
                .Append(FormatLabel(snapshot.WorldPressureLabel))
                .Append(" (L").Append(snapshot.WorldPressureLevel).Append(')')
                .AppendLine();

            builder.Append("Victory: ").Append(FormatLabel(snapshot.LeadingVictoryConditionId))
                .Append("  |  ")
                .Append((snapshot.LeadingVictoryProgressPct * 100f).ToString("0.0", CultureInfo.InvariantCulture))
                .Append("%  |  Rank ")
                .Append(snapshot.VictoryRank <= 0 ? "-" : snapshot.VictoryRank.ToString(CultureInfo.InvariantCulture));

            if (!float.IsNaN(snapshot.LeadingVictoryEtaInWorldDays))
            {
                builder.Append("  |  ETA ")
                    .Append(snapshot.LeadingVictoryEtaInWorldDays.ToString("0.0", CultureInfo.InvariantCulture))
                    .Append('d');
            }

            builder.AppendLine();
            builder.Append("Renown: ").Append(snapshot.RenownScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Append("  |  Rank ")
                .Append(snapshot.RenownRank <= 0 ? "-" : snapshot.RenownRank.ToString(CultureInfo.InvariantCulture))
                .Append("  |  ")
                .Append(FormatLabel(snapshot.RenownBandLabel))
                .AppendLine();

            builder.Append("Realm: Pop ").Append(FormatLabel(snapshot.PopulationBand))
                .Append("  |  Loyalty ").Append(FormatLabel(snapshot.LoyaltyBand))
                .Append("  |  Faith ").Append(FormatLabel(snapshot.FaithBand))
                .AppendLine();

            builder.Append("Fortification Threat: ")
                .Append(snapshot.FortificationThreatActive ? "Active" : "Stable");

            if (snapshot.GreatReckoningActive)
            {
                builder.AppendLine();
                builder.Append("Great Reckoning: Active");
            }

            return builder.ToString();
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
