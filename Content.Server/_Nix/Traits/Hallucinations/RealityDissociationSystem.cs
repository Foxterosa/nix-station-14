using System.Numerics;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Shared._Nix.Traits.Hallucinations;
using Content.Shared.Chat;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.Traits.Hallucinations;

/// <summary>
/// Server system managing Reality Dissociation Syndrome.
/// Ticks hallucination timers and orchestrates psychological incidents, including
/// fake local chat from nearby crew or pets, fake radio broadcasts from real players,
/// and client-side visual/audio illusions.
/// </summary>
public sealed class RealityDissociationSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    private static readonly string[] ChatterKeys =
    [
        "reality-dissociation-chatter-1",
        "reality-dissociation-chatter-2",
        "reality-dissociation-chatter-3",
        "reality-dissociation-chatter-4",
        "reality-dissociation-chatter-5",
        "reality-dissociation-chatter-6",
        "reality-dissociation-chatter-7",
        "reality-dissociation-chatter-8",
        "reality-dissociation-chatter-9",
        "reality-dissociation-chatter-10",
        "reality-dissociation-chatter-11",
        "reality-dissociation-chatter-12",
        "reality-dissociation-chatter-13",
        "reality-dissociation-chatter-14",
        "reality-dissociation-chatter-15",
        "reality-dissociation-chatter-16",
        "reality-dissociation-chatter-17",
        "reality-dissociation-chatter-18",
        "reality-dissociation-chatter-19",
        "reality-dissociation-chatter-20"
    ];

    private static readonly string[] AnimalChatterKeys =
    [
        "reality-dissociation-animal-1",
        "reality-dissociation-animal-2",
        "reality-dissociation-animal-3",
        "reality-dissociation-animal-4",
        "reality-dissociation-animal-5",
        "reality-dissociation-animal-6",
        "reality-dissociation-animal-7",
        "reality-dissociation-animal-8"
    ];

    private static readonly string[] RadioKeys =
    [
        "reality-dissociation-radio-1",
        "reality-dissociation-radio-2",
        "reality-dissociation-radio-3",
        "reality-dissociation-radio-4",
        "reality-dissociation-radio-5",
        "reality-dissociation-radio-6",
        "reality-dissociation-radio-7",
        "reality-dissociation-radio-8",
        "reality-dissociation-radio-9",
        "reality-dissociation-radio-10",
        "reality-dissociation-radio-11",
        "reality-dissociation-radio-12"
    ];

    private static readonly string[] FallbackRadioSenders =
    [
        "Julian Rossi",
        "Matias Romero",
        "Elena Gomez",
        "Lucas Vega",
        "Sofia Morales",
        "Capitan Mendez",
        "Oficial Perez",
        "Dr. Alvarez"
    ];

    private static readonly string[] PhantomMobVariants = ["xeno", "carp", "shadow", "clown", "flesh"];
    private static readonly string[] CombatAudioVariants = ["gunfire", "laser", "esword", "baton", "bomb"];
    private static readonly string[] PhantomItemVariants = ["revolver", "blade", "syringe", "briefcase", "toolbox"];

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RealityDissociationComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, RealityDissociationComponent component, ComponentStartup args)
    {
        component.NextIncidentTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(component.MinTimeBetweenIncidents, component.MaxTimeBetweenIncidents));
        Dirty(uid, component);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<RealityDissociationComponent, ActorComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var comp, out var actor, out var xform))
        {
            if (_timing.CurTime < comp.NextIncidentTime)
                continue;

            comp.NextIncidentTime = _timing.CurTime + TimeSpan.FromSeconds(_random.NextFloat(comp.MinTimeBetweenIncidents, comp.MaxTimeBetweenIncidents));
            Dirty(uid, comp);

            if (_mobState.IsIncapacitated(uid))
                continue;

            TriggerHallucinationIncident(uid, comp, actor, xform);
        }
    }

    /// <summary>
    /// Selects and executes a random hallucination incident for the given player.
    /// </summary>
    private void TriggerHallucinationIncident(
        EntityUid player,
        RealityDissociationComponent comp,
        ActorComponent actor,
        TransformComponent xform)
    {
        var totalWeight = comp.WeightFakeChatter + comp.WeightFakeRadio + comp.WeightPhantomMob + comp.WeightCombatAudio + comp.WeightPhantomItem;
        if (totalWeight <= 0)
            return;

        var roll = _random.NextFloat(0f, totalWeight);

        if (roll < comp.WeightFakeChatter)
        {
            if (TryTriggerFakeChatter(player, actor, xform))
                return;
            // Fallback to fake radio if no nearby mob was found
            TriggerFakeRadio(player, actor);
            return;
        }

        roll -= comp.WeightFakeChatter;
        if (roll < comp.WeightFakeRadio)
        {
            TriggerFakeRadio(player, actor);
            return;
        }

        roll -= comp.WeightFakeRadio;
        if (roll < comp.WeightPhantomMob)
        {
            TriggerPhantomMob(player, actor);
            return;
        }

        roll -= comp.WeightPhantomMob;
        if (roll < comp.WeightCombatAudio)
        {
            TriggerCombatAudio(player, actor);
            return;
        }

        TriggerPhantomItem(player, actor);
    }

    /// <summary>
    /// Looks for ANY nearby mob (crew member, dog, cat, rat, borg, etc.) and makes them appear to speak to the victim.
    /// </summary>
    private bool TryTriggerFakeChatter(EntityUid player, ActorComponent actor, TransformComponent xform)
    {
        var nearby = _lookup.GetEntitiesInRange<MobStateComponent>(xform.Coordinates, 6.5f);
        var candidates = new List<EntityUid>();

        foreach (var candidate in nearby)
        {
            if (candidate.Owner == player)
                continue;

            if (_mobState.IsDead(candidate.Owner))
                continue;

            candidates.Add(candidate.Owner);
        }

        if (candidates.Count == 0)
            return false;

        var speaker = _random.Pick(candidates);
        var speakerName = Identity.Name(speaker, EntityManager);
        var nameLower = speakerName.ToLowerInvariant();

        // Check if the speaker is a pet/animal/creature
        var isAnimal = nameLower.Contains("cat") || nameLower.Contains("gato") ||
                       nameLower.Contains("dog") || nameLower.Contains("perro") ||
                       nameLower.Contains("corgi") || nameLower.Contains("rat") ||
                       nameLower.Contains("rata") || nameLower.Contains("monkey") ||
                       nameLower.Contains("mono") || nameLower.Contains("slime") ||
                       nameLower.Contains("ian") || nameLower.Contains("punpun");

        var phraseKey = isAnimal
            ? _random.Pick(AnimalChatterKeys)
            : _random.Pick(ChatterKeys);

        var phrase = Loc.GetString(phraseKey);
        var wrappedMessage = _chatSystem.WrapPublicMessage(speaker, speakerName, phrase);

        _chatManager.ChatMessageToOne(
            ChatChannel.Local,
            phrase,
            wrappedMessage,
            speaker,
            hideChat: false,
            actor.PlayerSession.Channel);

        return true;
    }

    /// <summary>
    /// Injects a fake emergency radio broadcast onto the victim's headset using real active characters from the match.
    /// </summary>
    private void TriggerFakeRadio(EntityUid player, ActorComponent actor)
    {
        // Find names of real active characters in this round
        var realCrew = new List<string>();
        foreach (var session in _playerManager.Sessions)
        {
            if (session.AttachedEntity is { Valid: true } ent && ent != player && !_mobState.IsDead(ent))
            {
                var name = Identity.Name(ent, EntityManager);
                if (!string.IsNullOrWhiteSpace(name))
                    realCrew.Add(name);
            }
        }

        var senderName = realCrew.Count > 0
            ? _random.Pick(realCrew)
            : _random.Pick(FallbackRadioSenders);

        var phraseKey = _random.Pick(RadioKeys);
        var playerName = Identity.Name(player, EntityManager);
        var phrase = Loc.GetString(phraseKey, ("player", playerName));

        // Format natural radio message
        var wrappedRadio = $"[bold][color=#2ecc71]\\[Common\\] [Name]{senderName}[/Name] radios, \"{FormattedMessage.EscapeText(phrase)}\"[/color][/bold]";

        _chatManager.ChatMessageToOne(
            ChatChannel.Radio,
            phrase,
            wrappedRadio,
            player,
            hideChat: false,
            actor.PlayerSession.Channel);
    }

    /// <summary>
    /// Dispatches a phantom monster incident event to the client.
    /// </summary>
    private void TriggerPhantomMob(EntityUid player, ActorComponent actor)
    {
        var variant = _random.Pick(PhantomMobVariants);
        var angle = _random.NextFloat(0f, MathF.Tau);
        var distance = _random.NextFloat(3.5f, 6.0f);
        var offset = new Vector2(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);

        var ev = new RealityDissociationIncidentEvent(
            GetNetEntity(player),
            RealityDissociationIncidentType.PhantomMob,
            variant,
            offset);

        RaiseNetworkEvent(ev, actor.PlayerSession.Channel);

        // Send a chilling direct message from the phantom monster
        var monsterChatKey = $"reality-dissociation-monster-{variant}";
        if (Loc.TryGetString(monsterChatKey, out var monsterPhrase))
        {
            var monsterName = Loc.GetString($"reality-dissociation-monster-name-{variant}");
            var wrappedMonster = $"[italic][color=#e74c3c][BubbleHeader][Name]{monsterName}[/Name][/BubbleHeader] hisses: \"[BubbleContent]{FormattedMessage.EscapeText(monsterPhrase)}[/BubbleContent]\"[/color][/italic]";

            _chatManager.ChatMessageToOne(
                ChatChannel.Local,
                monsterPhrase,
                wrappedMonster,
                player,
                hideChat: false,
                actor.PlayerSession.Channel);
        }
    }

    /// <summary>
    /// Dispatches an illusory combat audio event to the client (sound only, no chat).
    /// </summary>
    private void TriggerCombatAudio(EntityUid player, ActorComponent actor)
    {
        var variant = _random.Pick(CombatAudioVariants);
        var angle = _random.NextFloat(0f, MathF.Tau);
        var distance = _random.NextFloat(5.0f, 8.5f);
        var offset = new Vector2(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);

        var ev = new RealityDissociationIncidentEvent(
            GetNetEntity(player),
            RealityDissociationIncidentType.CombatAudio,
            variant,
            offset);

        RaiseNetworkEvent(ev, actor.PlayerSession.Channel);
    }

    /// <summary>
    /// Dispatches a phantom floor item event to the client.
    /// </summary>
    private void TriggerPhantomItem(EntityUid player, ActorComponent actor)
    {
        var variant = _random.Pick(PhantomItemVariants);
        var angle = _random.NextFloat(0f, MathF.Tau);
        var distance = _random.NextFloat(2.0f, 4.0f);
        var offset = new Vector2(MathF.Cos(angle) * distance, MathF.Sin(angle) * distance);

        var ev = new RealityDissociationIncidentEvent(
            GetNetEntity(player),
            RealityDissociationIncidentType.PhantomItem,
            variant,
            offset);

        RaiseNetworkEvent(ev, actor.PlayerSession.Channel);
    }
}
