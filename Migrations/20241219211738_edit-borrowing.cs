using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libereay_System.Migrations
{
    /// <inheritdoc />
    public partial class editborrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "BorrowTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "BorrowTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
