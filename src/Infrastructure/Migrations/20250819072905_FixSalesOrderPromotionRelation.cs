using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSalesOrderPromotionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_Promotions_PromotionId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrderItems_SalesOrderId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrders_SalesOrderEntityId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SalesOrderPromotions_SalesOrderEntityId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropColumn(
                name: "SalesOrderEntityId",
                table: "SalesOrderPromotions");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_Promotions_PromotionId",
                table: "SalesOrderPromotions",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrders_SalesOrderId",
                table: "SalesOrderPromotions",
                column: "SalesOrderId",
                principalTable: "SalesOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_Promotions_PromotionId",
                table: "SalesOrderPromotions");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrders_SalesOrderId",
                table: "SalesOrderPromotions");

            migrationBuilder.AddColumn<Guid>(
                name: "SalesOrderEntityId",
                table: "SalesOrderPromotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderPromotions_SalesOrderEntityId",
                table: "SalesOrderPromotions",
                column: "SalesOrderEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_Promotions_PromotionId",
                table: "SalesOrderPromotions",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrderItems_SalesOrderId",
                table: "SalesOrderPromotions",
                column: "SalesOrderId",
                principalTable: "SalesOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesOrderPromotions_SalesOrders_SalesOrderEntityId",
                table: "SalesOrderPromotions",
                column: "SalesOrderEntityId",
                principalTable: "SalesOrders",
                principalColumn: "Id");
        }
    }
}
