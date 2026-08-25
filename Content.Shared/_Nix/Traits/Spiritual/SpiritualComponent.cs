using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Spiritual;

/// <summary>
/// Attached to spiritual or religious entities.
/// Gains inner calm and stamina recovery near holy artifacts and sacred places.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class SpiritualComponent : Component
{
    public TimeSpan NextCheckTime = TimeSpan.Zero;
}
