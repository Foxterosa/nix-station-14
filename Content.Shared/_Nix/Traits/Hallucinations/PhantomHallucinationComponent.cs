using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared._Nix.Traits.Hallucinations;

/// <summary>
/// Attached to client-only phantom entities (monsters or items) spawned as a hallucination.
/// Controls their lifetime, movement/lunge toward the player, and clean despawn.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PhantomHallucinationComponent : Component
{
    /// <summary>
    /// How long in seconds this phantom entity exists before naturally vanishing.
    /// </summary>
    [DataField("lifetime")]
    public float Lifetime = 2.5f;

    /// <summary>
    /// Timestamp when this entity was spawned.
    /// </summary>
    [DataField("spawnTime")]
    public TimeSpan SpawnTime = TimeSpan.Zero;

    /// <summary>
    /// The target player entity this phantom is haunting or lunging towards.
    /// </summary>
    [DataField("target")]
    public EntityUid? Target;

    /// <summary>
    /// Movement speed towards the target player while active.
    /// </summary>
    [DataField("lungeSpeed")]
    public float LungeSpeed = 4.0f;

    /// <summary>
    /// If true, this phantom is a static ground item rather than an aggressive mob.
    /// </summary>
    [DataField("isItem")]
    public bool IsItem = false;

    /// <summary>
    /// Distance threshold for phantom items to vanish when the player approaches.
    /// </summary>
    [DataField("disappearRange")]
    public float DisappearRange = 1.3f;

    /// <summary>
    /// Optional sound played when the phantom vanishes.
    /// </summary>
    [DataField("despawnSound")]
    public SoundSpecifier? DespawnSound;
}
