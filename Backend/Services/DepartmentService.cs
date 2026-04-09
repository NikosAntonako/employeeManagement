using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Provides department-related read operations.
/// </summary>
public class DepartmentService(EmployeeContext context) : IDepartmentService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync()
    {
        return await context.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .Select(department => new DepartmentDto(
                department.Id,
                department.Name))
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto department)
    {
        var existingDepartment = await context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(currentDepartment => currentDepartment.Name == department.Name);

        if (existingDepartment != null)
            return new DepartmentDto(existingDepartment.Id, existingDepartment.Name);

        var newDepartment = new Department
        {
            Name = department.Name
        };

        context.Departments.Add(newDepartment);
        await context.SaveChangesAsync();

        return new DepartmentDto(newDepartment.Id, newDepartment.Name);
    }
}
