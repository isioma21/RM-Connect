using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RmConnect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Branch",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branch",
                table: "Users");
        }
    }
}
