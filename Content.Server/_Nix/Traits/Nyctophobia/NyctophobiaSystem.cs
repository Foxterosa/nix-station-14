using System.Numerics;
using Content.Server.Chat.Managers;
using Content.Server.Damage.Systems;
using Content.Shared._Nix.Traits.Nyctophobia;
using Content.Shared.Chat;
using Content.Shared.Damage.Systems;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Jittering;
using Content.Shared.Light.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Nyctophobia;

/// <summary>
/// Server system managing the Nyctophobia (fear of the dark) trait with full SS13 parity.
/// Evaluates point lights, flashlights, and room illumination. In true darkness:
/// slows down movement, triggers periodic panic sensations, jitters, and blocks/penalizes sprinting in the dark.
/// </summary>
public sealed class NyctophobiaSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly MovementSpeedModifierSystem _speedModifier = default!;

    private static readonly string[] DarknessSensations =
    [
        "nyctophobia-stage-1-unease",
        "nyctophobia-stage-2-dread",
        "nyctophobia-stage-3-terror",
    ];

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<NyctophobiaComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMovementSpeedModifiers);
    }

    private void OnRefreshMovementSpeedModifiers(EntityUid uid, NyctophobiaComponent comp, RefreshMovementSpeedModifiersEvent args)
    {
        if (comp.CurrentStage >= 2)
        {
            // Severe slowdown in deep darkness
            args.ModifySpeed(0.65f, 0.55f);
        }
        else if (comp.CurrentStage >= 1)
        {
            args.ModifySpeed(0.85f, 0.75f);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<NyctophobiaComponent, TransformComponent, ActorComponent>();
        while (query.MoveNext(out var uid, out var comp, out var xform, out var actor))
        {
            if (_mobState.IsIncapacitated(uid))
                continue;

            var isLit = IsInLight(uid, xform);

            if (isLit)
            {
                // In well-lit area: reset
                if (comp.CurrentStage > 0 || comp.TimeInDark > 0f)
                {
                    comp.CurrentStage = 0;
                    comp.TimeInDark = 0f;
                    _speedModifier.RefreshMovementSpeedModifiers(uid);

                    var reliefText = Loc.GetString("nyctophobia-light-relief", ("fallback", "La luz te devuelve la calma... respiras aliviado."));
                    _popup.PopupEntity(reliefText, uid, uid, PopupType.Small);

                    var wrappedRelief = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(reliefText)}[/color][/italic]";
                    _chatManager.ChatMessageToOne(ChatChannel.Notifications, reliefText, wrappedRelief, uid, false, actor.PlayerSession.Channel);
                }
                continue;
            }

            // In genuine darkness
            comp.TimeInDark += frameTime;

            // Determine stage
            var prevStage = comp.CurrentStage;
            if (comp.TimeInDark >= 25f)
                comp.CurrentStage = 3;
            else if (comp.TimeInDark >= 12f)
                comp.CurrentStage = 2;
            else if (comp.TimeInDark >= 4f)
                comp.CurrentStage = 1;

            if (comp.CurrentStage != prevStage)
            {
                _speedModifier.RefreshMovementSpeedModifiers(uid);
            }

            // Sprinting in the dark triggers stumble/walk enforcement and SS13 warning
            if (TryComp<InputMoverComponent>(uid, out var mover) && mover.Sprinting && _timing.CurTime >= comp.NextSprintWarningTime)
            {
                comp.NextSprintWarningTime = _timing.CurTime + TimeSpan.FromSeconds(10.0f);

                var slowText = Loc.GetString("nyctophobia-darkness-panic", ("fallback", "Fácil, con calma, despacio... estás en la oscuridad..."));
                _popup.PopupEntity(slowText, uid, uid, PopupType.MediumCaution);

                var wrapped = $"[bold][color=#f39c12]{FormattedMessage.EscapeText(slowText)}[/color][/bold]";
                _chatManager.ChatMessageToOne(ChatChannel.Notifications, slowText, wrapped, uid, false, actor.PlayerSession.Channel);

                _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.0f), true, 6f, 2f);
                _stamina.TakeStaminaDamage(uid, 12f);

                if (_random.Prob(0.35f))
                {
                    _stun.TryKnockdown(uid, TimeSpan.FromSeconds(1.0f), refresh: true);
                }
            }

            // Periodic panic sensations (every 8-10 seconds while trapped in the dark)
            if (_timing.CurTime < comp.NextStageCheckTime)
                continue;

            comp.NextStageCheckTime = _timing.CurTime + TimeSpan.FromSeconds(9.0f);

            var locKey = DarknessSensations[Math.Clamp(comp.CurrentStage - 1, 0, DarknessSensations.Length - 1)];
            var panicText = Loc.GetString(locKey, ("fallback", "La oscuridad te envuelve... sientes que algo te acecha desde las sombras."));

            var pType = comp.CurrentStage >= 3 ? PopupType.LargeCaution : PopupType.MediumCaution;
            var cHex = comp.CurrentStage >= 3 ? "#e74c3c" : "#f39c12";

            _popup.PopupEntity(panicText, uid, uid, pType);

            var wrappedPanic = $"[bold][color={cHex}]{FormattedMessage.EscapeText(panicText)}[/color][/bold]";
            _chatManager.ChatMessageToOne(ChatChannel.Notifications, panicText, wrappedPanic, uid, false, actor.PlayerSession.Channel);

            _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.5f), true, 8f, 2.5f);
            _stamina.TakeStaminaDamage(uid, 10f);
        }
    }

    private bool IsInLight(EntityUid uid, TransformComponent xform)
    {
        // 1. Held active light source in hands (flashlight, glowstick, flare, lantern)
        foreach (var item in _hands.EnumerateHeld(uid))
        {
            if (TryComp<PointLightComponent>(item, out var itemLight) && itemLight.Enabled)
                return true;
            if (TryComp<HandheldLightComponent>(item, out var handheld) && handheld.Activated)
                return true;
            if (TryComp<UnpoweredFlashlightComponent>(item, out var unpowered) && unpowered.LightOn)
                return true;
        }

        // 2. Query nearby lights on station / grid
        var lights = _lookup.GetEntitiesInRange<PointLightComponent>(xform.Coordinates, 8f);
        foreach (var light in lights)
        {
            if (!light.Comp.Enabled || light.Comp.Radius < 1f || light.Comp.Energy <= 0f)
                continue;

            // If it belongs to a wall light fixture, check if powered on
            if (TryComp<PoweredLightComponent>(light.Owner, out var powered) && !powered.On)
                continue;

            var dist = Vector2.Distance(xform.Coordinates.Position, Transform(light.Owner).Coordinates.Position);
            if (dist <= light.Comp.Radius)
                return true;
        }

        return false;
    }
}
