using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_SalesOrderItemId_To_SalesOrderPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SalesOrderItemId",
                table: "SalesOrderPromotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderPromotions_SalesOrderItemId",
                table: "SalesOrderPromotions",
                column: "SalesOrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrderItems_SalesOrderItemId",
                table: "SalesOrderPromotions",
                column: "SalesOrderItemId",
                principalTable: "SalesOrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrderItems_SalesOrderItemId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrderPromotions_SalesOrderItemId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropColumn(
                name: "SalesOrderItemId",
                table: "SalesOrderPromotions");
        }
    }
}
