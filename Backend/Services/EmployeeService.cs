using Backend.Data;
using Backend.Common;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class EmployeeService(EmployeeContext context) : IEmployeeService
{
    private readonly EmployeeContext _context = context;

    public async Task<Result<PagedResultDto<EmployeeResponseDto>>> GetAllAsync(EmployeeQueryDto request)
    {
        IQueryable<Employee> query = _context.Employees.AsNoTracking();

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

        var items = employees.Select(MapToResponse).ToList();

        return Result<PagedResultDto<EmployeeResponseDto>>.CreateSuccess(
            new PagedResultDto<EmployeeResponseDto>
            {
                Items = items,
                TotalPages = totalPages
            },
            "Employees retrieved successfully.");
    }

    public async Task<Result<EmployeeResponseDto>> GetByIdAsync(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee == null
            ? Result<EmployeeResponseDto>.Failure("Employee not found.", StatusCodes.Status404NotFound)
            : Result<EmployeeResponseDto>.CreateSuccess(MapToResponse(employee), "Employee retrieved successfully.");
    }

    public async Task<Result<EmployeeResponseDto>> CreateAsync(EmployeeDto employee)
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

        return Result<EmployeeResponseDto>.CreateSuccess(
            MapToResponse(newEmployee),
            "Employee created successfully.",
            StatusCodes.Status201Created);
    }

    public async Task<Result<EmployeeResponseDto>> UpdateAsync(int id, EmployeeDto updatedEmployee)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return Result<EmployeeResponseDto>.Failure("Employee not found.", StatusCodes.Status404NotFound);

        employee.Name = updatedEmployee.Name;
        employee.Position = updatedEmployee.Position;
        employee.Department = updatedEmployee.Department;
        employee.Salary = updatedEmployee.Salary;

        await _context.SaveChangesAsync();

        return Result<EmployeeResponseDto>.CreateSuccess(MapToResponse(employee), "Employee updated successfully.");
    }

    public async Task<Result<object>> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return Result<object>.Failure("Employee not found.", StatusCodes.Status404NotFound);

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return Result<object>.CreateSuccess(new { Id = id }, "Employee deleted successfully.");
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
