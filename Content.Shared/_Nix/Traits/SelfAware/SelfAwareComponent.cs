using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.SelfAware;

/// <summary>
/// Attached to entities with the Self-Aware trait.
/// Allows the player to see their exact health and damage breakdown when self-examining.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SelfAwareComponent : Component
{
}
