using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Provides employee-related business operations and data access.
/// </summary>
/// <remarks>
/// This service supports querying, creating, updating, and deleting employee records,
/// and maps persistence models to response DTOs returned by the API.
/// </remarks>
public class EmployeeService(EmployeeContext context) : IEmployeeService
{
    /// <summary>
    /// Maps an <see cref="Employee"/> entity to an <see cref="EmployeeResponseDto"/>.
    /// </summary>
    /// <param name="employee">The employee entity to map.</param>
    /// <returns>A response DTO containing the employee data.</returns>
    private static EmployeeResponseDto MapToResponse(Employee employee)
    {
        return new EmployeeResponseDto(
            employee.Id,
            employee.Name,
            employee.Position,
            employee.Department.Name,
            employee.Salary);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto> GetAllAsync(EmployeeQueryDto request)
    {
        IQueryable<Employee> query = context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department);

        if (!string.IsNullOrWhiteSpace(request.Department))
            query = query.Where(employee => employee.Department.Name == request.Department);

        if (!string.IsNullOrWhiteSpace(request.Position))
            query = query.Where(employee => employee.Position == request.Position);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm;

            query = query.Where(employee =>
                employee.Name.Contains(term) ||
                employee.Position.Contains(term) ||
                employee.Department.Name.Contains(term));
        }

        var sorted = false;

        if (!string.IsNullOrWhiteSpace(request.SortBySalary))
        {
            switch (request.SortBySalary.ToLowerInvariant())
            {
                case "asc":
                    query = query.OrderBy(employee => employee.Salary);
                    sorted = true;
                    break;
                case "desc":
                    query = query.OrderByDescending(employee => employee.Salary);
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
                        ? ((IOrderedQueryable<Employee>)query).ThenBy(employee => employee.Name)
                        : query.OrderBy(employee => employee.Name);
                    break;

                case "desc":
                    query = sorted
                        ? ((IOrderedQueryable<Employee>)query).ThenByDescending(employee => employee.Name)
                        : query.OrderByDescending(employee => employee.Name);
                    break;
            }
        }

        if (!sorted && string.IsNullOrWhiteSpace(request.SortByName))
            query = query.OrderBy(employee => employee.Id);

        var totalEmployees = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalEmployees / (double)request.PageSize);

        var employees = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var items = employees.Select(MapToResponse).ToList();

        return new PagedResultDto(items, totalEmployees, totalPages, request.PageNumber, request.PageSize);
    }

    /// <inheritdoc />
    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        // apper.Contrib version kept temporarily for reference.
        //
        // await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        // await connection.OpenAsync();
        //
        // var employee = await connection.GetAsync<Employee>(id);
        //
        // if (employee == null)
        //     throw new KeyNotFoundException($"Employee with id {id} not found.");
        //
        // return MapToResponse(employee);

        var employee = await context.Employees
            .AsNoTracking()
            .Include(currentEmployee => currentEmployee.Department)
            .FirstOrDefaultAsync(currentEmployee => currentEmployee.Id == id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {id} not found.");

        return MapToResponse(employee);
    }

    /// <inheritdoc />
    public async Task<EmployeeResponseDto> CreateAsync(EmployeeDto employee)
    {
        var department = await context.Departments.FindAsync(employee.DepartmentId);

        if (department == null)
            throw new KeyNotFoundException($"Department with id {employee.DepartmentId} not found.");

        var newEmployee = new Employee
        {
            Name = employee.Name,
            Position = employee.Position,
            DepartmentId = employee.DepartmentId,
            Department = department,
            Salary = employee.Salary
        };

        context.Employees.Add(newEmployee);
        await context.SaveChangesAsync();

        return MapToResponse(newEmployee);
    }

    /// <inheritdoc />
    public async Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeDto updatedEmployee)
    {
        var employee = await context.Employees
            .Include(currentEmployee => currentEmployee.Department)
            .FirstOrDefaultAsync(currentEmployee => currentEmployee.Id == id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {id} not found.");

        var department = await context.Departments.FindAsync(updatedEmployee.DepartmentId);

        if (department == null)
            throw new KeyNotFoundException($"Department with id {updatedEmployee.DepartmentId} not found.");

        employee.Name = updatedEmployee.Name;
        employee.Position = updatedEmployee.Position;
        employee.DepartmentId = updatedEmployee.DepartmentId;
        employee.Department = department;
        employee.Salary = updatedEmployee.Salary;

        await context.SaveChangesAsync();

        return MapToResponse(employee);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {id} not found.");

        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
    }
}
