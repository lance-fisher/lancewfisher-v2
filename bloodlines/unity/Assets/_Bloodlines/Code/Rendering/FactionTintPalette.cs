using System;
using Unity.Mathematics;
using UnityEngine;

namespace Bloodlines.Rendering
{
    /// <summary>
    /// Canonical faction tint lookup. Seed defaults reference
    /// 06_FACTIONS/FOUNDING_HOUSES.md for house colors; entries
    /// without a documented canonical color fall back to a
    /// neutral grey-brown. This table is infrastructure-only.
    /// Final faction-identity palettes land with human art
    /// direction, not here.
    /// </summary>
    public static class FactionTintPalette
    {
        private static readonly Color PlayerIronmark  = new Color(0.23f, 0.56f, 0.96f, 1f);
        private static readonly Color EnemyStonehelm  = new Color(0.87f, 0.30f, 0.29f, 1f);
        private static readonly Color Tribes          = new Color(0.78f, 0.58f, 0.22f, 1f);
        private static readonly Color Neutral         = new Color(0.62f, 0.62f, 0.66f, 1f);
        private static readonly Color Goldgrave       = new Color(0.90f, 0.78f, 0.30f, 1f);
        private static readonly Color Hartvale        = new Color(0.35f, 0.70f, 0.42f, 1f);
        private static readonly Color Westland        = new Color(0.73f, 0.46f, 0.24f, 1f);
        private static readonly Color Whitehall       = new Color(0.90f, 0.90f, 0.92f, 1f);
        private static readonly Color Oldcrest        = new Color(0.40f, 0.24f, 0.38f, 1f);
        private static readonly Color Highborne       = new Color(0.68f, 0.55f, 0.82f, 1f);
        private static readonly Color Trueborn        = new Color(0.58f, 0.48f, 0.30f, 1f);
        private static readonly Color Fallback        = new Color(0.72f, 0.72f, 0.72f, 1f);

        public static Color ResolveColor(string factionIdOrHouseId)
        {
            if (string.IsNullOrWhiteSpace(factionIdOrHouseId))
            {
                return Fallback;
            }

            switch (factionIdOrHouseId.Trim().ToLowerInvariant())
            {
                case "player":
                case "ironmark":
                    return PlayerIronmark;
                case "enemy":
                case "stonehelm":
                    return EnemyStonehelm;
                case "tribes":
                    return Tribes;
                case "neutral":
                case "trueborn_city":
                    return Neutral;
                case "goldgrave":
                    return Goldgrave;
                case "hartvale":
                    return Hartvale;
                case "westland":
                    return Westland;
                case "whitehall":
                    return Whitehall;
                case "oldcrest":
                    return Oldcrest;
                case "highborne":
                    return Highborne;
                case "trueborn":
                    return Trueborn;
                default:
                    return ResolveHashedFallback(factionIdOrHouseId);
            }
        }

        public static float4 ResolveTint(string factionIdOrHouseId)
        {
            var color = ResolveColor(factionIdOrHouseId);
            return new float4(color.r, color.g, color.b, color.a);
        }

        private static Color ResolveHashedFallback(string key)
        {
            int hash = Math.Abs(key.GetHashCode());
            float hue = (hash % 1000) / 1000f;
            return Color.HSVToRGB(hue, 0.55f, 0.9f);
        }
    }
}
