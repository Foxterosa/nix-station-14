using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.FamilyHeirloom;

/// <summary>
/// Attached to entities attached to an irreplaceable family heirloom.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FamilyHeirloomComponent : Component
{
    public TimeSpan NextCheckTime = TimeSpan.Zero;
    public EntityUid? HeirloomEntity;
    public bool IsMissing = false;
}
