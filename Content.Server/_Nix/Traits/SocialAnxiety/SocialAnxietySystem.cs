using System.Text;
using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.SocialAnxiety;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Speech;
using Content.Shared.Stunnable;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.SocialAnxiety;

/// <summary>
/// Server system managing Social Anxiety trait with SS13 parity.
/// Causes nervous speech modifications when surrounded by people,
/// and stress panics upon being examined or making direct eye contact.
/// </summary>
public sealed class SocialAnxietySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    private static readonly string[] Fillers = ["eh...", "este...", "um...", "uh..."];

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SocialAnxietyComponent, AccentGetEvent>(OnAccentGet);
        SubscribeLocalEvent<SocialAnxietyComponent, ExaminedEvent>(OnExamined);
    }

    private void OnAccentGet(EntityUid uid, SocialAnxietyComponent comp, AccentGetEvent args)
    {
        if (args.Message == null || string.IsNullOrWhiteSpace(args.Message.Text) || _mobState.IsIncapacitated(uid))
            return;

        var xform = Transform(uid);
        var nearbyCount = 0;
        foreach (var nearby in _lookup.GetEntitiesInRange<HumanoidAppearanceComponent>(xform.Coordinates, 4f))
        {
            if (nearby.Owner != uid && !_mobState.IsDead(nearby.Owner))
                nearbyCount++;
        }

        if (nearbyCount == 0)
            return;

        var words = args.Message.Text.Split(' ');
        var sb = new StringBuilder();

        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            if (string.IsNullOrWhiteSpace(word))
                continue;

            // Chance of filler word
            if (i > 0 && _random.Prob(0.20f))
            {
                sb.Append(_random.Pick(Fillers));
                sb.Append(' ');
            }

            // Chance of stuttering first letter of word
            if (word.Length > 2 && _random.Prob(0.25f))
            {
                var firstChar = word[0];
                if (char.IsLetter(firstChar))
                {
                    sb.Append(firstChar).Append('-').Append(word);
                }
                else
                {
                    sb.Append(word);
                }
            }
            else
            {
                sb.Append(word);
            }

            if (i < words.Length - 1)
                sb.Append(' ');
        }

        args.Message.Text = sb.ToString();
    }

    private void OnExamined(EntityUid uid, SocialAnxietyComponent comp, ExaminedEvent args)
    {
        if (args.Examiner == uid || _mobState.IsIncapacitated(uid))
            return;

        if (_timing.CurTime < comp.LastPanicTime + comp.PanicCooldown)
            return;

        if (!_random.Prob(comp.EyeContactPanicChance))
            return;

        comp.LastPanicTime = _timing.CurTime;
        var examinerName = Identity.Name(args.Examiner, EntityManager);

        string panicMsg;
        PopupType popupType;

        switch (_random.Next(1, 3))
        {
            case 1:
                _jitter.DoJitter(uid, TimeSpan.FromSeconds(8f), true, 10f, 4f);
                panicMsg = Loc.GetString("social-anxiety-fidget", ("examiner", examinerName));
                popupType = PopupType.MediumCaution;
                break;

            default:
                _stun.TryKnockdown(uid, TimeSpan.FromSeconds(1.5f), refresh: true);
                panicMsg = Loc.GetString("social-anxiety-freeze", ("examiner", examinerName));
                popupType = PopupType.LargeCaution;
                break;
        }

        _popup.PopupEntity(panicMsg, uid, uid, popupType);

        if (TryComp<ActorComponent>(uid, out var actor))
        {
            var wrapped = $"[bold][color=#e74c3c]{FormattedMessage.EscapeText(panicMsg)}[/color][/bold]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                panicMsg,
                wrapped,
                uid,
                hideChat: false,
                actor.PlayerSession.Channel);
        }
    }
}
