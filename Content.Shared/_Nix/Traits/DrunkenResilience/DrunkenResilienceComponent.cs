using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.DrunkenResilience;

/// <summary>
/// Attached to entities with Drunken Resilience trait.
/// Slowly regenerates physical and heat damage while intoxicated.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DrunkenResilienceComponent : Component
{
    /// <summary>
    /// Interval in seconds between healing ticks.
    /// </summary>
    [DataField("healInterval")]
    [AutoNetworkedField]
    public float HealInterval = 2.0f;

    /// <summary>
    /// Amount of brute damage healed per interval while drunk.
    /// </summary>
    [DataField("bruteHeal")]
    [AutoNetworkedField]
    public float BruteHeal = 1.0f;

    /// <summary>
    /// Amount of burn/heat damage healed per interval while drunk.
    /// </summary>
    [DataField("burnHeal")]
    [AutoNetworkedField]
    public float BurnHeal = 0.5f;

    /// <summary>
    /// Internal timer for next heal tick.
    /// </summary>
    public TimeSpan NextHealTime = TimeSpan.Zero;
}
