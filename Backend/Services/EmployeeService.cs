using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService(EmployeeContext context) : IEmployeeService
{
    private readonly EmployeeContext _context = context;

    public async Task<(IEnumerable<Employee> Items, int TotalPages)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? sortBySalary,
        string? sortByName,
        string? department,
        string? position,
        string? searchTerm
        )
    {
        IQueryable<Employee> query = _context.Employees;

        if (!string.IsNullOrEmpty(department))
            query = query.Where(e => e.Department == department);
        if (!string.IsNullOrEmpty(position))
            query = query.Where(e => e.Position == position);

        // Filter by search term (case-insensitive)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm}%";
            query = query.Where(e =>
                EF.Functions.Like(e.Name, pattern) ||
                EF.Functions.Like(e.Position, pattern) ||
                EF.Functions.Like(e.Department, pattern));
        }

        bool sorted = false;

        // Sort By Salary
        if (!string.IsNullOrEmpty(sortBySalary))
        {
            switch (sortBySalary.ToLowerInvariant())
            {
                case "asc":
                    query = query.OrderBy(e => e.Salary);
                    sorted = true;
                    break;
                case "desc":
                    query = query.OrderByDescending(e => e.Salary);
                    sorted = true;
                    break;
                default:
                    // No sorting applied if value is invalid
                    break;
            }
        }

        // Sort By Name
        if (!string.IsNullOrEmpty(sortByName))
        {
            switch (sortByName.ToLowerInvariant())
            {
                case "asc":
                    query = sorted ? ((IOrderedQueryable<Employee>)query).ThenBy(e => e.Name) : query.OrderBy(e => e.Name);
                    break;
                case "desc":
                    query = sorted ? ((IOrderedQueryable<Employee>)query).ThenByDescending(e => e.Name) : query.OrderByDescending(e => e.Name);
                    break;
                default:
                    // No sorting applied if value is invalid
                    break;
            }
        }
        if (!sorted && string.IsNullOrEmpty(sortByName))
            query = query.OrderBy(e => e.Id);

        var totalEmployees = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);
        var employees = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (employees, totalPages);
    }

    public async Task<Employee?> GetByIdAsync(int id) => await _context.Employees.FindAsync(id);

    public async Task<Employee> CreateAsync(Employee employee)
    {
        employee.Id = 0;
        _context.Employees.Add(employee);

        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee?> UpdateAsync(int id, Employee updatedEmployee)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null) return null;

        employee.Name = updatedEmployee.Name;
        employee.Position = updatedEmployee.Position;
        employee.Department = updatedEmployee.Department;
        employee.Salary = updatedEmployee.Salary;

        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null) return false;

        _context.Employees.Remove(employee);

        await _context.SaveChangesAsync();

        return true;
    }
}
