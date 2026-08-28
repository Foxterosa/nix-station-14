using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
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
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_profile_id = table.Column<int>(type: "integer", nullable: false),
                    achievement_id = table.Column<string>(type: "text", nullable: false),
                    awarded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    character_name_snapshot = table.Column<string>(type: "text", nullable: false),
                    appearance_snapshot_json = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nix_web_achievement", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nix_web_character",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_name = table.Column<string>(type: "text", nullable: false),
                    species = table.Column<string>(type: "text", nullable: false),
                    appearance_json = table.Column<string>(type: "text", nullable: false),
                    first_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nix_web_character", x => x.profile_id);
                });

            migrationBuilder.CreateTable(
                name: "nix_web_statistic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    character_profile_id = table.Column<int>(type: "integer", nullable: false),
                    metric_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    round_id = table.Column<int>(type: "integer", nullable: false),
                    metadata = table.Column<string>(type: "text", nullable: true)
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
