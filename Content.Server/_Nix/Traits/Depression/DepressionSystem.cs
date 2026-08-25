using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.Damage.Systems;
using Content.Shared._Nix.Traits.Depression;
using Content.Shared.Chat;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Depression;

/// <summary>
/// Server system managing Depression trait with SS13 parity.
/// Periodically sighs heavily with local room emote, popup, notification chat, and stamina drain.
/// </summary>
public sealed class DepressionSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<DepressionComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var actor))
        {
            if (_timing.CurTime < comp.NextEpisodeTime)
                continue;

            comp.NextEpisodeTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(60f, 150f));

            if (_mobState.IsIncapacitated(uid))
                continue;

            var sighText = Loc.GetString("depression-heavy-sigh", ("fallback", "*suspira pesadamente* Todo se siente tan vacío y sin sentido..."));

            // 1. Audible room emote
            _chat.TrySendInGameICMessage(uid, "suspira pesadamente", InGameICChatType.Emote, hideChat: false);

            // 2. Personal popup
            _popup.PopupEntity(sighText, uid, uid, PopupType.Small);

            // 3. Personal persistent chat notification
            var wrapped = $"[italic][color=#7f8c8d]{FormattedMessage.EscapeText(sighText)}[/color][/italic]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                sighText,
                wrapped,
                uid,
                hideChat: false,
                actor.PlayerSession.Channel);

            _stamina.TakeStaminaDamage(uid, 12f);
        }
    }
}
