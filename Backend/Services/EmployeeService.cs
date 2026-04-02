using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService(EmployeeContext context) : IEmployeeService
{
    private readonly EmployeeContext _context = context;

    public async Task<(IEnumerable<Employee> Items, int TotalPages)> GetAllAsync(EmployeeQueryDto request)
    {
        IQueryable<Employee> query = _context.Employees;

        if (!string.IsNullOrWhiteSpace(request.Department))
            query = query.Where(e => e.Department == request.Department);

        if (!string.IsNullOrWhiteSpace(request.Position))
            query = query.Where(e => e.Position == request.Position);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var lowerTerm = request.SearchTerm.ToLower();

            query = query.Where(e =>
                e.Name.ToLower().StartsWith(lowerTerm) ||
                e.Position.ToLower().StartsWith(lowerTerm) ||
                e.Department.ToLower().StartsWith(lowerTerm));
        }

        var sorted = false;

        if (!string.IsNullOrWhiteSpace(request.SortBySalary))
        {
            switch (request.SortBySalary.ToLowerInvariant())
            {
                case "asc":
                    query = query.OrderBy(e => e.Salary);
                    sorted = true;
                    break;
                case "desc":
                    query = query.OrderByDescending(e => e.Salary);
                    sorted = true;
                    break;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SortByName))
        {
            switch (request.SortByName.ToLowerInvariant())
            {
                case "asc":
                    query = sorted
                        ? ((IOrderedQueryable<Employee>)query).ThenBy(e => e.Name)
                        : query.OrderBy(e => e.Name);
                    break;

                case "desc":
                    query = sorted
                        ? ((IOrderedQueryable<Employee>)query).ThenByDescending(e => e.Name)
                        : query.OrderByDescending(e => e.Name);
                    break;
            }
        }

        if (!sorted && string.IsNullOrWhiteSpace(request.SortByName))
            query = query.OrderBy(e => e.Id);

        var totalEmployees = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalEmployees / (double)request.PageSize);

        var employees = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

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
