using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeOwners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "homeowners");

            migrationBuilder.CreateTable(
                name: "communities",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    properties_count = table.Column<int>(type: "int", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "HomeOwner"),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    ReferalCodeId = table.Column<long>(type: "bigint", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "properties",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<long>(type: "bigint", nullable: true),
                    property_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    occupants = table.Column<int>(type: "int", nullable: false),
                    dues = table.Column<double>(type: "float", nullable: true),
                    monthly_dues = table.Column<double>(type: "float", nullable: false),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    CommunityId = table.Column<long>(type: "bigint", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.id);
                    table.ForeignKey(
                        name: "FK_properties_communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "homeowners",
                        principalTable: "communities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_properties_users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "homeowners",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "referal_codes",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_used = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatorId = table.Column<long>(type: "bigint", nullable: true),
                    CommunityId = table.Column<long>(type: "bigint", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referal_codes", x => x.id);
                    table.ForeignKey(
                        name: "FK_referal_codes_communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "homeowners",
                        principalTable: "communities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_referal_codes_users_CreatorId",
                        column: x => x.CreatorId,
                        principalSchema: "homeowners",
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_referal_codes_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "homeowners",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "homeowners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    street_address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    state = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    building_number = table.Column<long>(type: "bigint", nullable: true),
                    floor_number = table.Column<long>(type: "bigint", nullable: true),
                    apartment_number = table.Column<long>(type: "bigint", nullable: true),
                    latitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    longitude = table.Column<double>(type: "float(9)", precision: 9, scale: 6, nullable: true),
                    PropertyId = table.Column<long>(type: "bigint", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_addresses_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalSchema: "homeowners",
                        principalTable: "properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_PropertyId",
                schema: "homeowners",
                table: "addresses",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_properties_CommunityId",
                schema: "homeowners",
                table: "properties",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_properties_OwnerId",
                schema: "homeowners",
                table: "properties",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_referal_codes_CommunityId",
                schema: "homeowners",
                table: "referal_codes",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_referal_codes_CreatorId",
                schema: "homeowners",
                table: "referal_codes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_referal_codes_UserId",
                schema: "homeowners",
                table: "referal_codes",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses",
                schema: "homeowners");

            migrationBuilder.DropTable(
                name: "referal_codes",
                schema: "homeowners");

            migrationBuilder.DropTable(
                name: "properties",
                schema: "homeowners");

            migrationBuilder.DropTable(
                name: "communities",
                schema: "homeowners");

            migrationBuilder.DropTable(
                name: "users",
                schema: "homeowners");
        }
    }
}
