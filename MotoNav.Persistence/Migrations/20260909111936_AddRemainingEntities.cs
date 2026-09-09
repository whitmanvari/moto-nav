using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MotoNav.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HazardReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Location = table.Column<Point>(type: "geometry", nullable: true),
                    UpVotes = table.Column<int>(type: "integer", nullable: false),
                    DownVotes = table.Column<int>(type: "integer", nullable: false),
                    TotalVotes = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedViaVoiceCommand = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HazardReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoadConditionReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Condition = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<Point>(type: "geometry", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadConditionReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WindHazardZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Boundary = table.Column<Geometry>(type: "geometry", nullable: false),
                    AverageWindSpeedKmh = table.Column<double>(type: "double precision", nullable: false),
                    DangerousGustSpeedKmh = table.Column<double>(type: "double precision", nullable: false),
                    WarningMessage = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WindHazardZones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HazardVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HazardReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HazardVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HazardVerifications_HazardReports_HazardReportId",
                        column: x => x.HazardReportId,
                        principalTable: "HazardReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HazardVerifications_HazardReportId",
                table: "HazardVerifications",
                column: "HazardReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HazardVerifications");

            migrationBuilder.DropTable(
                name: "RoadConditionReports");

            migrationBuilder.DropTable(
                name: "WindHazardZones");

            migrationBuilder.DropTable(
                name: "HazardReports");
        }
    }
}
