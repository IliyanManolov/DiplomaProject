using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeOwners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePropertyNotMandatoryForAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_properties_PropertyId",
                schema: "homeowners",
                table: "addresses");

            migrationBuilder.DropIndex(
                name: "IX_addresses_PropertyId",
                schema: "homeowners",
                table: "addresses");

            migrationBuilder.AlterColumn<long>(
                name: "PropertyId",
                schema: "homeowners",
                table: "addresses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_PropertyId",
                schema: "homeowners",
                table: "addresses",
                column: "PropertyId",
                unique: true,
                filter: "[PropertyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_properties_PropertyId",
                schema: "homeowners",
                table: "addresses",
                column: "PropertyId",
                principalSchema: "homeowners",
                principalTable: "properties",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_addresses_properties_PropertyId",
                schema: "homeowners",
                table: "addresses");

            migrationBuilder.DropIndex(
                name: "IX_addresses_PropertyId",
                schema: "homeowners",
                table: "addresses");

            migrationBuilder.AlterColumn<long>(
                name: "PropertyId",
                schema: "homeowners",
                table: "addresses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_addresses_PropertyId",
                schema: "homeowners",
                table: "addresses",
                column: "PropertyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_addresses_properties_PropertyId",
                schema: "homeowners",
                table: "addresses",
                column: "PropertyId",
                principalSchema: "homeowners",
                principalTable: "properties",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
