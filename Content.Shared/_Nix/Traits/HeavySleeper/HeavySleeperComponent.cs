using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.HeavySleeper;

/// <summary>
/// Attached to entities that are heavy sleepers.
/// Makes them require significantly more damage/effort to wake up from sleep or unconsciousness.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HeavySleeperComponent : Component
{
    /// <summary>
    /// Extra damage threshold required to wake the entity up from sleep.
    /// </summary>
    [DataField("wakeThresholdBonus")]
    [AutoNetworkedField]
    public float WakeThresholdBonus = 10f;
}
