using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.CrewManifest;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Server.Humanoid;
using Content.Server.KillTracking;
using Content.Server._NullLink.Helpers;
using Content.Server._Starlight.Humanoid;
using Content.Server.StationRecords.Components;
using Content.Server.StationRecords.Systems;
using Content.Server.Preferences.Managers;
using Content.Server.Station.Systems;
using Content.Shared.Access.Systems;
using Content.Shared.CCVar;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.CrewManifest;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Nutrition;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.StationRecords;
using Content.Shared._Nix.WebBridge;
using Content.Shared._Starlight.Achievement;
using Content.Shared._Starlight.Time;
using Content.Shared.Tag;
using Robust.Server.Player;
using Robust.Server.ServerStatus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Nix.WebBridge;

/// <summary>
/// Exposes small, read-only snapshots of in-round data for Nix web applications.
/// </summary>
/// <remarks>
/// Status host handlers run outside the game thread. Every ECS and prototype read is
/// therefore collected through <see cref="RunOnMainThread{T}"/> before responding.
/// </remarks>
public sealed partial class NixWebBridgeSystem : EntitySystem
{
    private const string BearerScheme = "Bearer";
    private const string ManifestPath = "/nix/web/v1/manifest";
    private const string RankingPath = "/nix/web/v1/ranking";
    private const string LeaderboardPath = "/nix/web/v1/leaderboard";
    private const int DefaultRankingPageSize = 25;
    private const int MaximumRankingPageSize = 100;

    // These are the drink dispensers that a bartender can use to prepare a new serving.
    private static readonly HashSet<string> DrinkDispenserPrototypes = new(StringComparer.Ordinal)
    {
        "BoozeDispenser",
        "BoozeDispenserEmpty",
        "SodaDispenser",
        "SodaDispenserEmpty",
        "CoffeeDispenser",
    };

    // Track the hostile salvage fauna and dungeon mobs that are actually used in this branch.
    private static readonly HashSet<string> SalvageCreaturePrototypes = new(StringComparer.Ordinal)
    {
        "MobBearSpaceSalvage",
        "MobBearSoviet",
        "MobGoliath",
        "MobBasilisk",
        "MobHivelord",
        "MobCarpSalvage",
        "MobCarpDungeon",
        "MobCarpRainbow",
        "MobCobraSpaceSalvage",
        "MobKangarooSpaceSalvage",
        "MobSharkSalvage",
        "MobSpiderSpaceSalvage",
        "MobTickSalvage",
        "MobWatcherLavaland",
        "MobWatcherIcewing",
        "MobWatcherMagmawing",
        "MobXeno",
        "MobXenoDrone",
        "MobXenoPraetorian",
        "MobXenoQueen",
        "MobXenoRavager",
        "MobXenoRouny",
        "MobXenoRunner",
        "MobXenoSpitter",
    };

    // A category only maps to a metric recorded by the server; clients cannot choose arbitrary database fields.
    private static readonly Dictionary<string, string> LeaderboardMetrics = new(StringComparer.OrdinalIgnoreCase)
    {
        ["chef"] = "food.served",
        ["bartender"] = "drink.served",
        ["salvage"] = "salvage.kills",
    };

    private static readonly Dictionary<string, string> TemporarySlotMap = new(StringComparer.Ordinal)
    {
        ["head"] = "HELMET",
        ["eyes"] = "EYES",
        ["ears"] = "EARS",
        ["mask"] = "MASK",
        ["outerClothing"] = "OUTERCLOTHING",
        ["jumpsuit"] = "INNERCLOTHING",
        ["neck"] = "NECK",
        ["misc"] = "NECK", // Starlight
        ["MISC"] = "NECK", // Starlight
        ["back"] = "BACKPACK",
        ["belt"] = "BELT",
        ["gloves"] = "HAND",
        ["shoes"] = "FEET",
        ["id"] = "IDCARD",
        ["pocket1"] = "POCKET1",
        ["pocket2"] = "POCKET2",
        ["suitstorage"] = "SUITSTORAGE",
    };

    private static readonly FieldInfo ClothingVisualsField = typeof(ClothingComponent).GetField("ClothingVisuals")!;
    private static readonly FieldInfo ClothingEquippedPrefixField = typeof(ClothingComponent).GetField("EquippedPrefix")!;
    private static readonly FieldInfo ClothingEquippedStateField = typeof(ClothingComponent).GetField("EquippedState")!;
    private static readonly FieldInfo ClothingRsiPathField = typeof(ClothingComponent).GetField("RsiPath")!;

    [Dependency] private IStatusHost _statusHost = default!;
    [Dependency] private IConfigurationManager _config = default!;
    [Dependency] private ITaskManager _tasks = default!;
    [Dependency] private IServerDbManager _database = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private SharedIdCardSystem _idCard = default!;
    [Dependency] private InventorySystem _inventory = default!;
    [Dependency] private IResourceManager _resourceManager = default!;
    [Dependency] private IServerPreferencesManager _preferences = default!;
    [Dependency] private GameTicker _gameTicker = default!;
    [Dependency] private CrewManifestSystem _crewManifest = default!;
    [Dependency] private HumanoidAppearanceSystem _humanoidAppearance = default!;
    [Dependency] private StationRecordsSystem _stationRecords = default!;
    [Dependency] private StationSpawningSystem _stationSpawning = default!;
    [Dependency] private StationSystem _stationSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private SharedTimeSystem _stationTime = default!;
    [Dependency] private TagSystem _tag = default!;

