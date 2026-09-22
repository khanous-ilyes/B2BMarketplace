using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationBadge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationAdminNote",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationDocumentsJson",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNote",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationRequestedAt",
                table: "Suppliers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationResolvedAt",
                table: "Suppliers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatus",
                table: "Suppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationAdminNote",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VerificationDocumentsJson",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VerificationNote",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VerificationRequestedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VerificationResolvedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Suppliers");
        }
    }
}
