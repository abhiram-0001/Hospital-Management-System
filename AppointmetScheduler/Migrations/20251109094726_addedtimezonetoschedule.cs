using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmetScheduler.Migrations
{
    /// <inheritdoc />
    public partial class addedtimezonetoschedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "DoctorSchedules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "DoctorSchedules");
        }
    }
}
