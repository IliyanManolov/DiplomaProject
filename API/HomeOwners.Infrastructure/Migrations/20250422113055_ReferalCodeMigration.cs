using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeOwners.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferalCodeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "homeowners",
                table: "referal_codes",
                newName: "code");

            migrationBuilder.AlterColumn<Guid>(
                name: "code",
                schema: "homeowners",
                table: "referal_codes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                schema: "homeowners",
                table: "referal_codes",
                newName: "Code");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "homeowners",
                table: "referal_codes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
