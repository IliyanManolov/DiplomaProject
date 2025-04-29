using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeOwners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityMeetingsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "community_meetings",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<long>(type: "bigint", nullable: true),
                    CommunityId = table.Column<long>(type: "bigint", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    meeting_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_meetings", x => x.id);
                    table.ForeignKey(
                        name: "FK_community_meetings_communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "homeowners",
                        principalTable: "communities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_community_meetings_users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "homeowners",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_community_meetings_CommunityId",
                schema: "homeowners",
                table: "community_meetings",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_community_meetings_CreatorId",
                schema: "homeowners",
                table: "community_meetings",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "community_meetings",
                schema: "homeowners");
        }
    }
}
