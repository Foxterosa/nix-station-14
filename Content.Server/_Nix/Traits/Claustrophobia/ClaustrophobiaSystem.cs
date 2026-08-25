using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.Claustrophobia;
using Content.Shared.Chat;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Claustrophobia;

/// <summary>
/// Server system managing the Claustrophobia trait.
/// Triggers panic, asphyxiation damage, visible popups, and persistent chat alerts while inside
/// enclosed lockers, crates, bodybags, disposals, or machinery.
/// </summary>
public sealed class ClaustrophobiaSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ClaustrophobiaComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, ClaustrophobiaComponent component, ComponentStartup args)
    {
        component.NextPanicTime = _timing.CurTime + TimeSpan.FromSeconds(2f);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ClaustrophobiaComponent, DamageableComponent, ActorComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var damageable, out var actor, out var xform))
        {
            var isTrapped = IsInsideEnclosedContainer(uid, xform);

            if (!isTrapped)
            {
                if (comp.CurrentStage > 0 || comp.SecondsTrapped > 0f)
                {
                    // Player was released from container: send personal relief message
                    var reliefText = Loc.GetString("claustrophobia-relief", ("fallback", "¡El aire fresco te llena los pulmones! Ya no estás atrapado."));
                    _popup.PopupEntity(reliefText, uid, uid, PopupType.Small);

                    var wrapped = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(reliefText)}[/color][/italic]";
                    _chatManager.ChatMessageToOne(
                        ChatChannel.Notifications,
                        reliefText,
                        wrapped,
                        uid,
                        hideChat: false,
                        actor.PlayerSession.Channel);

                    comp.SecondsTrapped = 0f;
                    comp.CurrentStage = 0;
                }
                continue;
            }

            comp.SecondsTrapped += frameTime;

            if (_timing.CurTime < comp.NextPanicTime)
                continue;

            comp.NextPanicTime = _timing.CurTime + TimeSpan.FromSeconds(3.0f);

            if (_mobState.IsIncapacitated(uid))
                continue;

            // Determine escalating stage
            int stage;
            float damageAmount;
            string locKey;
            string colorHex;

            if (comp.SecondsTrapped < 10f)
            {
                stage = 1;
                damageAmount = 0.5f;
                locKey = "claustrophobia-panic-stage-1";
                colorHex = "#f39c12";
                _jitter.DoJitter(uid, TimeSpan.FromSeconds(1.5f), true, 4f, 1.5f);
            }
            else if (comp.SecondsTrapped < 25f)
            {
                stage = 2;
                damageAmount = 1.5f;
                locKey = "claustrophobia-panic-stage-2";
                colorHex = "#e67e22";
                _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.5f), true, 8f, 2.5f);
            }
            else
            {
                stage = 3;
                damageAmount = 3.0f;
                locKey = "claustrophobia-panic-stage-3";
                colorHex = "#e74c3c";
                _jitter.DoJitter(uid, TimeSpan.FromSeconds(3.5f), true, 14f, 4f);
            }

            comp.CurrentStage = stage;

            // Apply asphyxiation damage
            var damage = new DamageSpecifier
            {
                DamageDict = new()
                {
                    { "Asphyxiation", damageAmount }
                }
            };
            _damageable.TryChangeDamage(uid, damage, ignoreResistances: true);

            // Send escalating text to popup & chat notification every 8s
            if (_timing.CurTime >= comp.LastChatTime + TimeSpan.FromSeconds(8.0f))
            {
                comp.LastChatTime = _timing.CurTime;
                var panicText = Loc.GetString(locKey, ("fallback", "¡Te sientes atrapado! ¡Debes escapar... no puedes respirar!"));

                _popup.PopupEntity(panicText, uid, uid, PopupType.MediumCaution);

                var wrapped = $"[bold][color={colorHex}]{FormattedMessage.EscapeText(panicText)}[/color][/bold]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    panicText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);
            }
        }
    }

    private bool IsInsideEnclosedContainer(EntityUid uid, TransformComponent xform)
    {
        // 1. Direct parent entity check (e.g. locker, crate, disposal, machine)
        if (xform.ParentUid.IsValid() && xform.ParentUid != xform.GridUid && xform.ParentUid != xform.MapUid)
        {
            var parent = xform.ParentUid;
            if (TryComp<EntityStorageComponent>(parent, out var storage))
                return !storage.Open;

            return true;
        }

        // 2. Container system check
        if (_container.TryGetContainingContainer(uid, out var container))
        {
            var owner = container.Owner;
            if (TryComp<EntityStorageComponent>(owner, out var storage))
                return !storage.Open;

            return true;
        }

        return false;
    }
}
