using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Shared.Preferences;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;

public abstract partial class ServerDbBase
{
    public async Task RecordNixWebAchievementAsync(
        NixWebCharacterIdentity identity,
        string achievementId,
        int roundId)
    {
        await using var db = await GetDb();
        var character = await GetOrUpdateNixWebCharacterAsync(db.DbContext, identity);
        if (character == null)
            return;

        var alreadyAttributed = await db.DbContext.NixWebAchievements.AnyAsync(achievement =>
            achievement.SourceUserId == identity.UserId && achievement.AchievementId == achievementId);
        if (alreadyAttributed)
            return;

        var now = DateTimeOffset.UtcNow;
        db.DbContext.NixWebAchievements.Add(new NixWebAchievement
        {
            SourceUserId = identity.UserId,
            CharacterProfileId = character.ProfileId,
            AchievementId = achievementId,
            AwardedAt = now,
            RoundId = roundId,
            CharacterNameSnapshot = identity.CharacterName,
            AppearanceSnapshotJson = character.AppearanceJson,
        });

        await db.DbContext.SaveChangesAsync();
    }

    public async Task RecordNixWebStatisticAsync(
        NixWebCharacterIdentity identity,
        string metricId,
        int amount,
        int roundId,
        string? metadata = null)
    {
        if (amount <= 0)
            return;

        await using var db = await GetDb();
        var character = await GetOrUpdateNixWebCharacterAsync(db.DbContext, identity);
        if (character == null)
            return;

        db.DbContext.NixWebStatistics.Add(new NixWebStatistic
        {
            CharacterProfileId = character.ProfileId,
            MetricId = metricId,
            Amount = amount,
            OccurredAt = DateTimeOffset.UtcNow,
            RoundId = roundId,
            Metadata = metadata,
        });

        await db.DbContext.SaveChangesAsync();
    }

    public async Task UpsertNixWebAppearanceAsync(NixWebCharacterIdentity identity)
    {
        await using var db = await GetDb();
        var character = await GetOrUpdateNixWebCharacterAsync(db.DbContext, identity);
        if (character == null)
            return;

        await db.DbContext.SaveChangesAsync();
    }

