using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Cursed;

/// <summary>
/// Attached to entities cursed with bizarre bad luck.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CursedComponent : Component
{
    public TimeSpan NextCurseCheck = TimeSpan.Zero;
}
