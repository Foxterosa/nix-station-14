using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Depression;

/// <summary>
/// Attached to entities suffering from Depression.
/// Periodically undergoes bouts of melancholy, heavy sighing, and reduced stamina.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class DepressionComponent : Component
{
    public TimeSpan NextEpisodeTime = TimeSpan.Zero;
}
