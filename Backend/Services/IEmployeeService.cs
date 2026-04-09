using Backend.Dtos;

namespace Backend.Services;

/// <summary>
/// Defines methods for managing employee records.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves all employees
    /// </summary>
    Task<PagedResultDto> GetAllAsync(EmployeeQueryDto request);
    Task<EmployeeResponseDto> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee);
    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeDto updatedEmployee);
    Task DeleteAsync(int id);
}
