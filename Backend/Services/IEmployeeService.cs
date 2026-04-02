using Backend.Dtos;
using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<(IEnumerable<Employee> Items, int TotalPages)> GetAllAsync
        (
        EmployeeQueryDto request);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee?> UpdateAsync(int id, Employee updatedEmployee);
    Task<bool> DeleteAsync(int id);
}
