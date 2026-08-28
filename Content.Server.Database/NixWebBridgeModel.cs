using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;

/// <summary>
/// A durable SS14 character identity used by the public Nix web bridge.
/// The profile ID is the database primary key, so deleting and recreating a slot
/// does not merge two different characters in the leaderboard.
/// </summary>
[Table("nix_web_character")]
public sealed class NixWebCharacter
{
    [Key]
    [Column("profile_id")]
    public int ProfileId { get; set; }

    [Column("owner_user_id")]
    public Guid OwnerUserId { get; set; }

    [Column("character_name")]
    public string CharacterName { get; set; } = null!;

    [Column("species")]
    public string Species { get; set; } = null!;

    [Column("appearance_json")]
    public string AppearanceJson { get; set; } = null!;

    [Column("first_seen_at")]
    public DateTimeOffset FirstSeenAt { get; set; }

    [Column("last_seen_at")]
    public DateTimeOffset LastSeenAt { get; set; }
}

/// <summary>
/// Local attribution for an achievement whose ownership and unlock decision remain in NullLink.
/// </summary>
[Table("nix_web_achievement")]
[Index(nameof(SourceUserId), nameof(AchievementId), IsUnique = true)]
[Index(nameof(CharacterProfileId), nameof(AwardedAt))]
public sealed class NixWebAchievement
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("source_user_id")]
    public Guid SourceUserId { get; set; }

    [Column("character_profile_id")]
    public int CharacterProfileId { get; set; }

    [Column("achievement_id")]
    public string AchievementId { get; set; } = null!;

    [Column("awarded_at")]
    public DateTimeOffset AwardedAt { get; set; }

    [Column("round_id")]
    public int RoundId { get; set; }

    [Column("character_name_snapshot")]
    public string CharacterNameSnapshot { get; set; } = null!;

    [Column("appearance_snapshot_json")]
    public string AppearanceSnapshotJson { get; set; } = null!;
}

/// <summary>
/// Append-only, character-attributed metrics for activities that are not NullLink achievements.
/// </summary>
[Table("nix_web_statistic")]
[Index(nameof(CharacterProfileId), nameof(MetricId), nameof(OccurredAt))]
public sealed class NixWebStatistic
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("character_profile_id")]
    public int CharacterProfileId { get; set; }

    [Column("metric_id")]
    public string MetricId { get; set; } = null!;

    [Column("amount")]
    public int Amount { get; set; }

    [Column("occurred_at")]
    public DateTimeOffset OccurredAt { get; set; }

    [Column("round_id")]
    public int RoundId { get; set; }

    [Column("metadata")]
    public string? Metadata { get; set; }
}

public abstract partial class ServerDbContext
{
    public DbSet<NixWebCharacter> NixWebCharacters { get; set; } = null!;
    public DbSet<NixWebAchievement> NixWebAchievements { get; set; } = null!;
    public DbSet<NixWebStatistic> NixWebStatistics { get; set; } = null!;
}

public sealed record NixWebCharacterIdentity(
    Guid UserId,
    int ProfileSlot,
    string CharacterName,
    string Species,
    string AppearanceJson);

public sealed record NixWebAchievementData(string AchievementId, DateTimeOffset AwardedAt);

public sealed record NixWebCharacterRanking(
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
    IReadOnlyList<NixWebAchievementData> Achievements);

public sealed record NixWebRankingPage(int Total, IReadOnlyList<NixWebCharacterRanking> Entries);

/// <summary>
/// A single character's aggregate for one role-specific public leaderboard.
/// </summary>
public sealed record NixWebMetricRanking(
    int ProfileId,
    string CharacterName,
    string Species,
    string AppearanceJson,
    int Score);

public sealed record NixWebMetricRankingPage(int Total, IReadOnlyList<NixWebMetricRanking> Entries);