    private readonly Dictionary<NetUserId, NixWebCharacterIdentity> _activeCharacters = new();
    private readonly Dictionary<NetUserId, string> _activeJobs = new();
    private readonly HashSet<NetUserId> _pendingAppearanceSnapshots = new();
    private readonly Dictionary<string, HashSet<string>> _rsiStateCache = new(StringComparer.Ordinal);
    private string _apiToken = string.Empty;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        _config.OnValueChanged(CCVars.NixWebApiToken, UpdateApiToken, true);
        _statusHost.AddHandler(HandleStatusRequest);
        SubscribeNetworkEvent<NixWebAppearanceCaptureResponseEvent>(OnAppearanceCaptureResponse);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
        SubscribeLocalEvent<PlayerDetachedEvent>(OnPlayerDetached);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);
        SubscribeLocalEvent<NixWebPreparedFoodComponent, IngestedEvent>(OnPreparedFoodIngested);
        SubscribeLocalEvent<NixWebPreparedDrinkComponent, IngestedEvent>(OnPreparedDrinkIngested);
        SubscribeLocalEvent<SolutionTransferredEvent>(OnPreparedDrinkTransferred);
        SubscribeLocalEvent<KillReportedEvent>(OnKillReported);
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _config.UnsubValueChanged(CCVars.NixWebApiToken, UpdateApiToken);
    }

    private void UpdateApiToken(string token)
    {
        _apiToken = token;
    }

    private async Task<bool> HandleStatusRequest(IStatusHandlerContext context)
    {
        if (context.RequestMethod != HttpMethod.Get)
            return false;

        var path = context.Url.AbsolutePath;

        if (path == ManifestPath)
        {
            if (!await CheckAccess(context))
                return true;

            var manifest = await RunOnMainThread(BuildManifest).ConfigureAwait(false);
            await context.RespondJsonAsync(manifest).ConfigureAwait(false);
            return true;
        }

        var page = ReadQueryInteger(context.Url, "page", 1, minimum: 1, maximum: int.MaxValue);
        var pageSize = ReadQueryInteger(context.Url, "pageSize", DefaultRankingPageSize, minimum: 1, maximum: MaximumRankingPageSize);
        var offset = checked((page - 1) * pageSize);

        if (path == LeaderboardPath)
        {
            if (!await CheckAccess(context))
                return true;

            var category = ReadQueryString(context.Url, "category");
            if (category == null || !LeaderboardMetrics.TryGetValue(category, out var metricId))
                return false;

            string? selectedType = null;
            if (string.Equals(category, "salvage", StringComparison.OrdinalIgnoreCase))
            {
                selectedType = ReadQueryString(context.Url, "type");
                if (!string.IsNullOrWhiteSpace(selectedType))
                {
                    if (!SalvageCreaturePrototypes.Contains(selectedType))
                    {
                        await context.RespondAsync(
                            "Unknown salvage creature type",
                            HttpStatusCode.BadRequest);
                        return true;
                    }

                    metricId = $"salvage.kills.{selectedType}";
                }
            }

            var leaderboardRanking = await _database.GetNixWebMetricRankingAsync(metricId, offset, pageSize).ConfigureAwait(false);
            var leaderboardProfiles = await _database.GetNixWebProfilesAsync(
                leaderboardRanking.Entries.Select(entry => entry.ProfileId)).ConfigureAwait(false);
            var leaderboardResponse = await RunOnMainThread(() =>
                BuildMetricRankingResponse(category, metricId, selectedType, leaderboardRanking, leaderboardProfiles, page, pageSize)).ConfigureAwait(false);
            await context.RespondJsonAsync(leaderboardResponse).ConfigureAwait(false);
            return true;
        }

        if (path != RankingPath)
            return false;

        if (!await CheckAccess(context))
            return true;

        var ranking = await _database.GetNixWebRankingAsync(offset, pageSize).ConfigureAwait(false);
        var rankingProfiles = await _database.GetNixWebProfilesAsync(
            ranking.Entries.Select(entry => entry.ProfileId)).ConfigureAwait(false);
        var response = await RunOnMainThread(() => BuildRankingResponse(ranking, rankingProfiles, page, pageSize)).ConfigureAwait(false);
        await context.RespondJsonAsync(response).ConfigureAwait(false);
        return true;
    }

    private async Task<bool> CheckAccess(IStatusHandlerContext context)
    {
        if (string.IsNullOrWhiteSpace(_apiToken))
            return true;

        if (!context.RequestHeaders.TryGetValue("Authorization", out var authToken))
        {
            await context.RespondAsync(
                "Authorization is required",
                HttpStatusCode.Unauthorized);
            return false;
        }

        var authHeaderValue = authToken.ToString();
        var spaceIndex = authHeaderValue.IndexOf(' ');
        if (spaceIndex == -1)
        {
            await context.RespondAsync(
                "Invalid Authorization header value",
                HttpStatusCode.BadRequest);
            return false;
        }

        var authScheme = authHeaderValue[..spaceIndex];
        var authValue = authHeaderValue[(spaceIndex + 1)..].Trim();
        if (!string.Equals(authScheme, BearerScheme, StringComparison.Ordinal))
        {
            await context.RespondAsync(
                "Invalid Authorization scheme",
                HttpStatusCode.BadRequest);
            return false;
        }

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(authValue),
                Encoding.UTF8.GetBytes(_apiToken)))
        {
            await context.RespondAsync(
                "Authorization is invalid",
                HttpStatusCode.Unauthorized);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Called only after NullLink confirms an achievement unlock. NullLink remains the source of truth.
    /// </summary>
    public void TrackAchievementUnlocked(ICommonSession session, string achievementId)
    {
        if (!TryGetTrackedCharacter(session, out var character))
            return;

        RequestLiveAppearanceSnapshot(session, character);
        _database.RecordNixWebAchievementAsync(character, achievementId, _gameTicker.RoundId).FireAndForget();
    }

    /// <summary>
    /// Records a completed recipe and labels its resulting food so a later real consumption can count as service.
    /// </summary>
    public void TrackCookedFood(EntityUid? cook, EntityUid food, string recipeId)
    {
        if (cook == null
            || !_playerManager.TryGetSessionByEntity(cook.Value, out var session)
            || !TryGetTrackedCharacter(session, out var character)
            || !HasJob(session, "Chef"))
        {
            return;
        }

        var prepared = EnsureComp<NixWebPreparedFoodComponent>(food);
        prepared.OwnerUserId = character.UserId;
        prepared.ProfileSlot = character.ProfileSlot;
        prepared.CharacterName = character.CharacterName;
        prepared.Species = character.Species;
        prepared.AppearanceJson = character.AppearanceJson;
        prepared.RecipeId = recipeId;

        RequestLiveAppearanceSnapshot(session, character);
        _database.RecordNixWebStatisticAsync(character, "food.cooked", 1, _gameTicker.RoundId, recipeId).FireAndForget();
    }

    /// <summary>
    /// Called by the native dispenser only after it actually adds a reagent to the output container.
    /// </summary>
    public void TrackDrinkDispensed(EntityUid bartender, EntityUid dispenser, EntityUid drink)
    {
        var prototypeId = MetaData(dispenser).EntityPrototype?.ID;
        if (prototypeId == null || !DrinkDispenserPrototypes.Contains(prototypeId))
            return;

        TrackDrinkPrepared(bartender, drink, prototypeId);
    }

    private void TrackDrinkPrepared(EntityUid bartender, EntityUid drink, string dispenserId)
    {
        if (!_playerManager.TryGetSessionByEntity(bartender, out var session)
            || !TryGetTrackedCharacter(session, out var character)
            || !HasJob(session, "Bartender"))
        {
            return;
        }

        var prepared = EnsureComp<NixWebPreparedDrinkComponent>(drink);
        if (!prepared.ServiceRecorded
            && prepared.OwnerUserId == character.UserId
            && prepared.ProfileSlot == character.ProfileSlot)
        {
            return;
        }

        prepared.OwnerUserId = character.UserId;
        prepared.ProfileSlot = character.ProfileSlot;
        prepared.CharacterName = character.CharacterName;
        prepared.Species = character.Species;
        prepared.AppearanceJson = character.AppearanceJson;
        prepared.DispenserId = dispenserId;
        prepared.ServiceRecorded = false;

        RequestLiveAppearanceSnapshot(session, character);
        _database.RecordNixWebStatisticAsync(character, "drink.prepared", 1, _gameTicker.RoundId, dispenserId).FireAndForget();
    }

    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent args)
    {
        if (args.ProfileSlot < 0)
        {
            return;
        }

        _activeCharacters[args.Player.UserId] = CreateCharacterIdentity(args.Player.UserId, args.ProfileSlot, args.Profile, args.Mob);
        if (string.IsNullOrWhiteSpace(args.JobId))
            _activeJobs.Remove(args.Player.UserId);
        else
            _activeJobs[args.Player.UserId] = args.JobId;
    }

    private void OnPlayerDetached(PlayerDetachedEvent args)
    {
        _activeCharacters.Remove(args.Player.UserId);
        _activeJobs.Remove(args.Player.UserId);
        _pendingAppearanceSnapshots.Remove(args.Player.UserId);
    }

    private void OnRoundRestartCleanup(RoundRestartCleanupEvent _)
    {
        _activeCharacters.Clear();
        _activeJobs.Clear();
        _pendingAppearanceSnapshots.Clear();
    }

    private void OnPreparedFoodIngested(Entity<NixWebPreparedFoodComponent> food, ref IngestedEvent args)
    {
        if (food.Comp.ServiceRecorded
            || !_playerManager.TryGetSessionByEntity(args.Target, out var eater)
            || eater.UserId.UserId == food.Comp.OwnerUserId)
        {
            return;
        }

        food.Comp.ServiceRecorded = true;
        var cook = new NixWebCharacterIdentity(
            food.Comp.OwnerUserId,
            food.Comp.ProfileSlot,
            food.Comp.CharacterName,
            food.Comp.Species,
            food.Comp.AppearanceJson);
        _database.RecordNixWebStatisticAsync(cook, "food.served", 1, _gameTicker.RoundId, food.Comp.RecipeId).FireAndForget();
    }

    private void OnPreparedDrinkIngested(Entity<NixWebPreparedDrinkComponent> drink, ref IngestedEvent args)
    {
        if (drink.Comp.ServiceRecorded
            || !_playerManager.TryGetSessionByEntity(args.Target, out var drinker)
            || drinker.UserId.UserId == drink.Comp.OwnerUserId)
        {
            return;
        }

        drink.Comp.ServiceRecorded = true;
        var bartender = new NixWebCharacterIdentity(
            drink.Comp.OwnerUserId,
            drink.Comp.ProfileSlot,
            drink.Comp.CharacterName,
            drink.Comp.Species,
            drink.Comp.AppearanceJson);
        _database.RecordNixWebStatisticAsync(bartender, "drink.served", 1, _gameTicker.RoundId, drink.Comp.DispenserId).FireAndForget();
    }

    private void OnPreparedDrinkTransferred(ref SolutionTransferredEvent args)
    {
        if (args.From == args.To)
        {
            return;
        }

        if (!TryComp(args.From, out NixWebPreparedDrinkComponent? source))
        {
            // Bartenders commonly build drinks by pouring purchased bottles into a glass.
            // Those bottles have no bridge component yet, so establish attribution at the first pour.
            if (_tag.HasTag(args.From, "DrinkBottle"))
            {
                var sourceId = MetaData(args.From).EntityPrototype?.ID ?? "bottled-drink";
                TrackDrinkPrepared(args.User, args.To, sourceId);
            }

            return;
        }

        var target = EnsureComp<NixWebPreparedDrinkComponent>(args.To);
        target.OwnerUserId = source.OwnerUserId;
        target.ProfileSlot = source.ProfileSlot;
        target.CharacterName = source.CharacterName;
        target.Species = source.Species;
        target.AppearanceJson = source.AppearanceJson;
        target.DispenserId = source.DispenserId;
        target.ServiceRecorded = source.ServiceRecorded;
    }

    private void OnKillReported(ref KillReportedEvent args)
    {
        if (args.Suicide
            || MetaData(args.Entity).EntityPrototype?.ID is not { } prototypeId
            || !SalvageCreaturePrototypes.Contains(prototypeId))
        {
            return;
        }

        if (args.Primary is KillPlayerSource killer
            && _playerManager.TryGetSessionById(killer.PlayerId, out var killerSession))
        {
            TrackSalvageCombat(killerSession, prototypeId, assist: false);
        }

        if (args.Assist is KillPlayerSource assistant
            && _playerManager.TryGetSessionById(assistant.PlayerId, out var assistantSession))
        {
            TrackSalvageCombat(assistantSession, prototypeId, assist: true);
        }
    }

    private void TrackSalvageCombat(ICommonSession session, string creatureId, bool assist)
    {
        if (!TryGetTrackedCharacter(session, out var character))
            return;

        RequestLiveAppearanceSnapshot(session, character);
        var metric = assist ? "salvage.assists" : "salvage.kills";
        _database.RecordNixWebStatisticAsync(character, metric, 1, _gameTicker.RoundId, creatureId).FireAndForget();
        _database.RecordNixWebStatisticAsync(character, $"{metric}.{creatureId}", 1, _gameTicker.RoundId).FireAndForget();
    }

    private bool HasJob(ICommonSession session, string requiredJobId)
    {
        if (_activeJobs.TryGetValue(session.UserId, out var activeJob)
            && string.Equals(activeJob, requiredJobId, StringComparison.Ordinal))
        {
            return true;
        }

        if (!_activeCharacters.TryGetValue(session.UserId, out var character)
            || session.AttachedEntity is not { } entity
            || !_inventory.TryGetSlotEntity(entity, "id", out var idSlot)
            || !_idCard.TryGetIdCard(idSlot.Value, out var idCard)
            || !TryComp(idCard, out StationRecordKeyStorageComponent? recordKey)
            || recordKey.Key is not { } key
            || !_stationRecords.TryGetRecord<GeneralStationRecord>(key, out var record))
        {
            return false;
        }

        return record.Name == character.CharacterName && record.JobPrototype == requiredJobId;
    }

    private bool TryGetTrackedCharacter(ICommonSession session, out NixWebCharacterIdentity character)
    {
        if (!_activeCharacters.TryGetValue(session.UserId, out var trackedCharacter))
        {
            character = default!;
            return false;
        }

        character = trackedCharacter;

        if (session.AttachedEntity is not { Valid: true } entity
            || !TryComp(entity, out HumanoidAppearanceComponent? humanoid)
            || humanoid.BaseProfile == null)
        {
            return true;
        }

        var refreshedCharacter = CreateCharacterIdentity(session.UserId, character.ProfileSlot, humanoid.BaseProfile, entity, humanoid);
        character = MergeCharacterIdentity(character, refreshedCharacter);
        _activeCharacters[session.UserId] = character;
        return true;
    }

    private void RequestLiveAppearanceSnapshot(ICommonSession session, NixWebCharacterIdentity character)
    {
        if (_pendingAppearanceSnapshots.Contains(session.UserId)
            || session.AttachedEntity is not { Valid: true } entity)
        {
            return;
        }

        _pendingAppearanceSnapshots.Add(session.UserId);
        RaiseNetworkEvent(new NixWebAppearanceCaptureRequestEvent
        {
            Entity = GetNetEntity(entity),
            ProfileSlot = character.ProfileSlot,
            CharacterName = character.CharacterName,
            Species = character.Species,
        }, session.Channel);
    }

    private void OnAppearanceCaptureResponse(NixWebAppearanceCaptureResponseEvent ev, EntitySessionEventArgs args)
    {
        _pendingAppearanceSnapshots.Remove(args.SenderSession.UserId);

        if (args.SenderSession.AttachedEntity is not { Valid: true } attachedEntity
            || GetNetEntity(attachedEntity) != ev.Entity
            || ev.Appearance == null
            || !_activeCharacters.TryGetValue(args.SenderSession.UserId, out var trackedCharacter)
            || trackedCharacter.ProfileSlot != ev.ProfileSlot
            || !string.Equals(trackedCharacter.Species, ev.Species, StringComparison.Ordinal)
            || !string.Equals(trackedCharacter.CharacterName, ev.CharacterName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var rawJson = JsonSerializer.Serialize(ev.Appearance);
        if (!NixWebBridgeAppearanceJson.TryNormalize(rawJson, out var normalizedAppearanceJson))
            return;

        var updatedCharacter = MergeCharacterIdentity(
            trackedCharacter,
            trackedCharacter with { AppearanceJson = normalizedAppearanceJson });
        _activeCharacters[args.SenderSession.UserId] = updatedCharacter;
        _database.UpsertNixWebAppearanceAsync(updatedCharacter).FireAndForget();
    }

    private NixWebCharacterIdentity CreateCharacterIdentity(NetUserId userId, int profileSlot, HumanoidCharacterProfile profile, EntityUid? liveEntity = null, HumanoidAppearanceComponent? humanoid = null)
    {
        return new NixWebCharacterIdentity(
            userId.UserId,
            profileSlot,
            profile.Name,
            profile.Species,
            SerializeCharacterAppearance(profile, liveEntity, humanoid));
    }

    private string SerializeCharacterAppearance(HumanoidCharacterProfile profile, EntityUid? liveEntity = null, HumanoidAppearanceComponent? humanoid = null)
    {
        var appearance = profile.Appearance;
        humanoid ??= liveEntity is { } entity && TryComp(entity, out HumanoidAppearanceComponent? liveHumanoid)
            ? liveHumanoid
            : null;
        return SerializeCharacterAppearance(
            profile,
            humanoid?.Sex ?? profile.Sex,
            appearance.HairStyleId,
            appearance.HairColor.ToHex(),
            appearance.FacialHairStyleId,
            appearance.FacialHairColor.ToHex(),
            (humanoid?.EyeColor ?? appearance.EyeColor).ToHex(),
            (humanoid?.SkinColor ?? appearance.SkinColor).ToHex(),
            humanoid != null
                ? humanoid.MarkingSet.GetForwardEnumerator().Select(marking => marking.ToDBString()).ToList()
                : appearance.Markings.ConvertAll(marking => marking.ToDBString()),
            humanoid?.Width ?? appearance.Width,
            humanoid?.Height ?? appearance.Height,
            BuildPortraitLayers(profile, liveEntity, humanoid));
    }

    private static string SerializeCharacterAppearance(
        HumanoidCharacterProfile profile,
        Sex sex,
        string hairStyleId,
        string hairColor,
        string facialHairStyleId,
        string facialHairColor,
        string eyeColor,
        string skinColor,
        IReadOnlyList<string> markings,
        float width,
        float height,
        IReadOnlyList<NixWebPortraitLayer> portraitLayers)
    {
        var snapshot = new NixWebCharacterAppearance(
            sex.ToString(),
            hairStyleId,
            hairColor,
            facialHairStyleId,
            facialHairColor,
            eyeColor,
            skinColor,
            markings.ToList(),
            width,
            height,
            portraitLayers.ToList());

        return JsonSerializer.Serialize(snapshot);
    }

    private static NixWebCharacterIdentity MergeCharacterIdentity(
        NixWebCharacterIdentity current,
        NixWebCharacterIdentity incoming)
    {
        return incoming with
        {
            AppearanceJson = NixWebBridgeAppearanceJson.SelectPreferred(current.AppearanceJson, incoming.AppearanceJson),
        };
    }

    /// <summary>
    /// Resolves the same SSI layers used by the humanoid client for a compact public portrait.
    /// The website only receives paths and state names; it never receives a player's account data.
    /// </summary>
    private List<NixWebPortraitLayer> BuildPortraitLayers(HumanoidCharacterProfile profile, EntityUid? liveEntity = null, HumanoidAppearanceComponent? humanoid = null)
    {
        var result = new List<NixWebPortraitLayer>();
        var speciesId = humanoid?.Species ?? profile.Species;
        var species = _prototypeManager.Index<SpeciesPrototype>(speciesId);
        var baseSprites = _prototypeManager.Index<HumanoidSpeciesBaseSpritesPrototype>(species.SpriteSet);
        var appearance = profile.Appearance;
        var sex = humanoid?.Sex ?? profile.Sex;
        var skinColor = humanoid?.SkinColor ?? appearance.SkinColor;
        var eyeColor = humanoid?.EyeColor ?? appearance.EyeColor;

        var baseLayerOrder = new[]
        {
            HumanoidVisualLayers.RFoot,
            HumanoidVisualLayers.LFoot,
            HumanoidVisualLayers.RLeg,
            HumanoidVisualLayers.LLeg,
            HumanoidVisualLayers.RArm,
            HumanoidVisualLayers.LArm,
            HumanoidVisualLayers.Chest,
            HumanoidVisualLayers.Head,
            HumanoidVisualLayers.Eyes,
        };

        foreach (var layer in baseLayerOrder)
        {
            if (!IsPortraitLayerVisible(layer, humanoid))
                continue;

            if (!baseSprites.Sprites.TryGetValue(layer, out var spriteId))
                continue;

            spriteId = HumanoidVisualLayersExtension.GetSexMorph(layer, sex, spriteId);
            if (!_prototypeManager.TryIndex<HumanoidSpeciesSpriteLayer>(spriteId, out var sprite)
                || sprite.BaseSprite is not SpriteSpecifier.Rsi rsi)
            {
                continue;
            }

            var color = sprite.MatchSkin
                ? skinColor.WithAlpha(sprite.LayerAlpha).ToHex()
                : layer == HumanoidVisualLayers.Eyes
                    ? eyeColor.ToHex()
                    : "#FFFFFF";
            result.Add(new NixWebPortraitLayer(NormalizeRsiPath(rsi), rsi.RsiState, color));
        }

        var markings = humanoid != null
            ? humanoid.MarkingSet.GetForwardEnumerator().ToList()
            : new List<Marking>(appearance.Markings);

        if (humanoid == null)
        {
            markings.Add(new Marking(appearance.HairStyleId, new[] { appearance.HairColor }, appearance.HairGlowing));
            markings.Add(new Marking(appearance.FacialHairStyleId, new[] { appearance.FacialHairColor }, appearance.FacialHairGlowing));
        }

        foreach (var marking in markings)
        {
            if (!_prototypeManager.TryIndex<MarkingPrototype>(marking.MarkingId, out var prototype)
                || !IsPortraitLayer(prototype.BodyPart)
                || !IsPortraitLayerVisible(prototype.BodyPart, humanoid))
            {
                continue;
            }

            for (var index = 0; index < prototype.Sprites.Count; index++)
            {
                if (prototype.Sprites[index] is not SpriteSpecifier.Rsi rsi)
                    continue;

                var colorIndex = prototype.GetColorIndex(index);
                var color = colorIndex < marking.MarkingColors.Count
                    ? marking.MarkingColors[colorIndex].ToHex()
                    : "#FFFFFF";
                result.Add(new NixWebPortraitLayer(NormalizeRsiPath(rsi), rsi.RsiState, color));
            }
        }

        if (liveEntity != null && humanoid != null)
        {
            AppendEquippedPortraitLayers(result, liveEntity.Value, humanoid, speciesId);
        }

        return result;
    }

    private void AppendEquippedPortraitLayers(
        List<NixWebPortraitLayer> result,
        EntityUid entity,
        HumanoidAppearanceComponent humanoid,
        string speciesId)
    {
        if (!TryComp(entity, out InventoryComponent? inventory))
            return;

        var enumerator = _inventory.GetSlotEnumerator((entity, inventory));
        while (enumerator.NextItem(out var item, out var slot))
        {
            if (!TryComp(item, out ClothingComponent? clothing)
                || (clothing.Slots & slot.SlotFlags) == 0
                || !TryGetClothingPortraitLayers(clothing, slot.Name, speciesId, out var layers))
            {
                continue;
            }

            if (layers == null)
                continue;

            foreach (var layer in layers)
            {
                if (layer.Visible == false
                    || string.IsNullOrWhiteSpace(layer.RsiPath)
                    || string.IsNullOrWhiteSpace(layer.State))
                {
                    continue;
                }

                var rsiPath = ToTextureRsiPath(layer.RsiPath);
                var color = layer.Color?.ToHex() ?? "#FFFFFF";
                result.Add(new NixWebPortraitLayer(NormalizeRsiPath(rsiPath), layer.State, color));
            }
        }
    }

    private bool TryGetClothingPortraitLayers(
        ClothingComponent clothing,
        string slot,
        string speciesId,
        out List<PrototypeLayerData>? layers)
    {
        layers = null;
        var clothingVisuals = GetClothingVisuals(clothing);
        if (clothingVisuals == null)
        {
            return TryGetDefaultClothingPortraitLayers(clothing, slot, speciesId, out layers);
        }

        if (!string.IsNullOrWhiteSpace(speciesId)
            && clothingVisuals.TryGetValue($"{slot}-{speciesId}", out layers))
        {
            return true;
        }

        if (clothingVisuals.TryGetValue(slot, out layers))
        {
            return true;
        }

        return TryGetDefaultClothingPortraitLayers(clothing, slot, speciesId, out layers);
    }

    private bool TryGetDefaultClothingPortraitLayers(
        ClothingComponent clothing,
        string slot,
        string speciesId,
        out List<PrototypeLayerData>? layers)
    {
        layers = null;

        var clothingRsiPath = GetClothingRsiPath(clothing);
        if (string.IsNullOrWhiteSpace(clothingRsiPath))
            return false;

        var rsiPath = ToTextureRsiPath(clothingRsiPath);
        var correctedSlot = TemporarySlotMap.TryGetValue(slot, out var mappedSlot)
            ? mappedSlot
            : slot;

        var state = $"equipped-{correctedSlot}";
        var equippedPrefix = GetClothingEquippedPrefix(clothing);
        var equippedState = GetClothingEquippedState(clothing);

        if (!string.IsNullOrWhiteSpace(equippedPrefix))
            state = $"{equippedPrefix}-equipped-{correctedSlot}";

        if (!string.IsNullOrWhiteSpace(equippedState))
            state = equippedState;

        if (!string.IsNullOrWhiteSpace(speciesId) && RsiHasState(rsiPath, $"{state}-{speciesId}"))
            state = $"{state}-{speciesId}";
        else if (!RsiHasState(rsiPath, state))
            return false;

        layers = new List<PrototypeLayerData>
        {
            new()
            {
                RsiPath = rsiPath.ToString(),
                State = state,
                Scale = clothing.Scale,
            }
        };

        return true;
    }

    private bool RsiHasState(ResPath rsiPath, string state)
    {
        var key = rsiPath.ToString();
        if (!_rsiStateCache.TryGetValue(key, out var states))
        {
            states = LoadRsiStates(rsiPath);
            _rsiStateCache[key] = states;
        }

        return states.Contains(state);
    }

    private HashSet<string> LoadRsiStates(ResPath rsiPath)
    {
        var states = new HashSet<string>(StringComparer.Ordinal);
        if (!_resourceManager.TryContentFileRead(rsiPath / "meta.json", out var stream))
            return states;

        try
        {
            using (stream)
            {
                using var document = JsonDocument.Parse(stream);
                if (!document.RootElement.TryGetProperty("states", out var stateList)
                    || stateList.ValueKind != JsonValueKind.Array)
                {
                    return states;
                }

                foreach (var stateElement in stateList.EnumerateArray())
                {
                    if (!stateElement.TryGetProperty("name", out var nameElement)
                        || nameElement.ValueKind != JsonValueKind.String)
                    {
                        continue;
                    }

                    var stateName = nameElement.GetString();
                    if (!string.IsNullOrWhiteSpace(stateName))
                    {
                        states.Add(stateName);
                    }
                }
            }
        }
        catch (JsonException)
        {
            return states;
        }

        return states;
    }

    private static bool IsPortraitLayer(HumanoidVisualLayers layer)
        => layer is HumanoidVisualLayers.Special
            or HumanoidVisualLayers.Hair
            or HumanoidVisualLayers.FacialHair
            or HumanoidVisualLayers.Head
            or HumanoidVisualLayers.Snout
            or HumanoidVisualLayers.SnoutCover
            or HumanoidVisualLayers.HeadSide
            or HumanoidVisualLayers.HeadTop
            or HumanoidVisualLayers.Chest
            or HumanoidVisualLayers.Eyes
            or HumanoidVisualLayers.RArm
            or HumanoidVisualLayers.LArm
            or HumanoidVisualLayers.RLeg
            or HumanoidVisualLayers.LLeg;

    private static bool IsPortraitLayerVisible(HumanoidVisualLayers layer, HumanoidAppearanceComponent? humanoid)
        => humanoid == null
            || (!humanoid.PermanentlyHidden.Contains(layer)
                && !humanoid.HiddenLayers.ContainsKey(layer));

    private static ResPath ToTextureRsiPath(string rawPath)
    {
        var normalized = rawPath.Replace('\\', '/').Trim();
        if (normalized.StartsWith("/Textures/", StringComparison.Ordinal))
            return new ResPath(normalized);

        if (normalized.StartsWith("Textures/", StringComparison.Ordinal))
            return new ResPath($"/{normalized}");

        if (normalized.StartsWith("/", StringComparison.Ordinal))
            return new ResPath($"/Textures{normalized}");

        return new ResPath($"/Textures/{normalized}");
    }

    private static string NormalizeRsiPath(SpriteSpecifier.Rsi rsi)
        => rsi.RsiPath.ToString().Replace("/Textures/", string.Empty, StringComparison.Ordinal).TrimStart('/');

    private static string NormalizeRsiPath(ResPath rsiPath)
        => rsiPath.ToString().Replace("/Textures/", string.Empty, StringComparison.Ordinal).TrimStart('/');

    private static Dictionary<string, List<PrototypeLayerData>>? GetClothingVisuals(ClothingComponent clothing)
        => ClothingVisualsField.GetValue(clothing) as Dictionary<string, List<PrototypeLayerData>>;

    private static string? GetClothingEquippedPrefix(ClothingComponent clothing)
        => ClothingEquippedPrefixField.GetValue(clothing) as string;

    private static string? GetClothingEquippedState(ClothingComponent clothing)
        => ClothingEquippedStateField.GetValue(clothing) as string;

    private static string? GetClothingRsiPath(ClothingComponent clothing)
        => ClothingRsiPathField.GetValue(clothing) as string;

    private static int ReadQueryInteger(Uri url, string key, int defaultValue, int minimum, int maximum)
    {
        foreach (var pair in url.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0 || !string.Equals(pair[..separator], key, StringComparison.OrdinalIgnoreCase))
                continue;

            if (int.TryParse(Uri.UnescapeDataString(pair[(separator + 1)..]), NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                return Math.Clamp(value, minimum, maximum);
        }

        return defaultValue;
    }

    private static string? ReadQueryString(Uri url, string key)
    {
        foreach (var pair in url.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            if (separator <= 0 || !string.Equals(pair[..separator], key, StringComparison.OrdinalIgnoreCase))
                continue;

            return Uri.UnescapeDataString(pair[(separator + 1)..]);
        }

        return null;
    }

    private NixWebManifestResponse BuildManifest()
    {
        var departments = new List<DepartmentPrototype>();
        foreach (var department in _prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
        {
            departments.Add(department);
        }

        departments.Sort(DepartmentUIComparer.Instance);

        var stations = new List<NixWebStationManifest>();
        foreach (var station in _stationSystem.GetStations())
        {
            var (_, manifest) = _crewManifest.GetCrewManifest(station);
            stations.Add(CreateStationManifest(MetaData(station).EntityName, manifest?.Entries ?? Array.Empty<CrewManifestEntry>(), departments));
        }

        stations.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));
        var inRound = _gameTicker.RunLevel == GameRunLevel.InRound;
        var stationTime = _stationTime.GetStationTime();
        var roundDuration = inRound ? _stationTime.GetShiftDuration() : TimeSpan.Zero;
        return new NixWebManifestResponse(
            DateTimeOffset.UtcNow,
            stations,
            (long) roundDuration.TotalSeconds,
            (int) stationTime.Time.TotalSeconds,
            stationTime.Date);
    }

    private NixWebRankingResponse BuildRankingResponse(
        NixWebRankingPage ranking,
        IReadOnlyDictionary<int, HumanoidCharacterProfile> profiles,
        int page,
        int pageSize)
    {
        var entries = new List<NixWebRankingEntry>(ranking.Entries.Count);
        foreach (var entry in ranking.Entries)
        {
            var achievements = new List<NixWebAchievement>(entry.Achievements.Count);
            foreach (var achievement in entry.Achievements)
            {
                var title = achievement.AchievementId;
                if (_prototypeManager.TryIndex<AchievementPrototype>(achievement.AchievementId, out var prototype))
                    title = Loc.GetString(prototype.Name);

                achievements.Add(new NixWebAchievement(achievement.AchievementId, title, achievement.AwardedAt));
            }

            entries.Add(new NixWebRankingEntry(
                entry.ProfileId,
                entry.CharacterName,
                entry.Species,
                ResolveAppearanceJsonForResponse(entry.ProfileId, entry.CharacterName, entry.Species, entry.AppearanceJson, profiles),
                entry.AchievementCount,
                entry.MealsCooked,
                entry.MealsServed,
                entry.DrinksPrepared,
                entry.DrinksServed,
                entry.SalvageKills,
                entry.SalvageAssists,
                achievements));
        }

        return new NixWebRankingResponse(DateTimeOffset.UtcNow, page, pageSize, ranking.Total, entries);
    }

    private NixWebMetricRankingResponse BuildMetricRankingResponse(
        string category,
        string metricId,
        string? selectedType,
        NixWebMetricRankingPage ranking,
        IReadOnlyDictionary<int, HumanoidCharacterProfile> profiles,
        int page,
        int pageSize)
    {
        var entries = new List<NixWebMetricRankingEntry>(ranking.Entries.Count);
        foreach (var entry in ranking.Entries)
        {
            entries.Add(new NixWebMetricRankingEntry(
                entry.ProfileId,
                entry.CharacterName,
                entry.Species,
                ResolveAppearanceJsonForResponse(
                    entry.ProfileId,
                    entry.CharacterName,
                    entry.Species,
                    entry.AppearanceJson,
                    profiles,
                    ResolveMetricPreviewJobId(category)),
                entry.Score));
        }

        IReadOnlyList<NixWebMetricRankingType> availableTypes = Array.Empty<NixWebMetricRankingType>();
        if (string.Equals(category, "salvage", StringComparison.OrdinalIgnoreCase))
        {
            availableTypes = SalvageCreaturePrototypes
                .Select(id => new NixWebMetricRankingType(id, ResolveMetricTypeName(id)))
                .OrderBy(type => type.Name, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(type => type.Id, StringComparer.Ordinal)
                .ToArray();
        }

        return new NixWebMetricRankingResponse(
            DateTimeOffset.UtcNow,
            category,
            metricId,
            selectedType,
            page,
            pageSize,
            ranking.Total,
            availableTypes,
            entries);
    }

    private string ResolveMetricTypeName(string prototypeId)
    {
        if (!_prototypeManager.TryIndex<EntityPrototype>(prototypeId, out var prototype))
            return prototypeId;

        return Loc.GetString(prototype.Name);
    }

    private string ResolveAppearanceJsonForResponse(
        int profileId,
        string characterName,
        string species,
        string storedAppearanceJson,
        IReadOnlyDictionary<int, HumanoidCharacterProfile> profiles,
        string? previewJobId = null)
    {
        if (TryGetLiveAppearanceJson(characterName, species, out var liveAppearanceJson))
            return liveAppearanceJson;

        if (!string.IsNullOrWhiteSpace(previewJobId)
            && profiles.TryGetValue(profileId, out var profile)
            && TryBuildOfflineAppearanceJson(profile, previewJobId, out var rebuiltAppearanceJson))
        {
            return rebuiltAppearanceJson;
        }

        return storedAppearanceJson;
    }

    private bool TryGetLiveAppearanceJson(string characterName, string species, out string appearanceJson)
    {
        foreach (var session in _playerManager.Sessions)
        {
            if (!_activeCharacters.TryGetValue(session.UserId, out var trackedCharacter)
                || !string.Equals(trackedCharacter.CharacterName, characterName, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(trackedCharacter.Species, species, StringComparison.Ordinal))
            {
                continue;
            }

            if (!TryGetTrackedCharacter(session, out var refreshedCharacter)
                || !string.Equals(refreshedCharacter.CharacterName, characterName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            appearanceJson = refreshedCharacter.AppearanceJson;
            return true;
        }

        appearanceJson = string.Empty;
        return false;
    }

    private static string? ResolveMetricPreviewJobId(string category)
    {
        if (string.Equals(category, "chef", StringComparison.OrdinalIgnoreCase))
            return "Chef";

        if (string.Equals(category, "bartender", StringComparison.OrdinalIgnoreCase))
            return "Bartender";

        if (string.Equals(category, "salvage", StringComparison.OrdinalIgnoreCase))
            return "SalvageSpecialist";

        return null;
    }

    private bool TryBuildOfflineAppearanceJson(
        HumanoidCharacterProfile profile,
        string previewJobId,
        out string appearanceJson)
    {
        appearanceJson = string.Empty;

        if (!_prototypeManager.TryIndex<JobPrototype>(previewJobId, out var job))
            return false;

        var species = _prototypeManager.Index<SpeciesPrototype>(profile.Species);
        var dummy = Spawn(species.DollPrototype, MapCoordinates.Nullspace);

        try
        {
            _humanoidAppearance.LoadProfile(dummy, profile);

            if (species.Loadout != null)
            {
                var speciesLoadout = profile.GetSpeciesLoadoutOrDefault(null, _prototypeManager);
                GivePreviewLoadout(dummy, speciesLoadout);
            }

            GivePreviewJobClothes(dummy, profile, job);

            var jobLoadoutId = LoadoutSystem.GetJobPrototype(job.ID);
            if (_prototypeManager.HasIndex<RoleLoadoutPrototype>(jobLoadoutId))
            {
                var loadout = profile.GetLoadoutOrDefault(jobLoadoutId, null, profile.Species, EntityManager, _prototypeManager);
                GivePreviewLoadout(dummy, loadout);
            }

            if (!TryComp(dummy, out HumanoidAppearanceComponent? humanoid))
                return TryBuildPrototypeAppearanceJson(profile, previewJobId, out appearanceJson);

            appearanceJson = SerializeCharacterAppearance(profile, dummy, humanoid);
            var dummyAppearance = JsonSerializer.Deserialize<NixWebCharacterAppearance>(appearanceJson);
            if (dummyAppearance?.PortraitLayers.Count > BuildPortraitLayers(profile).Count)
                return true;
        }
        finally
        {
            Del(dummy);
        }

        return TryBuildPrototypeAppearanceJson(profile, previewJobId, out appearanceJson);
    }

    private void GivePreviewJobClothes(EntityUid dummy, HumanoidCharacterProfile profile, JobPrototype job)
    {
        if (!_inventory.TryGetSlots(dummy, out var slots))
            return;

        if (profile.Loadouts.TryGetValue(job.ID, out var jobLoadout))
        {
            foreach (var loadouts in jobLoadout.SelectedLoadouts.Values)
            {
                foreach (var loadout in loadouts)
                {
                    if (!_prototypeManager.TryIndex(loadout.Prototype, out LoadoutPrototype? loadoutProto))
                        continue;

                    foreach (var slot in slots)
                    {
                        if (_prototypeManager.Resolve(loadoutProto.StartingGear, out StartingGearPrototype? loadoutGear))
                            ReplacePreviewSlot(dummy, slot.Name, ((IEquipmentLoadout) loadoutGear).GetGear(slot.Name));
                        else
                            ReplacePreviewSlot(dummy, slot.Name, ((IEquipmentLoadout) loadoutProto).GetGear(slot.Name));
                    }
                }
            }
        }

        if (!_prototypeManager.Resolve(job.StartingGear, out StartingGearPrototype? gear))
            return;

        foreach (var slot in slots)
        {
            ReplacePreviewSlot(dummy, slot.Name, ((IEquipmentLoadout) gear).GetGear(slot.Name));
        }
    }

    private void GivePreviewLoadout(EntityUid dummy, RoleLoadout? roleLoadout)
    {
        if (roleLoadout == null)
            return;

        foreach (var group in roleLoadout.SelectedLoadouts.Values)
        {
            foreach (var loadout in group)
            {
                if (!_prototypeManager.TryIndex(loadout.Prototype, out LoadoutPrototype? loadoutProto))
                    continue;

                _stationSpawning.EquipStartingGear(dummy, loadoutProto);
            }
        }
    }

    private void ReplacePreviewSlot(EntityUid dummy, string slotName, string itemType)
    {
        if (_inventory.TryUnequip(dummy, slotName, out var unequippedItem, silent: true, force: true, reparent: false))
        {
            Del(unequippedItem.Value);
        }

        if (string.IsNullOrEmpty(itemType))
            return;

        var item = Spawn(itemType, MapCoordinates.Nullspace);
        if (!_inventory.TryEquip(dummy, item, slotName, true, true))
        {
            Del(item);
        }
    }

    private bool TryBuildPrototypeAppearanceJson(
        HumanoidCharacterProfile profile,
        string previewJobId,
        out string appearanceJson)
    {
        appearanceJson = string.Empty;
        if (!_prototypeManager.TryIndex<JobPrototype>(previewJobId, out var job))
            return false;

        var portraitLayers = BuildPortraitLayers(profile);
        var baseLayerCount = portraitLayers.Count;
        AppendProfileLoadoutPortraitLayers(portraitLayers, profile, job);
        if (portraitLayers.Count <= baseLayerCount)
            return false;

        var appearance = profile.Appearance;
        appearanceJson = SerializeCharacterAppearance(
            profile,
            profile.Sex,
            appearance.HairStyleId,
            appearance.HairColor.ToHex(),
            appearance.FacialHairStyleId,
            appearance.FacialHairColor.ToHex(),
            appearance.EyeColor.ToHex(),
            appearance.SkinColor.ToHex(),
            appearance.Markings.ConvertAll(marking => marking.ToDBString()),
            appearance.Width,
            appearance.Height,
            portraitLayers);
        return true;
    }

    private void AppendProfileLoadoutPortraitLayers(
        List<NixWebPortraitLayer> portraitLayers,
        HumanoidCharacterProfile profile,
        JobPrototype job)
    {
        var equipmentBySlot = new Dictionary<string, string>(StringComparer.Ordinal);

        ApplyRoleLoadoutEquipment(equipmentBySlot, profile.GetSpeciesLoadoutOrDefault(null, _prototypeManager));

        if (_prototypeManager.Resolve(job.StartingGear, out StartingGearPrototype? jobStartingGear))
            ApplyEquipmentLoadout(equipmentBySlot, jobStartingGear);

        var jobLoadoutId = LoadoutSystem.GetJobPrototype(job.ID);
        if (_prototypeManager.HasIndex<RoleLoadoutPrototype>(jobLoadoutId))
        {
            var roleLoadout = profile.GetLoadoutOrDefault(jobLoadoutId, null, profile.Species, EntityManager, _prototypeManager);
            ApplyRoleLoadoutEquipment(equipmentBySlot, roleLoadout);
        }

        foreach (var slot in equipmentBySlot.Keys.OrderBy(GetPortraitSlotPriority).ThenBy(slot => slot, StringComparer.Ordinal))
        {
            var prototypeId = equipmentBySlot[slot];
            if (!_prototypeManager.TryIndex<EntityPrototype>(prototypeId, out var itemPrototype)
                || !itemPrototype.TryGetComponent<ClothingComponent>(out ClothingComponent? clothing, EntityManager.ComponentFactory)
                || !TryGetClothingPortraitLayers(clothing, slot, profile.Species, out var layers)
                || layers == null)
            {
                continue;
            }

            foreach (var layer in layers)
            {
                if (layer.Visible == false
                    || string.IsNullOrWhiteSpace(layer.RsiPath)
                    || string.IsNullOrWhiteSpace(layer.State))
                {
                    continue;
                }

                var rsiPath = ToTextureRsiPath(layer.RsiPath);
                var color = layer.Color?.ToHex() ?? "#FFFFFF";
                portraitLayers.Add(new NixWebPortraitLayer(NormalizeRsiPath(rsiPath), layer.State, color));
            }
        }
    }

    private void ApplyRoleLoadoutEquipment(Dictionary<string, string> equipmentBySlot, RoleLoadout? roleLoadout)
    {
        if (roleLoadout == null)
            return;

        foreach (var loadouts in roleLoadout.SelectedLoadouts.Values)
        {
            foreach (var loadout in loadouts)
            {
                if (!_prototypeManager.TryIndex(loadout.Prototype, out LoadoutPrototype? loadoutPrototype))
                    continue;

                if (_prototypeManager.Resolve(loadoutPrototype.StartingGear, out StartingGearPrototype? loadoutStartingGear))
                    ApplyEquipmentLoadout(equipmentBySlot, loadoutStartingGear);

                ApplyEquipmentLoadout(equipmentBySlot, loadoutPrototype);
            }
        }
    }

    private static void ApplyEquipmentLoadout(Dictionary<string, string> equipmentBySlot, IEquipmentLoadout loadout)
    {
        foreach (var (slot, prototypeId) in loadout.Equipment)
        {
            if (!string.IsNullOrWhiteSpace(slot) && !string.IsNullOrWhiteSpace(prototypeId))
                equipmentBySlot[slot] = prototypeId;
        }
    }

    private static int GetPortraitSlotPriority(string slot)
        => slot switch
        {
            "jumpsuit" => 0,
            "shoes" => 1,
            "id" => 2,
            "belt" => 3,
            "back" => 4,
            "outerClothing" => 5,
            "neck" => 6,
            "gloves" => 7,
            "mask" => 8,
            "eyes" => 9,
            "ears" => 10,
            "head" => 11,
            _ => 100,
        };

    private NixWebStationManifest CreateStationManifest(
        string stationName,
        IReadOnlyList<CrewManifestEntry> manifestEntries,
        IReadOnlyList<DepartmentPrototype> departments)
    {
        var result = new List<NixWebDepartmentManifest>();
        foreach (var department in departments)
        {
            var entries = new List<NixWebCrewMember>();
            foreach (var entry in manifestEntries)
            {
                if (!department.Roles.Contains(entry.JobPrototype))
                    continue;

                entries.Add(new NixWebCrewMember(entry.Name, entry.JobTitle, entry.JobIcon, entry.JobPrototype));
            }

            // Keep the same behavior as the in-game manifest: empty departments are omitted.
            if (entries.Count == 0)
                continue;

            result.Add(new NixWebDepartmentManifest(
                department.ID,
                Loc.GetString(department.Name),
                department.Color.ToHex(),
                entries));
        }

        return new NixWebStationManifest(stationName, result);
    }

    private async Task<T> RunOnMainThread<T>(Func<T> callback)
    {
        var completion = new TaskCompletionSource<T>();
        _tasks.RunOnMainThread(() =>
        {
            try
            {
                completion.TrySetResult(callback());
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });

        return await completion.Task.ConfigureAwait(false);
    }
}

/// <summary>
/// Current crew-manifest data returned by the web bridge.
/// </summary>
public sealed record NixWebManifestResponse(
    DateTimeOffset GeneratedAt,
    IReadOnlyList<NixWebStationManifest> Stations,
    long RoundDurationSeconds,
    int StationTimeSeconds,
    string StationDate);

/// <summary>
/// Crew-manifest data for one active station.
/// </summary>
public sealed record NixWebStationManifest(string Name, IReadOnlyList<NixWebDepartmentManifest> Departments);

/// <summary>
/// Crew members assigned to one department.
/// </summary>
public sealed record NixWebDepartmentManifest(
    string Id,
    string Name,
    string Color,
    IReadOnlyList<NixWebCrewMember> Members);

/// <summary>
/// Publicly visible crew-manifest entry.
/// </summary>
public sealed record NixWebCrewMember(string Name, string JobTitle, string JobIcon, string JobId);

public sealed record NixWebRankingResponse(
    DateTimeOffset GeneratedAt,
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<NixWebRankingEntry> Entries);

public sealed record NixWebRankingEntry(
    int ProfileId,
    string CharacterName,
    string Species,
    string AppearanceJson,
    int AchievementCount,
    int MealsCooked,
    int MealsServed,
    int DrinksPrepared,
    int DrinksServed,
    int SalvageKills,
    int SalvageAssists,
    IReadOnlyList<NixWebAchievement> Achievements);

public sealed record NixWebAchievement(string Id, string Title, DateTimeOffset AwardedAt);

public sealed record NixWebMetricRankingResponse(
    DateTimeOffset GeneratedAt,
    string Category,
    string MetricId,
    string? Type,
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<NixWebMetricRankingType> AvailableTypes,
    IReadOnlyList<NixWebMetricRankingEntry> Entries);

public sealed record NixWebMetricRankingType(
    string Id,
    string Name);

public sealed record NixWebMetricRankingEntry(
    int ProfileId,
    string CharacterName,
    string Species,
    string AppearanceJson,
    int Score);
