using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnProductIdForDocumentSupplierEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "DocumentSuppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "DocumentSuppliers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSuppliers_ProductId",
                table: "DocumentSuppliers",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentSuppliers_Products_ProductId",
                table: "DocumentSuppliers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentSuppliers_Products_ProductId",
                table: "DocumentSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_DocumentSuppliers_ProductId",
                table: "DocumentSuppliers");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "DocumentSuppliers");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "DocumentSuppliers");
        }
    }
}
