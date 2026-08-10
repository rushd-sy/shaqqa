using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneVerificationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedAttempts",
                table: "PhoneVerifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneVerifications_PhoneNumber_IsUsed_ExpiresAtUtc",
                table: "PhoneVerifications",
                columns: new[] { "PhoneNumber", "IsUsed", "ExpiresAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhoneVerifications_PhoneNumber_IsUsed_ExpiresAtUtc",
                table: "PhoneVerifications");

            migrationBuilder.DropColumn(
                name: "FailedAttempts",
                table: "PhoneVerifications");
        }
    }
}
