using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.LightStep;

/// <summary>
/// Attached to entities with the Light Step trait.
/// Causes the entity to walk completely silently without generating footstep sounds.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class LightStepComponent : Component
{
}
