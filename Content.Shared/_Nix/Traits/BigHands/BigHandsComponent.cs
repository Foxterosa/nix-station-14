using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.BigHands;

/// <summary>
/// Attached to entities that have unusually large, clumsy hands.
/// Makes delicate item manipulation and tool usage prone to occasional fumbles.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BigHandsComponent : Component
{
    /// <summary>
    /// Probability of dropping the active held item when performing complex tool interactions.
    /// </summary>
    [DataField("fumbleChance")]
    [AutoNetworkedField]
    public float FumbleChance = 0.08f;

    /// <summary>
    /// Cooldown between fumbles to prevent continuous dropping.
    /// </summary>
    public TimeSpan LastFumbleTime = TimeSpan.Zero;

    [DataField("fumbleCooldown")]
    [AutoNetworkedField]
    public TimeSpan FumbleCooldown = TimeSpan.FromSeconds(5.0);
}
