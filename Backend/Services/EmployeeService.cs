using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService : IEmployeeService
{
    private readonly EmployeeContext _context;
    private readonly IConfiguration _configuration;

    public EmployeeService(EmployeeContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<PagedResultDto<EmployeeResponseDto>> GetAllAsync(EmployeeQueryDto request)
    {
        IQueryable<Employee> query = _context.Employees.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Department))
            query = query.Where(e => e.Department == request.Department);

        if (!string.IsNullOrWhiteSpace(request.Position))
            query = query.Where(e => e.Position == request.Position);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var lowerTerm = request.SearchTerm.ToLower();

            query = query.Where(employee =>
                employee.Name.ToLower().StartsWith(lowerTerm) ||
                employee.Position.ToLower().StartsWith(lowerTerm) ||
                employee.Department.ToLower().StartsWith(lowerTerm));
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

        var items = employees.Select(MapToResponse).ToList();

        return new PagedResultDto<EmployeeResponseDto>
        {
            Items = items,
            TotalPages = totalPages
        };
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        const string sql = @"SELECT Id, Name, Position, Department, Salary FROM Employees WHERE Id = @Id";
        var employee = await connection.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
        return employee == null ? null : MapToResponse(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee)
    {
        var newEmployee = new Employee
        {
            Name = employee.Name,
            Position = employee.Position,
            Department = employee.Department,
            Salary = employee.Salary
        };

        _context.Employees.Add(newEmployee);
        await _context.SaveChangesAsync();

        return MapToResponse(newEmployee);
    }

    public async Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeDto updatedEmployee)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return null;

        employee.Name = updatedEmployee.Name;
        employee.Position = updatedEmployee.Position;
        employee.Department = updatedEmployee.Department;
        employee.Salary = updatedEmployee.Salary;

        await _context.SaveChangesAsync();

        return MapToResponse(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }

    private static EmployeeResponseDto MapToResponse(Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Position = employee.Position,
            Department = employee.Department,
            Salary = employee.Salary
        };
    }
}
