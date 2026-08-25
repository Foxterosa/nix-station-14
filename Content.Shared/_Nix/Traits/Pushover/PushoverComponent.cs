using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Pushover;

/// <summary>
/// Attached to entities that are pushovers.
/// Makes them extraordinarily vulnerable to shoves and disarm attempts.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PushoverComponent : Component
{
    /// <summary>
    /// Guaranteed knockdown probability when shoved/disarmed.
    /// </summary>
    [DataField("shoveKnockdownProbability")]
    [AutoNetworkedField]
    public float ShoveKnockdownProbability = 0.90f;
}
