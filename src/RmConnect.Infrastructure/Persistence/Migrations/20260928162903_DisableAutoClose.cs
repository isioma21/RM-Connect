using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RmConnect.Infrastructure.Persistence.Migrations
{
    /// <summary>Turns off AUTO_CLOSE (on by default in LocalDB) so the database doesn't shut down and slow the next request after idle.</summary>
    public partial class DisableAutoClose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF DATABASEPROPERTYEX(DB_NAME(), 'IsAutoClose') = 1 ALTER DATABASE CURRENT SET AUTO_CLOSE OFF",
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
