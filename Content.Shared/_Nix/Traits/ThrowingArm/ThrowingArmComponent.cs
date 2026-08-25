using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.ThrowingArm;

/// <summary>
/// Attached to entities with exceptionally strong throwing arms.
/// Greatly increases thrown item velocity and distance.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ThrowingArmComponent : Component
{
    [DataField("speedMultiplier")]
    [AutoNetworkedField]
    public float SpeedMultiplier = 1.45f;
}