    public async Task<NixWebRankingPage> GetNixWebRankingAsync(int offset, int limit)
    {
        await using var db = await GetDb();
        var characters = db.DbContext.NixWebCharacters.AsNoTracking();
        var achievements = db.DbContext.NixWebAchievements.AsNoTracking();
        var statistics = db.DbContext.NixWebStatistics.AsNoTracking();

        var ranking = characters
            .Where(character => achievements.Any(achievement => achievement.CharacterProfileId == character.ProfileId))
            .Select(character => new
            {
                character.ProfileId,
                character.CharacterName,
                character.Species,
                character.AppearanceJson,
                AchievementCount = achievements.Count(achievement => achievement.CharacterProfileId == character.ProfileId),
                MealsCooked = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "food.cooked")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
                MealsServed = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "food.served")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
                DrinksPrepared = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "drink.prepared")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
                DrinksServed = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "drink.served")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
                SalvageKills = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "salvage.kills")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
                SalvageAssists = statistics
                    .Where(statistic => statistic.CharacterProfileId == character.ProfileId && statistic.MetricId == "salvage.assists")
                    .Select(statistic => (int?) statistic.Amount)
                    .Sum() ?? 0,
            });

        var total = await ranking.CountAsync();
        var page = await ranking
            .OrderByDescending(character => character.AchievementCount)
            .ThenByDescending(character => character.MealsServed)
            .ThenByDescending(character => character.MealsCooked)
            .ThenBy(character => character.CharacterName)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var profileIds = page.Select(character => character.ProfileId).ToArray();
        var awarded = await achievements
            .Where(achievement => profileIds.Contains(achievement.CharacterProfileId))
            .OrderByDescending(achievement => achievement.AwardedAt)
            .Select(achievement => new
            {
                achievement.CharacterProfileId,
                achievement.AchievementId,
                achievement.AwardedAt,
            })
            .ToListAsync();

        var byCharacter = awarded
            .GroupBy(achievement => achievement.CharacterProfileId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<NixWebAchievementData>) group
                    .Select(achievement => new NixWebAchievementData(achievement.AchievementId, achievement.AwardedAt))
                    .ToArray());

        var entries = page.Select(character => new NixWebCharacterRanking(
            character.ProfileId,
            character.CharacterName,
            character.Species,
            character.AppearanceJson,
            character.AchievementCount,
            character.MealsCooked,
            character.MealsServed,
            character.DrinksPrepared,
            character.DrinksServed,
            character.SalvageKills,
            character.SalvageAssists,
            byCharacter.GetValueOrDefault(character.ProfileId, Array.Empty<NixWebAchievementData>()))).ToArray();

        return new NixWebRankingPage(total, entries);
    }

    public async Task<NixWebMetricRankingPage> GetNixWebMetricRankingAsync(string metricId, int offset, int limit)
    {
        await using var db = await GetDb();
        var totals = db.DbContext.NixWebStatistics
            .AsNoTracking()
            .Where(statistic => statistic.MetricId == metricId)
            .GroupBy(statistic => statistic.CharacterProfileId)
            .Select(group => new
            {
                ProfileId = group.Key,
                Score = group.Sum(statistic => statistic.Amount),
            });

        var ranking = from character in db.DbContext.NixWebCharacters.AsNoTracking()
            join total in totals on character.ProfileId equals total.ProfileId
            select new
            {
                character.ProfileId,
                character.CharacterName,
                character.Species,
                character.AppearanceJson,
                total.Score,
            };

        var totalCount = await ranking.CountAsync();
        var page = await ranking
            .OrderByDescending(character => character.Score)
            .ThenBy(character => character.CharacterName)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        var entries = page.Select(character => new NixWebMetricRanking(
            character.ProfileId,
            character.CharacterName,
            character.Species,
            character.AppearanceJson,
            character.Score)).ToArray();

        return new NixWebMetricRankingPage(totalCount, entries);
    }

    public async Task<Dictionary<int, HumanoidCharacterProfile>> GetNixWebProfilesAsync(IEnumerable<int> profileIds)
    {
        var ids = profileIds.Distinct().ToArray();
        if (ids.Length == 0)
            return new Dictionary<int, HumanoidCharacterProfile>();

        await using var db = await GetDb();
        var profiles = await db.DbContext.Profile
            .AsNoTracking()
            .Where(profile => ids.Contains(profile.Id))
            .Include(profile => profile.Jobs)
            .Include(profile => profile.Antags)
            .Include(profile => profile.Traits)
            .Include(profile => profile.StarLightProfile)
            .Include(profile => profile.CharacterInfo)
            .Include(profile => profile.Loadouts)
                .ThenInclude(loadout => loadout.Groups)
                .ThenInclude(group => group.Loadouts)
            .Include(profile => profile.CDProfile)
                .ThenInclude(cdProfile => cdProfile!.CharacterRecordEntries)
            .AsSplitQuery()
            .ToListAsync();

        return profiles.ToDictionary(profile => profile.Id, ConvertProfiles);
    }

    private static async Task<NixWebCharacter?> GetOrUpdateNixWebCharacterAsync(
        ServerDbContext db,
        NixWebCharacterIdentity identity)
    {
        var profileId = await db.Profile
            .Where(profile => profile.Preference.UserId == identity.UserId && profile.Slot == identity.ProfileSlot)
            .Select(profile => (int?) profile.Id)
            .SingleOrDefaultAsync();
        if (profileId == null)
            return null;

        var now = DateTimeOffset.UtcNow;
        var character = await db.NixWebCharacters.SingleOrDefaultAsync(item => item.ProfileId == profileId.Value);
        if (character == null)
        {
            character = new NixWebCharacter
            {
                ProfileId = profileId.Value,
                OwnerUserId = identity.UserId,
                CharacterName = identity.CharacterName,
                Species = identity.Species,
                AppearanceJson = identity.AppearanceJson,
                FirstSeenAt = now,
                LastSeenAt = now,
            };
            db.NixWebCharacters.Add(character);
            return character;
        }

        character.CharacterName = identity.CharacterName;
        character.Species = identity.Species;
        character.AppearanceJson = NixWebBridgeAppearanceJson.SelectPreferred(character.AppearanceJson, identity.AppearanceJson);
        character.LastSeenAt = now;
        return character;
    }
}
