using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Nix.Traits.Musician;

[RegisterComponent, NetworkedComponent]
public sealed partial class MusicianChoiceBoxComponent : Component
{
    [DataField]
    public Dictionary<string, string> Instruments = new()
    {
        { "AcousticGuitarInstrument", "Guitarra Acústica" },
        { "SynthesizerInstrument", "Sintetizador Portátil" },
        { "HarmonicaInstrument", "Armónica" },
        { "RecorderInstrument", "Flauta Dulce" },
        { "TrumpetInstrument", "Trompeta" },
        { "ViolinInstrument", "Violín" },
        { "AccordionInstrument", "Acordeón" },
        { "SaxophoneInstrument", "Saxofón" },
        { "OcarinaInstrument", "Ocarina" },
    };
}

[Serializable, NetSerializable]
public enum MusicianChoiceBoxUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class MusicianChoiceBoxBuiState : BoundUserInterfaceState
{
    public Dictionary<string, string> Instruments { get; }

    public MusicianChoiceBoxBuiState(Dictionary<string, string> instruments)
    {
        Instruments = instruments;
    }
}

[Serializable, NetSerializable]
public sealed class MusicianChoiceBoxSelectMessage : BoundUserInterfaceMessage
{
    public string SelectedInstrumentId { get; }

    public MusicianChoiceBoxSelectMessage(string selectedInstrumentId)
    {
        SelectedInstrumentId = selectedInstrumentId;
    }
}
