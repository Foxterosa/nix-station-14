using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Skittish;

/// <summary>
/// Attached to entities with the Skittish trait.
/// Easily startled; instinctively hides inside lockers and containers.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SkittishComponent : Component
{
}
