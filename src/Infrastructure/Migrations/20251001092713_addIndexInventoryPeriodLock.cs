using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIndexInventoryPeriodLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryPeriodLocks_StoreId_ProductId_IngredientId_CreateDate",
                table: "InventoryPeriodLocks",
                columns: new[] { "StoreId", "ProductId", "IngredientId", "CreateDate" },
                unique: true,
                filter: "[ProductId] IS NOT NULL AND [IngredientId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryPeriodLocks_StoreId_ProductId_IngredientId_CreateDate",
                table: "InventoryPeriodLocks");
        }
    }
}
