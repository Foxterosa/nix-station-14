using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Frail;

/// <summary>
/// Attached to entities that are physically frail.
/// Causes them to suffer +25% increased brute/physical damage from all attacks and impacts.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class FrailComponent : Component
{
    /// <summary>
    /// Damage multiplier for incoming physical/brute damage.
    /// </summary>
    [DataField("damageMultiplier")]
    [AutoNetworkedField]
    public float DamageMultiplier = 1.25f;
}
