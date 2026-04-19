namespace Bloodlines.Fortification
{
    /// <summary>
    /// Browser reference: simulation.js IMMINENT_ENGAGEMENT_POSTURES (271-308).
    /// </summary>
    public static class ImminentEngagementCanon
    {
        public readonly struct ImminentEngagementPostureProfile
        {
            public ImminentEngagementPostureProfile(
                string id,
                string label,
                float reserveHealMultiplier,
                float reserveMusterMultiplier,
                float desiredFrontlineBonus,
                float retreatThresholdBonus,
                float defenderAttackMultiplier,
                float defenderSightBonus,
                float enemyApproachMultiplier,
                bool autoSortieOnExpiry)
            {
                Id = id;
                Label = label;
                ReserveHealMultiplier = reserveHealMultiplier;
                ReserveMusterMultiplier = reserveMusterMultiplier;
                DesiredFrontlineBonus = desiredFrontlineBonus;
                RetreatThresholdBonus = retreatThresholdBonus;
                DefenderAttackMultiplier = defenderAttackMultiplier;
                DefenderSightBonus = defenderSightBonus;
                EnemyApproachMultiplier = enemyApproachMultiplier;
                AutoSortieOnExpiry = autoSortieOnExpiry;
            }

            public string Id { get; }
            public string Label { get; }
            public float ReserveHealMultiplier { get; }
            public float ReserveMusterMultiplier { get; }
            public float DesiredFrontlineBonus { get; }
            public float RetreatThresholdBonus { get; }
            public float DefenderAttackMultiplier { get; }
            public float DefenderSightBonus { get; }
            public float EnemyApproachMultiplier { get; }
            public bool AutoSortieOnExpiry { get; }
        }

        public static readonly ImminentEngagementPostureProfile Brace =
            new(
                id: "brace",
                label: "Brace Defenses",
                reserveHealMultiplier: 1.2f,
                reserveMusterMultiplier: 1.16f,
                desiredFrontlineBonus: 0f,
                retreatThresholdBonus: 0.05f,
                defenderAttackMultiplier: 0.98f,
                defenderSightBonus: 8f,
                enemyApproachMultiplier: 0.92f,
                autoSortieOnExpiry: false);

        public static readonly ImminentEngagementPostureProfile Steady =
            new(
                id: "steady",
                label: "Steady Defense",
                reserveHealMultiplier: 1f,
                reserveMusterMultiplier: 1f,
                desiredFrontlineBonus: 0f,
                retreatThresholdBonus: 0f,
                defenderAttackMultiplier: 1f,
                defenderSightBonus: 0f,
                enemyApproachMultiplier: 1f,
                autoSortieOnExpiry: false);

        public static readonly ImminentEngagementPostureProfile Counterstroke =
            new(
                id: "counterstroke",
                label: "Counterstroke",
                reserveHealMultiplier: 0.96f,
                reserveMusterMultiplier: 1.1f,
                desiredFrontlineBonus: 1f,
                retreatThresholdBonus: -0.02f,
                defenderAttackMultiplier: 1.1f,
                defenderSightBonus: 12f,
                enemyApproachMultiplier: 1f,
                autoSortieOnExpiry: true);

        public static ImminentEngagementPostureProfile GetPosture(string id)
        {
            return id switch
            {
                "brace" => Brace,
                "counterstroke" => Counterstroke,
                _ => Steady,
            };
        }
    }
}
