using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.BadTouch;

/// <summary>
/// Attached to entities with the Bad Touch trait.
/// Causes strong discomfort when touched, hugged, or patted by other crew members.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BadTouchComponent : Component
{
    /// <summary>
    /// Cooldown timestamp to prevent popup spam.
    /// </summary>
    public TimeSpan LastAnnoyedTime = TimeSpan.Zero;

    /// <summary>
    /// Cooldown between annoyance reactions.
    /// </summary>
    [DataField("annoyedCooldown")]
    [AutoNetworkedField]
    public TimeSpan AnnoyedCooldown = TimeSpan.FromSeconds(4.0);
}
