using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.CrewManifest;
using Content.Server.Database;
using Content.Server.GameTicking;
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
using Content.Shared.CrewManifest;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Nutrition;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.StationRecords;
using Content.Shared._Starlight.Achievement;
using Content.Shared._Starlight.Time;
using Content.Shared.Tag;
using Robust.Server.Player;
using Robust.Server.ServerStatus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
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

    // Keep the initial category deliberately small and based on actual salvage fauna in this content branch.
    private static readonly HashSet<string> SalvageCreaturePrototypes = new(StringComparer.Ordinal)
    {
        "MobGoliath",
        "MobBasilisk",
        "MobHivelord",
        "MobWatcherLavaland",
    };

    // A category only maps to a metric recorded by the server; clients cannot choose arbitrary database fields.
    private static readonly Dictionary<string, string> LeaderboardMetrics = new(StringComparer.OrdinalIgnoreCase)
    {
        ["chef"] = "food.served",
        ["bartender"] = "drink.served",
        ["salvage"] = "salvage.kills",
    };

    [Dependency] private IStatusHost _statusHost = default!;
    [Dependency] private IConfigurationManager _config = default!;
    [Dependency] private ITaskManager _tasks = default!;
    [Dependency] private IServerDbManager _database = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private SharedIdCardSystem _idCard = default!;
    [Dependency] private InventorySystem _inventory = default!;
    [Dependency] private IServerPreferencesManager _preferences = default!;
    [Dependency] private GameTicker _gameTicker = default!;
    [Dependency] private CrewManifestSystem _crewManifest = default!;
    [Dependency] private StationRecordsSystem _stationRecords = default!;
    [Dependency] private StationSystem _stationSystem = default!;
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private SharedTimeSystem _stationTime = default!;
    [Dependency] private TagSystem _tag = default!;

    private readonly Dictionary<NetUserId, NixWebCharacterIdentity> _activeCharacters = new();
    private string _apiToken = string.Empty;

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        _config.OnValueChanged(CCVars.NixWebApiToken, UpdateApiToken, true);
        _statusHost.AddHandler(HandleStatusRequest);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawnComplete);
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

            var leaderboardRanking = await _database.GetNixWebMetricRankingAsync(metricId, offset, pageSize).ConfigureAwait(false);
            var leaderboardResponse = BuildMetricRankingResponse(category, leaderboardRanking, page, pageSize);
            await context.RespondJsonAsync(leaderboardResponse).ConfigureAwait(false);
            return true;
        }

        if (path != RankingPath)
            return false;

        if (!await CheckAccess(context))
            return true;

        var ranking = await _database.GetNixWebRankingAsync(offset, pageSize).ConfigureAwait(false);
        var response = await RunOnMainThread(() => BuildRankingResponse(ranking, page, pageSize)).ConfigureAwait(false);
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
        if (!_activeCharacters.TryGetValue(session.UserId, out var character))
            return;

        _database.RecordNixWebAchievementAsync(character, achievementId, _gameTicker.RoundId).FireAndForget();
    }

    /// <summary>
    /// Records a completed recipe and labels its resulting food so a later real consumption can count as service.
    /// </summary>
    public void TrackCookedFood(EntityUid? cook, EntityUid food, string recipeId)
    {
        if (cook == null
            || !_playerManager.TryGetSessionByEntity(cook.Value, out var session)
            || !_activeCharacters.TryGetValue(session.UserId, out var character)
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
            || !_activeCharacters.TryGetValue(session.UserId, out var character)
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

        _database.RecordNixWebStatisticAsync(character, "drink.prepared", 1, _gameTicker.RoundId, dispenserId).FireAndForget();
    }

    private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent args)
    {
        if (!_preferences.TryGetCachedPreferences(args.Player.UserId, out var preferences)
            || !preferences.TryIndexOfCharacter(args.Profile, out var slot))
        {
            return;
        }

        _activeCharacters[args.Player.UserId] = CreateCharacterIdentity(args.Player.UserId, slot, args.Profile);
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
        if (!_activeCharacters.TryGetValue(session.UserId, out var character))
            return;

        var metric = assist ? "salvage.assists" : "salvage.kills";
        _database.RecordNixWebStatisticAsync(character, metric, 1, _gameTicker.RoundId, creatureId).FireAndForget();
        _database.RecordNixWebStatisticAsync(character, $"{metric}.{creatureId}", 1, _gameTicker.RoundId).FireAndForget();
    }

    private bool HasJob(ICommonSession session, string requiredJobId)
    {
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

    private NixWebCharacterIdentity CreateCharacterIdentity(NetUserId userId, int profileSlot, HumanoidCharacterProfile profile)
    {
        var appearance = profile.Appearance;
        var snapshot = new NixWebCharacterAppearance(
            profile.Sex.ToString(),
            appearance.HairStyleId,
            appearance.HairColor.ToHex(),
            appearance.FacialHairStyleId,
            appearance.FacialHairColor.ToHex(),
            appearance.EyeColor.ToHex(),
            appearance.SkinColor.ToHex(),
            appearance.Markings.ConvertAll(marking => marking.ToDBString()),
            appearance.Width,
            appearance.Height,
            BuildPortraitLayers(profile));

        return new NixWebCharacterIdentity(
            userId.UserId,
            profileSlot,
            profile.Name,
            profile.Species,
            JsonSerializer.Serialize(snapshot));
    }

    /// <summary>
    /// Resolves the same SSI layers used by the humanoid client for a compact public portrait.
    /// The website only receives paths and state names; it never receives a player's account data.
    /// </summary>
    private List<NixWebPortraitLayer> BuildPortraitLayers(HumanoidCharacterProfile profile)
    {
        var result = new List<NixWebPortraitLayer>();
        var species = _prototypeManager.Index<SpeciesPrototype>(profile.Species);
        var baseSprites = _prototypeManager.Index<HumanoidSpeciesBaseSpritesPrototype>(species.SpriteSet);
        var appearance = profile.Appearance;

        // These layers form a full-standing, unclothed profile portrait. Equipment is intentionally omitted:
        // it is round-specific, while a ranking entry is historical and character-specific.
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
            if (!baseSprites.Sprites.TryGetValue(layer, out var spriteId))
                continue;

            spriteId = HumanoidVisualLayersExtension.GetSexMorph(layer, profile.Sex, spriteId);
            if (!_prototypeManager.TryIndex<HumanoidSpeciesSpriteLayer>(spriteId, out var sprite)
                || sprite.BaseSprite is not SpriteSpecifier.Rsi rsi)
            {
                continue;
            }

            var color = sprite.MatchSkin
                ? appearance.SkinColor.WithAlpha(sprite.LayerAlpha).ToHex()
                : layer == HumanoidVisualLayers.Eyes
                    ? appearance.EyeColor.ToHex()
                    : "#FFFFFF";
            result.Add(new NixWebPortraitLayer(NormalizeRsiPath(rsi), rsi.RsiState, color));
        }

        var markings = new List<Marking>(appearance.Markings);
        markings.Add(new Marking(appearance.HairStyleId, new[] { appearance.HairColor }, appearance.HairGlowing));
        markings.Add(new Marking(appearance.FacialHairStyleId, new[] { appearance.FacialHairColor }, appearance.FacialHairGlowing));

        foreach (var marking in markings)
        {
            if (!_prototypeManager.TryIndex<MarkingPrototype>(marking.MarkingId, out var prototype)
                || !IsPortraitLayer(prototype.BodyPart))
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

        return result;
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

    private static string NormalizeRsiPath(SpriteSpecifier.Rsi rsi)
        => rsi.RsiPath.ToString().Replace("/Textures/", string.Empty, StringComparison.Ordinal).TrimStart('/');

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

    private NixWebRankingResponse BuildRankingResponse(NixWebRankingPage ranking, int page, int pageSize)
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
                entry.AppearanceJson,
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

    private static NixWebMetricRankingResponse BuildMetricRankingResponse(
        string category,
        NixWebMetricRankingPage ranking,
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
                entry.AppearanceJson,
                entry.Score));
        }

        return new NixWebMetricRankingResponse(DateTimeOffset.UtcNow, category, page, pageSize, ranking.Total, entries);
    }

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

public sealed record NixWebCharacterAppearance(
    string Sex,
    string HairStyleId,
    string HairColor,
    string FacialHairStyleId,
    string FacialHairColor,
    string EyeColor,
    string SkinColor,
    IReadOnlyList<string> Markings,
    float Width,
    float Height,
    IReadOnlyList<NixWebPortraitLayer> PortraitLayers);

public sealed record NixWebPortraitLayer(string RsiPath, string State, string Color);

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
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<NixWebMetricRankingEntry> Entries);

public sealed record NixWebMetricRankingEntry(
    int ProfileId,
    string CharacterName,
    string Species,
    string AppearanceJson,
    int Score);
