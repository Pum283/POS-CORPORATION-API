using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToFoodAppTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột IsActive vào FoodAppPlatforms
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FoodAppPlatforms",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Thêm cột IsActive vào FoodAppPrices
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FoodAppPrices",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FoodAppPrices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FoodAppPlatforms");
        }
    }
}