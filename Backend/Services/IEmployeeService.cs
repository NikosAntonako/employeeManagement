using Backend.Dtos;

namespace Backend.Services;

/// <summary>
/// Defines operations for querying and managing employee records.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves a paged list of employees that match the supplied filtering, sorting, and paging criteria.
    /// </summary>
    /// <param name="request">The query options used to filter, sort, and paginate the result set.</param>
    /// <returns>A paged result containing the matched employees.</returns>
    Task<PagedResultDto> GetAllAsync(EmployeeQueryDto request);

    /// <summary>
    /// Retrieves a single employee by identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve.</param>
    /// <returns>The matched employee.</returns>
    Task<EmployeeResponseDto> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new employee record.
    /// </summary>
    /// <param name="employee">The employee data to create.</param>
    /// <returns>The newly created employee.</returns>
    Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee);

    /// <summary>
    /// Updates an existing employee record.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="updatedEmployee">The updated employee data.</param>
    /// <returns>The updated employee.</returns>
    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeDto updatedEmployee);

    /// <summary>
    /// Deletes an employee record.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete.</param>
    Task DeleteAsync(int id);
}
