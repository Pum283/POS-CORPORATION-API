using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIngredientInDocumentSupplierEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IngredientId",
                table: "DocumentSuppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSuppliers_IngredientId",
                table: "DocumentSuppliers",
                column: "IngredientId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSuppliers_Ingredients_IngredientId",
                table: "DocumentSuppliers",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSuppliers_Ingredients_IngredientId",
                table: "DocumentSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSuppliers_IngredientId",
                table: "DocumentSuppliers");

            migrationBuilder.DropColumn(
                name: "IngredientId",
                table: "DocumentSuppliers");
        }
    }
}
