using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.SocialAnxiety;

/// <summary>
/// Attached to entities suffering from Social Anxiety.
/// Causes nervous stuttering/fillers when speaking around crowds,
/// and stress reactions (jittering, stuttering, or freezing) when making direct eye contact / being examined.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SocialAnxietyComponent : Component
{
    /// <summary>
    /// Chance for eye contact / examination to trigger an anxiety panic reaction.
    /// </summary>
    [DataField("eyeContactPanicChance")]
    [AutoNetworkedField]
    public float EyeContactPanicChance = 0.30f;

    /// <summary>
    /// Cooldown between two eye contact panic reactions.
    /// </summary>
    [DataField("lastPanicTime")]
    public TimeSpan LastPanicTime = TimeSpan.Zero;

    /// <summary>
    /// Minimum time between eye contact panics.
    /// </summary>
    [DataField("panicCooldown")]
    public TimeSpan PanicCooldown = TimeSpan.FromSeconds(15f);
}
