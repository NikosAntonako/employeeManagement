using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// Represents the database context for the Employee Management application.
/// This class is used to configure and interact with the database using Entity Framework Core.
/// </summary>
public class EmployeeContext(DbContextOptions<EmployeeContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .Property(employee => employee.Salary)
            .HasPrecision(18, 2);
    }
}
