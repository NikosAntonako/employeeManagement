using EmployeeManagement.Shared;

namespace Backend.Services;

/// <summary>
/// Defines operations for querying departments.
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// Retrieves all departments ordered by name.
    /// </summary>
    /// <returns>The department list.</returns>
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync();
}
