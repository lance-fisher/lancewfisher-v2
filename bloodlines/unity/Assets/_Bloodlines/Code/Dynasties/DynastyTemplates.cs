using Bloodlines.Components;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Canonical eight-member dynasty template set. Direct 1:1 port of the
    /// browser runtime templates in simulation.js createDynastyState (line 4267).
    /// Every new faction (non-tribe) spawns with exactly this set: one ruling
    /// head, one heir, one war captain, one governor, one envoy, one covenant
    /// speaker, one trade factor, and one house shadow.
    /// </summary>
    public static class DynastyTemplates
    {
        public readonly struct Template
        {
            public readonly string Suffix;
            public readonly string Title;
            public readonly DynastyRole Role;
            public readonly DynastyPath Path;
            public readonly float AgeYears;
            public readonly DynastyMemberStatus Status;
            public readonly float Renown;

            public Template(
                string suffix,
                string title,
                DynastyRole role,
                DynastyPath path,
                float ageYears,
                DynastyMemberStatus status,
                float renown)
            {
                Suffix = suffix;
                Title = title;
                Role = role;
                Path = path;
                AgeYears = ageYears;
                Status = status;
                Renown = renown;
            }
        }

        public static readonly Template[] Canonical = new[]
        {
            new Template("head", "Head of Bloodline", DynastyRole.HeadOfBloodline, DynastyPath.Governance, 38f, DynastyMemberStatus.Ruling, 22f),
            new Template("heir", "Eldest Heir", DynastyRole.HeirDesignate, DynastyPath.MilitaryCommand, 19f, DynastyMemberStatus.Active, 12f),
            new Template("marshal", "War Captain", DynastyRole.Commander, DynastyPath.MilitaryCommand, 28f, DynastyMemberStatus.Active, 14f),
            new Template("governor", "March Governor", DynastyRole.Governor, DynastyPath.Governance, 31f, DynastyMemberStatus.Active, 10f),
            new Template("envoy", "House Envoy", DynastyRole.Diplomat, DynastyPath.Diplomacy, 27f, DynastyMemberStatus.Active, 9f),
            new Template("priest", "Covenant Speaker", DynastyRole.IdeologicalLeader, DynastyPath.ReligiousLeadership, 24f, DynastyMemberStatus.Active, 8f),
            new Template("factor", "Trade Factor", DynastyRole.Merchant, DynastyPath.EconomicStewardshipTrade, 30f, DynastyMemberStatus.Active, 8f),
            new Template("shadow", "House Shadow", DynastyRole.Spymaster, DynastyPath.CovertOperations, 26f, DynastyMemberStatus.Active, 7f),
        };

        public const int InitialActiveMemberCap = 20;
        public const float InitialLegitimacy = 58f;
    }
}
