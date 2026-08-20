using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Nix.StationEvents;

[Prototype("nixStationEventAudio")]
public sealed partial class NixStationEventAudioPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField]
    public SoundSpecifier? StartAudio;

    [DataField]
    public SoundSpecifier? EndAudio;
}
