using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixPeriodlockAndPeriodlocknotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodAppMoney",
                table: "PeriodLockNotes");

            migrationBuilder.DropColumn(
                name: "PaymentMoney",
                table: "PeriodLockNotes");

            migrationBuilder.AddColumn<decimal>(
                name: "FoodAppMoney",
                table: "PeriodLocks",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentMoney",
                table: "PeriodLocks",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FoodAppMoney",
                table: "PeriodLocks");

            migrationBuilder.DropColumn(
                name: "PaymentMoney",
                table: "PeriodLocks");

            migrationBuilder.AddColumn<decimal>(
                name: "FoodAppMoney",
                table: "PeriodLockNotes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentMoney",
                table: "PeriodLockNotes",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
