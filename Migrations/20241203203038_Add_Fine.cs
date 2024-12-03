using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libereay_System.Migrations
{
    /// <inheritdoc />
    public partial class Add_Fine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FineConfigurations",
                columns: new[] { "Id", "FinePerDay" },
                values: new object[] { 1, 2.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FineConfigurations",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
