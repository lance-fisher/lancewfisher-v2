using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Lane-local live sabotage state materialized on the target building so
    /// fire-raising and temporary gate exposure can tick without widening the
    /// shared building component surface.
    /// </summary>
    public struct PlayerSabotageEffectComponent : IComponentData
    {
        public double GateExposedUntil;
        public double BurningUntil;
        public float BurnDamagePerSecond;
    }
}
