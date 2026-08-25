using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Freerunning;

/// <summary>
/// Attached to entities skilled in freerunning/parkour.
/// Doubles vaulting/climbing speed over obstacles.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FreerunningComponent : Component
{
}
