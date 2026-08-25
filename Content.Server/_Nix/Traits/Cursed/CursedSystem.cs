using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.Cursed;
using Content.Shared.Chat;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Cursed;

/// <summary>
/// Server system managing Cursed (Bad Luck) trait with SS13 parity.
/// Randomly triggers comical clumsy mishaps, tripping, or item drops with popup and notification chat.
/// </summary>
public sealed class CursedSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<CursedComponent, InputMoverComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var mover, out var actor))
        {
            if (_timing.CurTime < comp.NextCurseCheck)
                continue;

            comp.NextCurseCheck = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(20f, 40f));

            if (!mover.Sprinting)
                continue;

            // 20% chance during sprint tick of a bad luck stumble
            if (_random.Prob(0.20f))
            {
                var stumbleText = Loc.GetString("cursed-bad-luck-stumble", ("fallback", "¡Tropiezas torpemente con tus propios pies!"));
                _popup.PopupEntity(stumbleText, uid, uid, PopupType.SmallCaution);

                var wrapped = $"[italic][color=#9b59b6]{FormattedMessage.EscapeText(stumbleText)}[/color][/italic]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    stumbleText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);

                // Drop held active item or trip
                if (_hands.TryGetActiveItem(uid, out var held) && _random.Prob(0.5f))
                {
                    _hands.TryDrop(uid, held.Value);
                }
                else
                {
                    _stun.TryKnockdown(uid, TimeSpan.FromSeconds(1.0f), true);
                }
            }
        }
    }
}
