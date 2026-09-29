using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RmConnect.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelAndEndReasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndReason",
                table: "Relationships",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Appointments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndReason",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Appointments");
        }
    }
}
