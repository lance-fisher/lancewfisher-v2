using Unity.Entities;

namespace Bloodlines.Economy
{
    /// <summary>
    /// Per-faction trade route summary. Written by TradeRouteEvaluationSystem once per
    /// in-world day. ActiveRouteCount reflects the number of adjacent uncontested
    /// control-point pairs owned by the faction; TotalGoldPerTickFromTrades is the
    /// resulting passive gold yield applied to ResourceStockpileComponent each day.
    ///
    /// "Adjacent" means two control points whose combined radii plus a 2-tile padding
    /// span the distance between them. Contested control points do not contribute.
    ///
    /// Browser equivalent: absent -- implemented from canonical trade route design.
    /// </summary>
    public struct TradeRouteComponent : IComponentData
    {
        public int ActiveRouteCount;
        public float TotalGoldPerTickFromTrades;
        public float LastUpdatedAtInWorldDays;
    }
}
