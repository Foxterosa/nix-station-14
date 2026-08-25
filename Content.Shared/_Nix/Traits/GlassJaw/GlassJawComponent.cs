using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.GlassJaw;

/// <summary>
/// Attached to entities with a very fragile jaw.
/// Heavy physical blows to the head have a high probability of knocking them unconscious or stunning them.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class GlassJawComponent : Component
{
    /// <summary>
    /// Minimum damage threshold required in a single hit to trigger a knockout roll.
    /// </summary>
    [DataField("minDamageThreshold")]
    [AutoNetworkedField]
    public float MinDamageThreshold = 4.5f;

    /// <summary>
    /// Chance multiplier per point of damage received (e.g. 10 damage = 60% chance).
    /// </summary>
    [DataField("knockoutChancePerDamage")]
    [AutoNetworkedField]
    public float KnockoutChancePerDamage = 0.06f;

    /// <summary>
    /// Duration of the knockout / stun in seconds.
    /// </summary>
    [DataField("knockoutDuration")]
    [AutoNetworkedField]
    public float KnockoutDuration = 3.0f;
}
