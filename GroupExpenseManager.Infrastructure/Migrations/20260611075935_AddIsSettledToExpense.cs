using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupExpenseManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSettledToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSettled",
                table: "Expenses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSettled",
                table: "Expenses");
        }
    }
}
