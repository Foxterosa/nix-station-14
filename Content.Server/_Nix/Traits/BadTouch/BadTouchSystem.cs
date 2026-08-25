using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Shared._Nix.Traits.BadTouch;
using Content.Shared.Chat;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.BadTouch;

/// <summary>
/// Server system managing the Bad Touch (Haphephobia) trait.
/// Triggers shuddering recoil, visible disgust emote, and distinct private feedback for both parties when touched.
/// </summary>
public sealed class BadTouchSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BadTouchComponent, InteractHandEvent>(OnInteractHand);
        SubscribeLocalEvent<BadTouchComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void OnInteractHand(EntityUid uid, BadTouchComponent comp, InteractHandEvent args)
    {
        if (args.User == uid)
            return;

        TriggerDiscomfort(uid, comp, args.User);
    }

    private void OnInteractUsing(EntityUid uid, BadTouchComponent comp, InteractUsingEvent args)
    {
        if (args.User == uid)
            return;

        TriggerDiscomfort(uid, comp, args.User);
    }

    private void TriggerDiscomfort(EntityUid uid, BadTouchComponent comp, EntityUid toucher)
    {
        if (_timing.CurTime < comp.LastAnnoyedTime + comp.AnnoyedCooldown)
            return;

        comp.LastAnnoyedTime = _timing.CurTime;

        var victimName = Identity.Name(uid, EntityManager);
        var toucherName = Identity.Name(toucher, EntityManager);

        // 1. Shudder / tremor animation on quirk holder
        _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.0f), true, 6f, 2f);

        // 2. Audible / visible local emote for the whole room
        var emoteText = Loc.GetString("bad-touch-emote", ("name", victimName), ("fallback", "se aparta bruscamente con repulsión y desagrado ante el contacto físico."));
        _chat.TrySendInGameICMessage(uid, emoteText, InGameICChatType.Emote, hideChat: false);

        // 3. Private popup and chat notification to the quirk holder
        var warningText = Loc.GetString("trait-bad-touch-recoil", ("toucher", toucherName), ("fallback", "¡Te invaden el espacio personal! ¡Odias que te toquen!"));
        _popup.PopupEntity(warningText, uid, uid, PopupType.SmallCaution);

        if (TryComp<ActorComponent>(uid, out var victimActor))
        {
            var wrapped = $"[bold][color=#e74c3c]{FormattedMessage.EscapeText(warningText)}[/color][/bold]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                warningText,
                wrapped,
                uid,
                hideChat: false,
                victimActor.PlayerSession.Channel);
        }

        // 4. Private popup and chat notification to the toucher
        var toucherText = Loc.GetString("bad-touch-toucher-feedback", ("name", victimName), ("fallback", $"{victimName} tiembla al ser tocado y se aparta bruscamente; no se ve para nada cómodo."));
        _popup.PopupEntity(toucherText, uid, toucher, PopupType.SmallCaution);

        if (TryComp<ActorComponent>(toucher, out var toucherActor))
        {
            var wrappedToucher = $"[italic][color=#f39c12]{FormattedMessage.EscapeText(toucherText)}[/color][/italic]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                toucherText,
                wrappedToucher,
                toucher,
                hideChat: false,
                toucherActor.PlayerSession.Channel);
        }
    }
}
