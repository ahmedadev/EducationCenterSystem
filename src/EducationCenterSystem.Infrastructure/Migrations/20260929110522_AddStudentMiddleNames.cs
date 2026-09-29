using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationCenterSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentMiddleNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdName",
                table: "Students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("UPDATE \"Students\" SET \"SecondName\" = 'ياسر', \"ThirdName\" = 'مصطفى' WHERE \"SecondName\" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ThirdName",
                table: "Students");
        }
    }
}
