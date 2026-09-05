using System.Linq;
using Content.Server.GameTicking;
using Content.Server.RoundEnd;
using Content.Server.StationEvents.Components;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.EntityTable;

namespace Content.Server.StationEvents;

public sealed partial class EventManagerSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _configurationManager = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private IPrototypeManager _prototype = default!;
    [Dependency] private EntityTableSystem _entityTable = default!;
    [Dependency] public GameTicker GameTicker = default!;
    [Dependency] private RoundEndSystem _roundEnd = default!;

    public bool EventsEnabled { get; private set; }

    /// <summary>
    /// Sets whether station events are globally enabled.
    /// </summary>
    private void SetEnabled(bool value) => EventsEnabled = value;

    public Dictionary<EntityPrototype, StationEventComponent>? AllEventCache;
    private HashSet<string>? _allEventIdsCache;
    private List<KeyValuePair<EntityPrototype, StationEventComponent>>? _allEventsOrderedCache;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);
        Subs.CVar(_configurationManager, CCVars.EventsEnabled, SetEnabled, true);
    }

    /// <summary>
    /// Invalidates cached event prototype collections when prototypes are reloaded.
    /// </summary>
    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<EntityPrototype>())
        {
            AllEventCache = GetAllEvents();
            _allEventIdsCache = null;
            _allEventsOrderedCache = null;
        }
    }

    /// <summary>
    ///     The events ordered by id. The admin panel asks for them this way once per second
    ///     and the order does not change between prototype reloads, so sort once.
    ///     The returned list is shared: do not mutate it.
    /// </summary>
    public IReadOnlyList<KeyValuePair<EntityPrototype, StationEventComponent>> AllEventsOrdered()
    {
        return _allEventsOrderedCache ??= AllEvents()
            .OrderBy(pair => pair.Key.ID)
            .ToList();
    }

    /// <summary>
    /// Randomly runs an event from provided EntityTableSelector.
    /// </summary>
    public void RunRandomEvent(EntityTableSelector limitedEventsTable)
    {
        if (!TryBuildLimitedEvents(limitedEventsTable, out var limitedEvents))
        {
            Log.Warning("Provided event table could not build dict!");
            return;
        }

        // This picks the event. Arguably we should be doing this with GetSpawns but that would be a massive amount of YAML slop.
        // Or you'd need a new table prototype which inherits from EntityTables with its own logic for events.
        // It's a ton of effort that only results in Events being able to use GroupSelectors so not worth it unless you're insane.
        if (FindEvent(limitedEvents) is not { } randomLimitedEvent)
        {
            Log.Warning("The selected random event is null!");
            return;
        }

        if (!_prototype.Resolve(randomLimitedEvent, out _))
        {
            Log.Warning("A requested event is not available!");
            return;
        }

        GameTicker.AddGameRule(randomLimitedEvent);
    }

    // Starlight-start
    /// <summary>
    ///     Whether a prototype with this id exists and is a station event.
    /// </summary>
    /// <param name="eventId">Prototype id of the event.</param>
    /// <returns>True when the id names a known station event.</returns>
    public bool HasEvent(string eventId)
    {
        // Cached set rather than scanning the keys: this runs on every Schedule and RunEventById.
        _allEventIdsCache ??= AllEvents().Keys.Select(proto => proto.ID).ToHashSet();
        return _allEventIdsCache.Contains(eventId);
    }

    /// <summary>
    ///     Runs a station event by id, whatever the scheduler would have chosen.
    /// </summary>
    /// <param name="eventId">Prototype id of the event to run.</param>
    /// <returns>
    ///     False when no station event carries that id, or when the game rule could not be
    ///     added. Restrictions such as minimum players are not consulted: this is the path an
    ///     admin takes to run an event deliberately.
    /// </returns>
    public bool RunEventById(string eventId)
    {
        if (!HasEvent(eventId))
            return false;

        return GameTicker.AddGameRule(eventId) != EntityUid.Invalid;
    }
    // Starlight-end

    /// <inheritdoc cref="TryBuildLimitedEvents(IEnumerable{EntProtoId},out Dictionary{EntityPrototype,StationEventComponent},TimeSpan?,int?)"/>
    public bool TryListLimitedEvents(
        EntityTableSelector limitedEventsTable,
        out Dictionary<EntityPrototype, StationEventComponent> limitedEvents,
        TimeSpan? currentTime = null,
        int? playerCount = null)
    {
        var selectedEvents = _entityTable.ListSpawns(limitedEventsTable);

        return TryBuildLimitedEvents(selectedEvents, out limitedEvents, currentTime, playerCount);
    }

    /// <inheritdoc cref="TryBuildLimitedEvents(IEnumerable{EntProtoId},out Dictionary{EntityPrototype,StationEventComponent},TimeSpan?,int?)"/>
    public bool TryBuildLimitedEvents(
        EntityTableSelector limitedEventsTable,
        out Dictionary<EntityPrototype, StationEventComponent> limitedEvents,
        TimeSpan? currentTime = null,
        int? playerCount = null)
    {
        var selectedEvents = _entityTable.GetSpawns(limitedEventsTable);

        return TryBuildLimitedEvents(selectedEvents, out limitedEvents, currentTime, playerCount);
    }

    /// <summary>
    /// Builds a dictionary of valid event prototypes from a list of <see cref="EntProtoId"/>.
    /// Dictionary output consists of the valid prototype as the key, and the <see cref="StationEventComponent"/> as the value.
    /// </summary>
    /// <param name="selectedEvents">List of events we're selecting from.</param>
    /// <param name="limitedEvents">Dictionary we're outputting.</param>
    /// <param name="currentTime">Optional override for station time.</param>
    /// <param name="playerCount">Optional override for playerCount.</param>
    /// <returns>Returns true if the provided EntProtoId list has at least one prototype with a StationEventComp that can successfully run!</returns>
    public bool TryBuildLimitedEvents(
        IEnumerable<EntProtoId> selectedEvents,
        out Dictionary<EntityPrototype, StationEventComponent> limitedEvents,
        TimeSpan? currentTime = null,
        int? playerCount = null)
    {
        limitedEvents = new Dictionary<EntityPrototype, StationEventComponent>();

        playerCount ??= _playerManager.PlayerCount;

        // playerCount does a lock so we'll just keep the variable here
        currentTime ??= GameTicker.RoundDuration();

        foreach (var eventid in selectedEvents)
        {
            if (GameTicker.IsIgnored(eventid))
                continue;

            if (!_prototype.Resolve(eventid, out var eventproto))
            {
                Log.Warning("An event ID has no prototype index!");
                continue;
            }

            if (limitedEvents.ContainsKey(eventproto)) // This stops it from dying if you add duplicate entries in a fucked table
                continue;

            if (eventproto.Abstract)
                continue;

            if (!eventproto.TryGetComponent<StationEventComponent>(out var stationEvent, EntityManager.ComponentFactory))
                continue;

            if (!CanRun(eventproto, stationEvent, playerCount.Value, currentTime.Value))
                continue;

            limitedEvents.Add(eventproto, stationEvent);
        }

        if (!limitedEvents.Any())
            return false;

        return true;
    }

    /// <summary>
    /// Randomly picks a valid event.
    /// </summary>
    public string? PickRandomEvent()
    {
        var availableEvents = AvailableEvents();
        Log.Info($"Picking from {availableEvents.Count} total available events");
        return FindEvent(availableEvents);
    }

    /// <summary>
    /// Pick a random event from the available events at this time, also considering their weightings.
    /// </summary>
    /// <returns></returns>
    // Starlight-start
    /// <summary>
    ///     An event's weight after accounting for how often it has already run this round.
    /// </summary>
    /// <remarks>
    ///     Plain weighted selection has no memory, so the same event can be drawn several times
    ///     in a row purely by chance, which reads as the scheduler being broken. Decaying the
    ///     weight per occurrence makes repeats progressively less likely while still leaving
    ///     them possible. Controlled by <c>events.repetition_falloff</c>, which ships at 0.6;
    ///     setting it to 1 disables the penalty.
    /// </remarks>
    public float GetEffectiveWeight(EntityPrototype prototype, StationEventComponent stationEvent)
    {
        var falloff = Math.Clamp(_configurationManager.GetCVar(CCVars.EventsRepetitionFalloff), 0f, 1f);
        if (MathHelper.CloseTo(falloff, 1f))
            return stationEvent.Weight;

        var occurrences = GetOccurrences(prototype);
        if (occurrences <= 0)
            return stationEvent.Weight;

        return stationEvent.Weight * MathF.Pow(falloff, occurrences);
    }

    // Starlight-end
    public string? FindEvent(Dictionary<EntityPrototype, StationEventComponent> availableEvents)
    {
        if (availableEvents.Count == 0)
        {
            Log.Warning("No events were available to run!");
            return null;
        }

        var sumOfWeights = 0.0f;

        foreach (var (proto, stationEvent) in availableEvents) // Starlight
        {
            sumOfWeights += GetEffectiveWeight(proto, stationEvent); // Starlight
        }

        sumOfWeights = _random.NextFloat(sumOfWeights);

        foreach (var (proto, stationEvent) in availableEvents)
        {
            sumOfWeights -= GetEffectiveWeight(proto, stationEvent); // Starlight

            if (sumOfWeights <= 0.0f)
            {
                return proto.ID;
            }
        }

        Log.Error("Event was not found after weighted pick process!");
        return null;
    }

    /// <summary>
    /// Gets the events that have met their player count, time-until start, etc.
    /// </summary>
    /// <param name="playerCountOverride">Override for player count, if using this to simulate events rather than in an actual round.</param>
    /// <param name="currentTimeOverride">Override for round time, if using this to simulate events rather than in an actual round.</param>
    /// <returns></returns>
    public Dictionary<EntityPrototype, StationEventComponent> AvailableEvents(
        int? playerCountOverride = null,
        TimeSpan? currentTimeOverride = null)
    {
        var playerCount = playerCountOverride ?? _playerManager.PlayerCount;

        // playerCount does a lock so we'll just keep the variable here
        var currentTime = currentTimeOverride ?? GameTicker.RoundDuration();

        var result = new Dictionary<EntityPrototype, StationEventComponent>();

        foreach (var (proto, stationEvent) in AllEvents())
        {
            if (CanRun(proto, stationEvent, playerCount, currentTime))
            {
                result.Add(proto, stationEvent);
            }
        }

        return result;
    }

    /// <summary>
    /// Returns all events prototypes which exist. Prioritizes the cache.
    /// </summary>
    /// <returns>All event prototypes, and their event component.</returns>
    public Dictionary<EntityPrototype, StationEventComponent> AllEvents()
    {
        return AllEventCache ?? GetAllEvents();
    }

    /// <summary>
    /// Gets all event prototypes that exist. Private because you should be using the cache!
    /// </summary>
    private Dictionary<EntityPrototype, StationEventComponent> GetAllEvents()
    {
        var allEvents = new Dictionary<EntityPrototype, StationEventComponent>();
        foreach (var prototype in _prototype.EnumeratePrototypes<EntityPrototype>())
        {
            if (prototype.Abstract)
                continue;

            if (!prototype.TryGetComponent<StationEventComponent>(out var stationEvent, EntityManager.ComponentFactory))
                continue;

            allEvents.Add(prototype, stationEvent);
        }

        AllEventCache = allEvents; // Starlight
        return allEvents;
    }

    /// <summary>
    ///     How many times this event has already run in the current round.
    /// </summary>
    /// <param name="stationEvent">Prototype of the event to count.</param>
    /// <returns>The number of times its game rule was added this round, zero if never.</returns>
    public int GetOccurrences(EntityPrototype stationEvent) // Starlight
    {
        return GetOccurrences(stationEvent.ID);
    }

    /// <summary>
    /// How many times each event has run this round, counted in a single pass.
    /// </summary>
    /// <remarks>
    /// The admin panel projects every event at once, and asking per event walked the round's
    /// rule history twice for each of them once a second per admin watching.
    /// </remarks>
    public Dictionary<string, int> GetOccurrenceCounts() // Starlight
    {
        var counts = new Dictionary<string, int>();
        foreach (var (_, ruleId) in GameTicker.AllPreviousGameRules)
        {
            counts.TryGetValue(ruleId, out var seen);
            counts[ruleId] = seen + 1;
        }

        return counts;
    }

    /// <summary>
    /// The effective weight when the caller already knows the occurrence count.
    /// </summary>
    /// <remarks>
    /// The admin panel projects every event at once and would otherwise walk the round's
    /// rule history twice per event, so it counts them in one pass and passes the result in.
    /// </remarks>
    public float GetEffectiveWeight(StationEventComponent stationEvent, int occurrences) // Starlight
    {
        var falloff = Math.Clamp(_configurationManager.GetCVar(CCVars.EventsRepetitionFalloff), 0f, 1f);
        if (MathHelper.CloseTo(falloff, 1f) || occurrences <= 0)
            return stationEvent.Weight;

        return stationEvent.Weight * MathF.Pow(falloff, occurrences);
    }

    /// <summary>
    /// Counts how many times an event with the given prototype ID has run this round.
    /// </summary>
    public int GetOccurrences(string stationEvent) // Starlight
    {
        return GameTicker.AllPreviousGameRules.Count(p => p.Item2 == stationEvent);
    }

    // Starlight-start
    /// <summary>
    ///     When this event last ran, and whether it ran at all.
    /// </summary>
    /// <remarks>
    ///     <see cref="TimeSinceLastEvent"/> answers both with one TimeSpan, so an event that
    ///     never ran is indistinguishable from one that ran at round time zero.
    /// </remarks>
    public bool TryGetLastEventTime(EntityPrototype stationEvent, out TimeSpan lastRun)
    {
        foreach (var (time, rule) in GameTicker.AllPreviousGameRules.Reverse())
        {
            if (rule != stationEvent.ID)
                continue;

            lastRun = time;
            return true;
        }

        lastRun = TimeSpan.Zero;
        return false;
    }
    // Starlight-end

    /// <summary>
    /// Returns the elapsed round time when the specified event last ran, or TimeSpan.Zero if it hasn't run.
    /// </summary>
    public TimeSpan TimeSinceLastEvent(EntityPrototype stationEvent)
    {
        foreach (var (time, rule) in GameTicker.AllPreviousGameRules.Reverse())
        {
            if (rule == stationEvent.ID)
                return time;
        }

        return TimeSpan.Zero;
    }
    // Starlight-start

    /// <summary>
    ///     Whether the event can run right now, under current conditions.
    /// </summary>
    /// <remarks>
    ///     Needed because automatic events are picked minutes before they fire: between the
    ///     pick and the trigger players can leave and stop satisfying MinimumPlayers, or round
    ///     end can begin. Projecting at pick time covers round duration but not headcount,
    ///     which cannot be predicted.
    /// </remarks>
    public bool CanRunNow(string eventId)
    {
        foreach (var (proto, stationEvent) in AllEvents())
        {
            if (proto.ID != eventId)
                continue;

            return CanRun(proto, stationEvent, _playerManager.PlayerCount, GameTicker.RoundDuration());
        }

        return false;
    }
    // Starlight-end

    /// <summary>
    /// Evaluates all restrictions (active check, max occurrences, min players, earliest start, cooldown, round end) to decide if an event is allowed to run.
    /// </summary>
    private bool CanRun(EntityPrototype prototype, StationEventComponent stationEvent, int playerCount, TimeSpan currentTime)
    {
        if (GameTicker.IsGameRuleActive(prototype.ID))
            return false;

        if (stationEvent.MaxOccurrences.HasValue && GetOccurrences(prototype) >= stationEvent.MaxOccurrences.Value)
        {
            return false;
        }

        if (playerCount < stationEvent.MinimumPlayers)
        {
            return false;
        }

        if (currentTime != TimeSpan.Zero && currentTime.TotalMinutes < stationEvent.EarliestStart)
        {
            return false;
        }

        var lastRun = TimeSinceLastEvent(prototype);
        if (lastRun != TimeSpan.Zero && currentTime.TotalMinutes <
            stationEvent.ReoccurrenceDelay + lastRun.TotalMinutes)
        {
            return false;
        }

        if (_roundEnd.IsRoundEndRequested() && !stationEvent.OccursDuringRoundEnd)
        {
            return false;
        }

        return true;
    }
}
