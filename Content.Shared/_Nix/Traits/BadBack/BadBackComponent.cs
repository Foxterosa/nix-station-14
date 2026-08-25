using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.BadBack;

/// <summary>
/// Attached to entities suffering from chronic bad back posture.
/// Wearing backpacks or heavy storage items on the back slot incurs a movement speed penalty and periodic back pain.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BadBackComponent : Component
{
    /// <summary>
    /// Movement speed multiplier when wearing a backpack or storage item on the back slot.
    /// </summary>
    [DataField("speedModifier")]
    [AutoNetworkedField]
    public float SpeedModifier = 0.82f;

    /// <summary>
    /// Timestamp of the last back pain popup/groan.
    /// </summary>
    [DataField("lastPainTime")]
    public TimeSpan LastPainTime = TimeSpan.Zero;

    /// <summary>
    /// Cooldown between back pain notifications.
    /// </summary>
    [DataField("painCooldown")]
    public TimeSpan PainCooldown = TimeSpan.FromSeconds(25f);
}
