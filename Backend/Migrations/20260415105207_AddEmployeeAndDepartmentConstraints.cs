using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAndDepartmentConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Employees_Name_NotEmpty",
                table: "Employees",
                sql: "LEN(LTRIM(RTRIM([Name]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employees_Position_NotEmpty",
                table: "Employees",
                sql: "LEN(LTRIM(RTRIM([Position]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employees_Salary_NonNegative",
                table: "Employees",
                sql: "[Salary] IS NULL OR [Salary] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Departments_Name_NotEmpty",
                table: "Departments",
                sql: "LEN(LTRIM(RTRIM([Name]))) > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Employees_Name_NotEmpty",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employees_Position_NotEmpty",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employees_Salary_NonNegative",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Departments_Name_NotEmpty",
                table: "Departments");
        }
    }
}
