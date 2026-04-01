using Backend.Models;

namespace Backend.Services;

public interface IEmployeeService
{
    Task<(IEnumerable<Employee> Items, int TotalPages)> GetAllAsync
        (
        int pageNumber,
        int pageSize,
        string? sortBySalary,
        string? sortByName,
        string? department,
        string? position,
        string? searchTerm
        );
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee?> UpdateAsync(int id, Employee updatedEmployee);
    Task<bool> DeleteAsync(int id);
}
