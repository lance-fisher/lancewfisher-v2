namespace Bloodlines.Naval
{
    /// <summary>
    /// Canonical naval vessel classification, mirroring the browser unit definition
    /// `vesselClass` field. The six ship classes are the canonical naval roster per
    /// 11_MATCHFLOW/NAVAL_SYSTEM.md and design bible section 36.
    ///
    /// Browser runtime equivalent: data/units.json `vesselClass` strings.
    /// </summary>
    public enum VesselClass : byte
    {
        None = 0,
        Fishing = 1,
        Scout = 2,
        WarGalley = 3,
        Transport = 4,
        FireShip = 5,
        CapitalShip = 6,
    }
}
