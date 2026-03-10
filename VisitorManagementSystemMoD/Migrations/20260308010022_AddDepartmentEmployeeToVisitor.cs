using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitorManagementSystemMoD.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentEmployeeToVisitor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentEmployeeId",
                table: "Visitors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DepartmentEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsHighPriority = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentEmployees_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visitors_DepartmentEmployeeId",
                table: "Visitors",
                column: "DepartmentEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEmployees_UserId",
                table: "DepartmentEmployees",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitors_DepartmentEmployees_DepartmentEmployeeId",
                table: "Visitors",
                column: "DepartmentEmployeeId",
                principalTable: "DepartmentEmployees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitors_DepartmentEmployees_DepartmentEmployeeId",
                table: "Visitors");

            migrationBuilder.DropTable(
                name: "DepartmentEmployees");

            migrationBuilder.DropIndex(
                name: "IX_Visitors_DepartmentEmployeeId",
                table: "Visitors");

            migrationBuilder.DropColumn(
                name: "DepartmentEmployeeId",
                table: "Visitors");
        }
    }
}
