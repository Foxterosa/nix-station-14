using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Nix.Traits.Hallucinations;

/// <summary>
/// Attached to entities afflicted with Reality Dissociation Syndrome.
/// Periodically generates client-exclusive psychological hallucinations including phantom monsters,
/// fake nearby chatter, fake radio broadcasts, illusory combat noises, and phantom items.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RealityDissociationComponent : Component
{
    /// <summary>
    /// Minimum time in seconds between two hallucination incidents.
    /// </summary>
    [DataField("minTimeBetweenIncidents")]
    [AutoNetworkedField]
    public float MinTimeBetweenIncidents = 20f;

    /// <summary>
    /// Maximum time in seconds between two hallucination incidents.
    /// </summary>
    [DataField("maxTimeBetweenIncidents")]
    [AutoNetworkedField]
    public float MaxTimeBetweenIncidents = 50f;

    /// <summary>
    /// Timestamp for when the next hallucination incident will occur.
    /// </summary>
    [DataField("nextIncidentTime", customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField]
    public TimeSpan NextIncidentTime = TimeSpan.Zero;

    /// <summary>
    /// Weight chance for experiencing fake chatter from a nearby crew member or pet.
    /// </summary>
    [DataField("weightFakeChatter")]
    public float WeightFakeChatter = 35f;

    /// <summary>
    /// Weight chance for experiencing fake emergency radio reports over the headset.
    /// </summary>
    [DataField("weightFakeRadio")]
    public float WeightFakeRadio = 30f;

    /// <summary>
    /// Weight chance for a client-exclusive phantom monster to appear and charge.
    /// </summary>
    [DataField("weightPhantomMob")]
    public float WeightPhantomMob = 20f;

    /// <summary>
    /// Weight chance for hearing illusory battle/gunfire sounds behind walls.
    /// </summary>
    [DataField("weightCombatAudio")]
    public float WeightCombatAudio = 10f;

    /// <summary>
    /// Weight chance for seeing a phantom item on the floor.
    /// </summary>
    [DataField("weightPhantomItem")]
    public float WeightPhantomItem = 5f;
}
