using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.BrainTumor;
using Content.Shared.Body.Components;
using Content.Shared.Chat;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Jittering;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.BrainTumor;

/// <summary>
/// Server system managing Brain Tumor trait with full SS13 parity.
/// Periodically inflicts progressive brain damage and headache symptoms unless patient is medicated with suppressants.
/// Ingesting Mannitol or neurological medicine suppresses all pain and damage for 5 minutes (300 seconds).
/// </summary>
public sealed class BrainTumorSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BrainTumorComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, BrainTumorComponent component, ComponentStartup args)
    {
        component.NextDamageTime = _timing.CurTime + TimeSpan.FromSeconds(component.DamageInterval);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<BrainTumorComponent, DamageableComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var damageable, out var actor))
        {
            if (_mobState.IsDead(uid))
                continue;

            // 1. Check if patient has recently taken medicine that should initiate or renew suppression
            if (HasMedicinePresent(uid, comp))
            {
                var targetSuppression = _timing.CurTime + TimeSpan.FromSeconds(comp.SuppressionDuration);
                if (comp.SuppressedUntil < _timing.CurTime + TimeSpan.FromSeconds(60f))
                {
                    comp.SuppressedUntil = targetSuppression;
                    comp.NextDamageTime = targetSuppression + TimeSpan.FromSeconds(comp.DamageInterval);

                    var reliefText = Loc.GetString("brain-tumor-medicated-relief", ("fallback", "El medicamento hace efecto en tu organismo... el agudo dolor de cabeza se calma por completo."));
                    _popup.PopupEntity(reliefText, uid, uid, PopupType.Medium);

                    var wrapped = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(reliefText)}[/color][/italic]";
                    _chatManager.ChatMessageToOne(
                        ChatChannel.Notifications,
                        reliefText,
                        wrapped,
                        uid,
                        hideChat: false,
                        actor.PlayerSession.Channel);
                }
            }

            // 2. If currently within the 5-minute suppression window, skip all symptoms and damage
            if (_timing.CurTime < comp.SuppressedUntil)
                continue;

            // 3. Check damage tick interval
            if (_timing.CurTime < comp.NextDamageTime)
                continue;

            comp.NextDamageTime = _timing.CurTime + TimeSpan.FromSeconds(comp.DamageInterval);

            // Apply damage
            _damageable.TryChangeDamage(uid, comp.Damage, ignoreResistances: true);

            var headacheText = Loc.GetString("brain-tumor-headache", ("fallback", "Sientes una punzada aguda de dolor en la cabeza..."));

            // Floating popup directly on screen (visible only to player)
            _popup.PopupEntity(headacheText, uid, uid, PopupType.SmallCaution);

            // Personal persistent chat notification
            var wrappedHeadache = $"[bold][color=#e74c3c]{FormattedMessage.EscapeText(headacheText)}[/color][/bold]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                headacheText,
                wrappedHeadache,
                uid,
                hideChat: false,
                actor.PlayerSession.Channel);

            // Light jitter from the pain
            _jitter.DoJitter(uid, TimeSpan.FromSeconds(1.2f), true, 4f, 1.5f);
        }
    }

    private bool HasMedicinePresent(EntityUid uid, BrainTumorComponent comp)
    {
        if (TryComp<Content.Shared.Chemistry.Components.SolutionManager.SolutionContainerManagerComponent>(uid, out var solManager))
        {
            foreach (var (_, solEntity) in _solution.EnumerateSolutions((uid, solManager)))
            {
                foreach (var suppressant in comp.Suppressants)
                {
                    if (solEntity.Comp.Solution.ContainsReagent(new ReagentId(suppressant, null)))
                        return true;
                }
            }
        }

        return false;
    }
}
