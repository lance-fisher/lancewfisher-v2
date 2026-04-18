using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Trigger component placed on a faction entity to queue a renown award.
    /// Consumed by RenownAwardSystem, which routes the award to the best available
    /// dynasty member (Commander > HeadOfBloodline > MilitaryCommand path > any).
    /// Browser reference: simulation.js findRenownAwardRecipient (~3062),
    /// awardRenownToFaction (~3080).
    /// RENOWN_CAP = 100 (simulation.js:3058).
    /// </summary>
    public struct RenownAwardRequestComponent : IComponentData
    {
        public FixedString32Bytes FactionId;
        public float Amount;
        public bool Consumed;
    }
}
