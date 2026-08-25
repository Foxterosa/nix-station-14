using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.Musician;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;

namespace Content.Server._Nix.Traits.Musician;

/// <summary>
/// Server system managing the Musician Choice Box.
/// Opens interactive UI dialog on use so the player can select their preferred instrument and confirm.
/// </summary>
public sealed class MusicianChoiceBoxSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly TransformSystem _xform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<MusicianChoiceBoxComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<MusicianChoiceBoxComponent, ActivateInWorldEvent>(OnActivateInWorld);
        SubscribeLocalEvent<MusicianChoiceBoxComponent, MusicianChoiceBoxSelectMessage>(OnInstrumentSelected);
    }

    private void OnUseInHand(EntityUid uid, MusicianChoiceBoxComponent comp, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        OpenChoiceUi(uid, args.User, comp);
        args.Handled = true;
    }

    private void OnActivateInWorld(EntityUid uid, MusicianChoiceBoxComponent comp, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        OpenChoiceUi(uid, args.User, comp);
        args.Handled = true;
    }

    private void OpenChoiceUi(EntityUid uid, EntityUid user, MusicianChoiceBoxComponent comp)
    {
        if (!TryComp<ActorComponent>(user, out _))
            return;

        _ui.TryOpenUi(uid, MusicianChoiceBoxUiKey.Key, user);
        _ui.SetUiState(uid, MusicianChoiceBoxUiKey.Key, new MusicianChoiceBoxBuiState(comp.Instruments));
    }

    private void OnInstrumentSelected(EntityUid uid, MusicianChoiceBoxComponent comp, MusicianChoiceBoxSelectMessage args)
    {
        if (!comp.Instruments.TryGetValue(args.SelectedInstrumentId, out var instrumentName))
            return;

        var user = args.Actor;
        var coords = _xform.GetMapCoordinates(uid);
        var spawned = Spawn(args.SelectedInstrumentId, Transform(uid).Coordinates);

        _audio.PlayPvs(new SoundPathSpecifier("/Audio/Effects/sparks1.ogg"), uid);
        _popup.PopupEntity(Loc.GetString("musician-unboxed-msg", ("instrument", instrumentName), ("fallback", $"¡Has desempaquetado tu {instrumentName}!")), user, user, PopupType.Medium);

        if (!_hands.TryPickupAnyHand(user, spawned))
        {
            _xform.SetWorldPosition(spawned, coords.Position);
        }

        QueueDel(uid);
    }
}
