using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    /// <summary>
    /// One-shot request entity for a player-side holy war declaration.
    /// </summary>
    public struct PlayerHolyWarDeclarationRequestComponent : IComponentData
    {
        public FixedString32Bytes SourceFactionId;
        public FixedString32Bytes TargetFactionId;
    }
}
