using Backend.Dtos;

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

    /// <summary>
    /// Creates a new department.
    /// </summary>
    /// <param name="department">The department data to create.</param>
    /// <returns>The created department.</returns>
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto department);
}
