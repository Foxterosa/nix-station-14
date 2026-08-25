using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Smoker;

/// <summary>
/// Attached to entities with the Smoker quirk.
/// Needs regular smoking to stay calm; suffers withdrawal coughing/tremors if deprived of nicotine.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SmokerComponent : Component
{
    /// <summary>
    /// Timestamp when character last smoked or inhaled nicotine.
    /// </summary>
    public TimeSpan LastSmokedTime = TimeSpan.Zero;

    /// <summary>
    /// Next timestamp for craving check.
    /// </summary>
    public TimeSpan NextCravingCheck = TimeSpan.Zero;

    /// <summary>
    /// Withdrawal threshold in seconds (240s = 4 mins).
    /// </summary>
    [DataField("cravingDelay")]
    [AutoNetworkedField]
    public float CravingDelay = 240f;
}
