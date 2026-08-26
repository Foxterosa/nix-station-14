using Robust.Shared.Serialization;

namespace Content.Shared._Nix.AI;

/// <summary>
/// Evento para cambiar el modo de privacidad (Altavoz / Auricular).
/// </summary>
[Serializable, NetSerializable]
public sealed class AIBrainTogglePrivateModeEvent : EntityEventArgs
{
    public NetEntity Entity { get; }

    public AIBrainTogglePrivateModeEvent(NetEntity entity)
    {
        Entity = entity;
    }
}

/// <summary>
/// Evento para borrar la memoria de la IA (Wipe).
/// </summary>
[Serializable, NetSerializable]
public sealed class AIBrainWipeMemoryEvent : EntityEventArgs
{
    public NetEntity Entity { get; }

    public AIBrainWipeMemoryEvent(NetEntity entity)
    {
        Entity = entity;
    }
}
