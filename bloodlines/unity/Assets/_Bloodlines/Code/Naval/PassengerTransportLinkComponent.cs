using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Reverse pointer from an embarked passenger back to its transport. Used by
    /// disembark and fire-ship detonation systems to reach the carrier without
    /// scanning every transport's passenger buffer.
    ///
    /// Browser runtime equivalent: the `embarkedInTransportId` field on the
    /// passenger unit. Cleared on disembark.
    /// </summary>
    public struct PassengerTransportLinkComponent : IComponentData
    {
        public Entity Transport;
    }
}
