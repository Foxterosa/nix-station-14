using Content.Shared._Nix.Traits.Musician;
using Robust.Client.UserInterface;

namespace Content.Client._Nix.Traits.Musician;

public sealed class MusicianChoiceBoxBoundUserInterface : BoundUserInterface
{
    private MusicianChoiceBoxWindow? _window;

    public MusicianChoiceBoxBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<MusicianChoiceBoxWindow>();
        _window.OnInstrumentSelected += instrumentId =>
        {
            SendMessage(new MusicianChoiceBoxSelectMessage(instrumentId));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is MusicianChoiceBoxBuiState buiState && _window != null)
        {
            _window.Populate(buiState.Instruments);
        }
    }
}
