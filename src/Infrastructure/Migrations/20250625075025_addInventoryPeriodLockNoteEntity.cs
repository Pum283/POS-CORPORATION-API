using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addInventoryPeriodLockNoteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "PeriodLocks");

            migrationBuilder.CreateTable(
                name: "PeriodLockNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodLockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpendingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodLockNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodLockNotes_PeriodLocks_PeriodLockId",
                        column: x => x.PeriodLockId,
                        principalTable: "PeriodLocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PeriodLockNotes_PeriodLockId",
                table: "PeriodLockNotes",
                column: "PeriodLockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodLockNotes");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "PeriodLocks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
