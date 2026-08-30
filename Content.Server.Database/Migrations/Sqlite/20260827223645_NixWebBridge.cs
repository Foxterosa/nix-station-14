using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class NixWebBridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nix_web_achievement",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    source_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    character_profile_id = table.Column<int>(type: "INTEGER", nullable: false),
                    achievement_id = table.Column<string>(type: "TEXT", nullable: false),
                    awarded_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    round_id = table.Column<int>(type: "INTEGER", nullable: false),
                    character_name_snapshot = table.Column<string>(type: "TEXT", nullable: false),
                    appearance_snapshot_json = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nix_web_achievement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nix_web_character",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    owner_user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    character_name = table.Column<string>(type: "TEXT", nullable: false),
                    species = table.Column<string>(type: "TEXT", nullable: false),
                    appearance_json = table.Column<string>(type: "TEXT", nullable: false),
                    first_seen_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nix_web_character", x => x.profile_id);
                });

            migrationBuilder.CreateTable(
                name: "nix_web_statistic",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    character_profile_id = table.Column<int>(type: "INTEGER", nullable: false),
                    metric_id = table.Column<string>(type: "TEXT", nullable: false),
                    amount = table.Column<int>(type: "INTEGER", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    round_id = table.Column<int>(type: "INTEGER", nullable: false),
                    metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nix_web_statistic", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nix_web_achievement_character_profile_id_awarded_at",
                table: "nix_web_achievement",
                columns: new[] { "character_profile_id", "awarded_at" });

            migrationBuilder.CreateIndex(
                name: "IX_nix_web_achievement_source_user_id_achievement_id",
                table: "nix_web_achievement",
                columns: new[] { "source_user_id", "achievement_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_nix_web_statistic_character_profile_id_metric_id_occurred_at",
                table: "nix_web_statistic",
                columns: new[] { "character_profile_id", "metric_id", "occurred_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nix_web_achievement");

            migrationBuilder.DropTable(
                name: "nix_web_character");

            migrationBuilder.DropTable(
                name: "nix_web_statistic");
        }
    }
}
