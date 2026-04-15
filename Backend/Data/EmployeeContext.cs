using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// Represents the Entity Framework Core database context for employee data.
/// </summary>
/// <param name="options">The options used to configure this context.</param>
public class EmployeeContext(DbContextOptions<EmployeeContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the employee records in the database.
    /// </summary>
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Department> Departments { get; set; }

    /// <summary>
    /// Configures the entity model for the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(department => department.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(department => department.Name).IsUnique();

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Departments_Name_NotEmpty",
                    "LEN(LTRIM(RTRIM([Name]))) > 0");
            });
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(employee => employee.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(employee => employee.Position)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(employee => employee.Salary)
                .HasPrecision(9, 2);

            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Employees_Name_NotEmpty",
                    "LEN(LTRIM(RTRIM([Name]))) > 0");

                table.HasCheckConstraint(
                    "CK_Employees_Position_NotEmpty",
                    "LEN(LTRIM(RTRIM([Position]))) > 0");

                table.HasCheckConstraint(
                    "CK_Employees_Salary_NonNegative",
                    "[Salary] IS NULL OR [Salary] >= 0");
            });
        });
    }
}
