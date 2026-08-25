using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Nix.Traits.Claustrophobia;

/// <summary>
/// Attached to entities suffering from Claustrophobia.
/// When trapped inside enclosed containers (lockers, crates, bodybags),
/// they suffer progressive escalating panic attacks, asphyxiation symptoms, and agitation.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ClaustrophobiaComponent : Component
{
    /// <summary>
    /// Next timestamp for applying claustrophobic panic effects.
    /// </summary>
    [DataField("nextPanicTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField]
    public TimeSpan NextPanicTime = TimeSpan.Zero;

    /// <summary>
    /// Interval between panic checks in seconds.
    /// </summary>
    [DataField("panicInterval")]
    [AutoNetworkedField]
    public float PanicInterval = 3.0f;

    /// <summary>
    /// Accumulated seconds spent trapped inside an enclosed container.
    /// </summary>
    public float SecondsTrapped = 0f;

    /// <summary>
    /// Current panic escalation stage (0 = none, 1 = uneasy, 2 = hyperventilating, 3 = terror).
    /// </summary>
    public int CurrentStage = 0;

    /// <summary>
    /// Last timestamp a chat notification was sent.
    /// </summary>
    public TimeSpan LastChatTime = TimeSpan.Zero;
}
