using Backend.Dtos;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<PagedResultDto> GetAllAsync(EmployeeQueryDto request);
    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee);
    Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeDto updatedEmployee);
    Task<bool> DeleteAsync(int id);
}
