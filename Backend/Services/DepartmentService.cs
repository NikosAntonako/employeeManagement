using Backend.Data;
using EmployeeManagement.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Provides department query and creation operations.
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
}
