using Content.Server.Chat.Managers;
using Content.Shared._Nix.Traits.Diet;
using Content.Shared.Chat;
using Content.Shared.Damage.Systems;
using Content.Shared.Jittering;
using Content.Shared.Medical;
using Content.Shared.Nutrition;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Diet;

/// <summary>
/// Server system managing dietary traits: Vegetarian, Carnivore, PineappleLiker, PineappleHater, and Voracious.
/// Subscribes to IngestingEvent directly on the character component when consuming food.
/// Violating dietary traits causes disgust, jittering, stamina drain, gagging, and eventual vomiting (SS13 parity).
/// </summary>
public sealed class DietTraitsSystem : EntitySystem
{
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;
    [Dependency] private readonly VomitSystem _vomit = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<VoraciousComponent, IngestingEvent>(OnVoraciousIngesting);
        SubscribeLocalEvent<VegetarianComponent, IngestingEvent>(OnVegetarianIngesting);
        SubscribeLocalEvent<CarnivoreComponent, IngestingEvent>(OnCarnivoreIngesting);
        SubscribeLocalEvent<PineappleHaterComponent, IngestingEvent>(OnPineappleHaterIngesting);
        SubscribeLocalEvent<PineappleLikerComponent, IngestingEvent>(OnPineappleLikerIngesting);
    }

    private void OnVoraciousIngesting(EntityUid uid, VoraciousComponent comp, ref IngestingEvent args)
    {
        _popup.PopupEntity(Loc.GetString("trait-voracious-devour", ("fallback", "¡Devoras la comida con una velocidad y apetito descomunal!")), uid, uid, PopupType.Small);
    }

    private void OnVegetarianIngesting(EntityUid uid, VegetarianComponent comp, ref IngestingEvent args)
    {
        if (IsMeatFood(args.Food, args.Split))
        {
            var disgustText = Loc.GetString("trait-vegetarian-disgust", ("fallback", "¡Sientes una profunda repulsión y náuseas al haber comido carne!"));
            ApplyDietDisgust(uid, disgustText, "#e67e22");
        }
        else
        {
            var delightText = Loc.GetString("trait-vegetarian-delight", ("fallback", "¡Disfrutas de una comida fresca, sana y libre de carne!"));
            _popup.PopupEntity(delightText, uid, uid, PopupType.Small);

            if (TryComp<ActorComponent>(uid, out var actor))
            {
                var wrapped = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(delightText)}[/color][/italic]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    delightText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);
            }
        }
    }

    private void OnCarnivoreIngesting(EntityUid uid, CarnivoreComponent comp, ref IngestingEvent args)
    {
        if (IsMeatFood(args.Food, args.Split))
        {
            var delightText = Loc.GetString("trait-carnivore-delight", ("fallback", "¡El potente y exquisito sabor a carne te llena de energía y satisfacción!"));
            _popup.PopupEntity(delightText, uid, uid, PopupType.Small);

            if (TryComp<ActorComponent>(uid, out var actor))
            {
                var wrapped = $"[italic][color=#e74c3c]{FormattedMessage.EscapeText(delightText)}[/color][/italic]";
                _chatManager.ChatMessageToOne(
                    ChatChannel.Notifications,
                    delightText,
                    wrapped,
                    uid,
                    hideChat: false,
                    actor.PlayerSession.Channel);
            }
        }
        else
        {
            var disgustText = Loc.GetString("trait-carnivore-disgust", ("fallback", "¡Sientes una profunda repulsión al comer vegetales! ¡Tu cuerpo exige carne!"));
            ApplyDietDisgust(uid, disgustText, "#c0392b");
        }
    }

    private void OnPineappleHaterIngesting(EntityUid uid, PineappleHaterComponent comp, ref IngestingEvent args)
    {
        if (!HasPineapple(args.Food, args.Split))
            return;

        var hateText = Loc.GetString("trait-pineapple-hater-disgust", ("fallback", "¡Qué asco tan inmundo! ¿Cómo puede alguien comer piña?!"));
        ApplyDietDisgust(uid, hateText, "#e74c3c");
    }

    private void OnPineappleLikerIngesting(EntityUid uid, PineappleLikerComponent comp, ref IngestingEvent args)
    {
        if (!HasPineapple(args.Food, args.Split))
            return;

        var delightText = Loc.GetString("trait-pineapple-liker-delight", ("fallback", "¡El delicioso y dulce sabor a piña te llena de satisfacción y alegría!"));
        _popup.PopupEntity(delightText, uid, uid, PopupType.Small);

        if (TryComp<ActorComponent>(uid, out var actor))
        {
            var wrapped = $"[italic][color=#2ecc71]{FormattedMessage.EscapeText(delightText)}[/color][/italic]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                delightText,
                wrapped,
                uid,
                hideChat: false,
                actor.PlayerSession.Channel);
        }
    }

    private void ApplyDietDisgust(EntityUid uid, string messageText, string colorHex)
    {
        _popup.PopupEntity(messageText, uid, uid, PopupType.MediumCaution);
        _jitter.DoJitter(uid, TimeSpan.FromSeconds(2.0f), true, 6f, 2f);

        // SS13 Parity: Stamina fatigue and nausea
        _stamina.TakeStaminaDamage(uid, 12f);

        // Gagging & eventual vomiting
        var roll = _random.NextFloat();
        if (roll < 0.25f)
        {
            _vomit.Vomit(uid);
        }
        else if (roll < 0.60f)
        {
            _popup.PopupEntity(Loc.GetString("trait-diet-gagging", ("fallback", "¡Sufres fuertes arcadas intentando contener el vómito!")), uid, uid, PopupType.MediumCaution);
        }

        if (TryComp<ActorComponent>(uid, out var actor))
        {
            var wrapped = $"[bold][color={colorHex}]{FormattedMessage.EscapeText(messageText)}[/color][/bold]";
            _chatManager.ChatMessageToOne(
                ChatChannel.Notifications,
                messageText,
                wrapped,
                uid,
                hideChat: false,
                actor.PlayerSession.Channel);
        }
    }

    private bool IsMeatFood(EntityUid food, Content.Shared.Chemistry.Components.Solution split)
    {
        if (_tag.HasTag(food, "Meat"))
            return true;

        if (TryComp<FlavorProfileComponent>(food, out var flavorProfile))
        {
            foreach (var flavor in flavorProfile.Flavors)
            {
                var f = flavor.ToLowerInvariant();
                if (f is "meat" or "meaty" or "chicken" or "pork" or "beef" or "bacon" or "fish" or "fishy" or "raw_meat" or "bloody" or "salami" or "sausage")
                    return true;
            }
        }

        foreach (var reagent in split.Contents)
        {
            var id = reagent.Reagent.Prototype.ToLowerInvariant();
            if (id.Contains("meat") || id.Contains("blood") || id.Contains("fat"))
                return true;
        }

        return false;
    }

    private bool HasPineapple(EntityUid food, Content.Shared.Chemistry.Components.Solution split)
    {
        if (TryComp<FlavorProfileComponent>(food, out var flavorProfile))
        {
            foreach (var flavor in flavorProfile.Flavors)
            {
                var f = flavor.ToLowerInvariant();
                if (f.Contains("pineapple") || f.Contains("ananas") || f.Contains("hawaiian"))
                    return true;
            }
        }

        foreach (var reagent in split.Contents)
        {
            var id = reagent.Reagent.Prototype.ToLowerInvariant();
            if (id.Contains("pineapple") || id.Contains("ananas"))
                return true;
        }

        return false;
    }
}
