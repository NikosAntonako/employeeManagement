using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// Represents the Entity Framework Core database context for managing employee data.
/// </summary>
/// <remarks>This context provides access to the Employees table and configures entity properties such as salary
/// precision. It is intended to be used with dependency injection in ASP.NET Core applications.</remarks>
/// <param name="options">The options to be used by the DbContext. These typically include configuration information such as the database
/// provider and connection string.</param>
public class EmployeeContext(DbContextOptions<EmployeeContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(employee => employee.Name).HasMaxLength(50);
            entity.Property(employee => employee.Position).HasMaxLength(50);
            entity.Property(employee => employee.Department).HasMaxLength(50);
            entity.Property(employee => employee.Salary).HasPrecision(9, 2);
        });
    }
}
