using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationCenterSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInOutToAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "AttendanceRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutTime",
                table: "AttendanceRecords",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "AttendanceRecords");
        }
    }
}
