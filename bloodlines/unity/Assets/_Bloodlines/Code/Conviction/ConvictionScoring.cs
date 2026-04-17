using Bloodlines.Components;
using Unity.Mathematics;

namespace Bloodlines.Conviction
{
    /// <summary>
    /// Pure, deterministic scoring helpers that translate bucket amounts into the
    /// canonical score and band. Separated from the ECS system so tests and debug
    /// surfaces can reuse them without running a World.
    ///
    /// Browser reference: simulation.js deriveConvictionScore (4229) and
    /// refreshConvictionBand (4233). Band thresholds match data/conviction-states.json.
    /// </summary>
    public static class ConvictionScoring
    {
        public const float ApexMoralMinScore = 75f;
        public const float MoralMinScore = 25f;
        public const float NeutralMinScore = -24f;
        public const float CruelMinScore = -74f;

        public static float DeriveScore(in ConvictionComponent conviction)
        {
            return (conviction.Stewardship + conviction.Oathkeeping)
                - (conviction.Ruthlessness + conviction.Desecration);
        }

        public static ConvictionBand ResolveBand(float score)
        {
            if (score >= ApexMoralMinScore) return ConvictionBand.ApexMoral;
            if (score >= MoralMinScore) return ConvictionBand.Moral;
            if (score >= NeutralMinScore) return ConvictionBand.Neutral;
            if (score >= CruelMinScore) return ConvictionBand.Cruel;
            return ConvictionBand.ApexCruel;
        }

        public static void Refresh(ref ConvictionComponent conviction)
        {
            conviction.Score = DeriveScore(conviction);
            conviction.Band = ResolveBand(conviction.Score);
        }

        public static void ApplyEvent(
            ref ConvictionComponent conviction,
            ConvictionBucket bucket,
            float amount)
        {
            switch (bucket)
            {
                case ConvictionBucket.Ruthlessness:
                    conviction.Ruthlessness = math.max(0f, conviction.Ruthlessness + amount);
                    break;
                case ConvictionBucket.Stewardship:
                    conviction.Stewardship = math.max(0f, conviction.Stewardship + amount);
                    break;
                case ConvictionBucket.Oathkeeping:
                    conviction.Oathkeeping = math.max(0f, conviction.Oathkeeping + amount);
                    break;
                case ConvictionBucket.Desecration:
                    conviction.Desecration = math.max(0f, conviction.Desecration + amount);
                    break;
            }

            Refresh(ref conviction);
        }
    }

    public enum ConvictionBucket : byte
    {
        Ruthlessness = 0,
        Stewardship = 1,
        Oathkeeping = 2,
        Desecration = 3,
    }
}
