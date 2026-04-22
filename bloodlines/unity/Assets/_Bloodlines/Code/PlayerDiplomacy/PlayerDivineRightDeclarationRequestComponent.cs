using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for a player-side divine-right declaration.
    /// </summary>
    public struct PlayerDivineRightDeclarationRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
    }
}
