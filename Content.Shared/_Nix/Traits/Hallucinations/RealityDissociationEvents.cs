using System.Numerics;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.Traits.Hallucinations;

/// <summary>
/// Category of client-side hallucination incident.
/// </summary>
[Serializable, NetSerializable]
public enum RealityDissociationIncidentType : byte
{
    /// <summary>
    /// Spawns a client-only phantom entity that lunges or stalks the player.
    /// </summary>
    PhantomMob,

    /// <summary>
    /// Plays an illusory sequence of spatial battle/combat audio near the player.
    /// </summary>
    CombatAudio,

    /// <summary>
    /// Spawns a client-only illusory item on the floor that vanishes when approached.
    /// </summary>
    PhantomItem,
}

/// <summary>
/// Network event sent from server to client to trigger a local client-side hallucination.
/// </summary>
[Serializable, NetSerializable]
public sealed class RealityDissociationIncidentEvent : EntityEventArgs
{
    /// <summary>
    /// The player's net entity experiencing the hallucination.
    /// </summary>
    public NetEntity TargetNetEntity { get; }

    /// <summary>
    /// The type of incident to instantiate on the client.
    /// </summary>
    public RealityDissociationIncidentType Type { get; }

    /// <summary>
    /// Specific variant/prototype key for the incident (e.g. mob type, combat sound collection, or item).
    /// </summary>
    public string Variant { get; }

    /// <summary>
    /// Positional offset relative to the player's current coordinates.
    /// </summary>
    public Vector2 RelativeOffset { get; }

    public RealityDissociationIncidentEvent(
        NetEntity targetNetEntity,
        RealityDissociationIncidentType type,
        string variant,
        Vector2 relativeOffset)
    {
        TargetNetEntity = targetNetEntity;
        Type = type;
        Variant = variant;
        RelativeOffset = relativeOffset;
    }
}
