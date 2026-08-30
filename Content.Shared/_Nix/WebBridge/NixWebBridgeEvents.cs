using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.WebBridge;

[Serializable, NetSerializable]
public sealed class NixWebAppearanceCaptureRequestEvent : EntityEventArgs
{
    public NetEntity Entity;
    public int ProfileSlot;
    public string CharacterName = string.Empty;
    public string Species = string.Empty;
}

[Serializable, NetSerializable]
public sealed class NixWebAppearanceCaptureResponseEvent : EntityEventArgs
{
    public NetEntity Entity;
    public int ProfileSlot;
    public string CharacterName = string.Empty;
    public string Species = string.Empty;
    public NixWebCharacterAppearance? Appearance;
}
