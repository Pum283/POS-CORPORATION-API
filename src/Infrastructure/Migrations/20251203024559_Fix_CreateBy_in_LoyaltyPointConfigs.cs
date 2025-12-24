using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_CreateBy_in_LoyaltyPointConfigs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoyaltyPointConfigs_Users_CreatedByUserId",
                table: "LoyaltyPointConfigs");

            migrationBuilder.DropIndex(
                name: "IX_LoyaltyPointConfigs_CreatedByUserId",
                table: "LoyaltyPointConfigs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LoyaltyPointConfigs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "LoyaltyPointConfigs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyPointConfigs_CreatedByUserId",
                table: "LoyaltyPointConfigs",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoyaltyPointConfigs_Users_CreatedByUserId",
                table: "LoyaltyPointConfigs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
