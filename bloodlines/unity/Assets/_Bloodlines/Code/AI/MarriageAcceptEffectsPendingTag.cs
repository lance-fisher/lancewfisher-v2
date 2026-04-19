using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Tag attached to the primary MarriageComponent entity when an AI-accepted
    /// marriage is first created. AIMarriageAcceptEffectsSystem consumes the tag
    /// to apply the one-shot diplomatic and dynasty effects the browser applies
    /// in simulation.js acceptMarriage (~7388-7469), then removes the tag.
    ///
    /// Tagging only the primary record (IsPrimary = true) ensures effects apply
    /// exactly once per accepted marriage. The mirror record does not receive
    /// the tag.
    ///
    /// Browser reference: simulation.js acceptMarriage (~7388-7469) — the
    /// post-record-creation block starting at the legitimacy increment.
    /// </summary>
    public struct MarriageAcceptEffectsPendingTag : IComponentData
    {
    }
}
