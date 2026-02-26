using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Notifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRepayment_Loans_LoanId",
                table: "UserRepayment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRepayment",
                table: "UserRepayment");

            migrationBuilder.RenameTable(
                name: "UserRepayment",
                newName: "UserRepayments");

            migrationBuilder.RenameIndex(
                name: "IX_UserRepayment_LoanId",
                table: "UserRepayments",
                newName: "IX_UserRepayments_LoanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRepayments",
                table: "UserRepayments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRepayments_Loans_LoanId",
                table: "UserRepayments",
                column: "LoanId",
                principalSchema: "dbo",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRepayments_Loans_LoanId",
                table: "UserRepayments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRepayments",
                table: "UserRepayments");

            migrationBuilder.RenameTable(
                name: "UserRepayments",
                newName: "UserRepayment");

            migrationBuilder.RenameIndex(
                name: "IX_UserRepayments_LoanId",
                table: "UserRepayment",
                newName: "IX_UserRepayment_LoanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRepayment",
                table: "UserRepayment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRepayment_Loans_LoanId",
                table: "UserRepayment",
                column: "LoanId",
                principalSchema: "dbo",
                principalTable: "Loans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
