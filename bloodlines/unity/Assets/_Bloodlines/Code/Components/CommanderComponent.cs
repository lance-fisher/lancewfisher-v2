using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Commander attachment tied to a bloodline member. Canonical Decision 8 element
    /// ("Commanders are battlefield-present, killable, capturable, aura-strong").
    /// Browser runtime equivalent: unit.commanderMemberId.
    ///
    /// The commander entity itself is a combat unit with this additional component
    /// attached, NOT a separate entity parented to the unit. Matches browser runtime
    /// where commanders ARE combat units with a dynasty member id.
    /// </summary>
    public struct CommanderComponent : IComponentData
    {
        public FixedString64Bytes MemberId;
        public FixedString32Bytes Role;
        public float Renown;
    }

    /// <summary>
    /// Marks a commander unit as currently present at the keep. Read by the fortification
    /// block of the realm-condition snapshot and by the defensive systems that augment
    /// ward potency, reserve muster rate, and sortie capability when the commander is home.
    /// Session 10 adds the sortie capability layer.
    /// </summary>
    public struct CommanderAtKeepTag : IComponentData
    {
    }
}
