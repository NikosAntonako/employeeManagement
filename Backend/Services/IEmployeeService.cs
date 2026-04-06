using Backend.Dtos;

namespace Backend.Services;

/// <summary>
/// Defines methods for managing employee records, including retrieval, creation, updating, and deletion operations.
/// </summary>
/// <remarks>Implementations of this interface should ensure thread safety if used in multi-threaded scenarios.
/// Methods are asynchronous and may involve database or external service calls.</remarks>
public interface IEmployeeService
{
    /// <summary>
    /// retrieeves all employees
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PagedResultDto> GetAllAsync(EmployeeQueryDto request);
    Task<EmployeeResponseDto> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee);
    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeDto updatedEmployee);
    Task DeleteAsync(int id);
}
