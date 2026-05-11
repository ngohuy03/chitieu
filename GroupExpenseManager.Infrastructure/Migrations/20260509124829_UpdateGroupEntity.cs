using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupExpenseManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Groups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ContributorId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsDelete",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextSearch",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ContributorId",
                table: "Groups",
                column: "ContributorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Users_ContributorId",
                table: "Groups",
                column: "ContributorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Users_ContributorId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ContributorId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ContributorId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "TextSearch",
                table: "Groups");
        }
    }
}
