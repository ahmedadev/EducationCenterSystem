using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationCenterSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherSecondAndThirdName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThirdName",
                table: "Teachers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ThirdName",
                table: "Teachers");
        }
    }
}
