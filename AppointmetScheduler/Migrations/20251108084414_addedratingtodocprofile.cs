using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmetScheduler.Migrations
{
    /// <inheritdoc />
    public partial class addedratingtodocprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DoctorProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rating",
                table: "DoctorProfiles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "DoctorProfiles");
        }
    }
}
