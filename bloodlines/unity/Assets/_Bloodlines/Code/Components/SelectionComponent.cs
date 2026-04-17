using Unity.Entities;

namespace Bloodlines.Components
{
    /// <summary>
    /// Debug/runtime selection marker for the first interactive Unity shell.
    /// This is currently driven by the debug command surface and used only for early
    /// battlefield interaction and selection highlighting.
    /// </summary>
    public struct SelectedTag : IComponentData
    {
    }
}
