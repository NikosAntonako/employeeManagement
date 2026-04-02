using Backend.Common;
using Backend.Dtos;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<Result<PagedResultDto<EmployeeResponseDto>>> GetAllAsync(EmployeeQueryDto request);
    Task<Result<EmployeeResponseDto>> GetByIdAsync(int id);
    Task<Result<EmployeeResponseDto>> CreateAsync(EmployeeDto employee);
    Task<Result<EmployeeResponseDto>> UpdateAsync(int id, EmployeeDto updatedEmployee);
    Task<Result<object>> DeleteAsync(int id);
}
