using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Nix.Traits.BloodDeficiency;

/// <summary>
/// Attached to entities suffering from chronic Blood Deficiency (Severe Anemia).
/// Their body cannot produce enough blood to sustain full volume, causing chronic low blood levels.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BloodDeficiencyComponent : Component
{
    /// <summary>
    /// Next timestamp for draining blood volume.
    /// </summary>
    [DataField("nextDrainTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField]
    public TimeSpan NextDrainTime = TimeSpan.Zero;

    /// <summary>
    /// Interval in seconds between blood drains.
    /// </summary>
    [DataField("drainInterval")]
    [AutoNetworkedField]
    public float DrainInterval = 12f;

    /// <summary>
    /// Amount of blood volume drained each tick.
    /// </summary>
    [DataField("drainAmount")]
    [AutoNetworkedField]
    public float DrainAmount = 2.0f;

    /// <summary>
    /// Minimum blood volume ratio below which automatic drainage stops.
    /// </summary>
    [DataField("minBloodRatio")]
    [AutoNetworkedField]
    public float MinBloodRatio = 0.65f;
}
