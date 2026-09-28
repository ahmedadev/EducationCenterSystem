using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationCenterSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAndPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teachers_NationalId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_NationalId",
                table: "Students");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Module = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Module", "Name" },
                values: new object[,]
                {
                    { 1, "صلاحية استعراض بيانات الطلاب", "Students", "Permissions.Students.Read" },
                    { 2, "صلاحية إضافة طالب جديد", "Students", "Permissions.Students.Create" },
                    { 3, "صلاحية تعديل بيانات الطلاب", "Students", "Permissions.Students.Update" },
                    { 4, "صلاحية حذف أو أرشفة الطلاب", "Students", "Permissions.Students.Delete" },
                    { 5, "صلاحية استعراض بيانات المدرسين", "Teachers", "Permissions.Teachers.Read" },
                    { 6, "صلاحية إضافة مدرس جديد", "Teachers", "Permissions.Teachers.Create" },
                    { 7, "صلاحية تعديل بيانات المدرسين", "Teachers", "Permissions.Teachers.Update" },
                    { 8, "صلاحية حذف أو أرشفة المدرسين", "Teachers", "Permissions.Teachers.Delete" },
                    { 9, "صلاحية استعراض سجلات الحضور", "Attendance", "Permissions.Attendance.Read" },
                    { 10, "صلاحية تسجيل حضور وغياب الطلاب", "Attendance", "Permissions.Attendance.Mark" },
                    { 11, "صلاحية استعراض الحسابات والمصروفات", "Finance", "Permissions.Finance.Read" },
                    { 12, "صلاحية تسجيل المدفوعات والاشتراكات المالية", "Finance", "Permissions.Finance.Manage" },
                    { 13, "صلاحية استعراض حسابات المستخدمين", "Users", "Permissions.Users.Read" },
                    { 14, "صلاحية إنشاء وإدارة المستخدمين", "Users", "Permissions.Users.Manage" },
                    { 15, "صلاحية إدارة الأدوار وتعيين الصلاحيات", "Roles", "Permissions.Roles.Manage" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "IsSystemRole", "Name" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "مسؤول النظام العام - يمتلك كافة الصلاحيات", true, "Admin" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 2, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 3, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 4, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 5, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 6, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 7, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 8, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 9, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 10, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 11, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 12, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 13, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 14, new Guid("00000000-0000-0000-0000-000000000001") },
                    { 15, new Guid("00000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_NationalId",
                table: "Teachers",
                column: "NationalId",
                unique: true,
                filter: "\"NationalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Students_NationalId",
                table: "Students",
                column: "NationalId",
                unique: true,
                filter: "\"NationalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_NationalId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_NationalId",
                table: "Students");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_NationalId",
                table: "Teachers",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_NationalId",
                table: "Students",
                column: "NationalId",
                unique: true);
        }
    }
}
