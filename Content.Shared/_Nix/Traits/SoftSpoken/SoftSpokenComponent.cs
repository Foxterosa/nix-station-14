using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.SoftSpoken;

/// <summary>
/// Attached to entities that are naturally soft-spoken.
/// Reduces speech volume and forces their voice to carry much shorter distances.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SoftSpokenComponent : Component
{
}
