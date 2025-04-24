using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeOwners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "community_messages",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: true),
                    CommunityId = table.Column<long>(type: "bigint", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_community_messages_communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "homeowners",
                        principalTable: "communities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_community_messages_users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "homeowners",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_community_messages_CommunityId",
                schema: "homeowners",
                table: "community_messages",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_community_messages_CreatorId",
                schema: "homeowners",
                table: "community_messages",
                column: "CreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "community_messages",
                schema: "homeowners");
        }
    }
}
