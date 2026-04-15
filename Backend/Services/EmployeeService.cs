using Backend.Data;
using Backend.Models;
using EmployeeManagement.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Provides employee-related business operations and data access.
/// </summary>
/// <remarks>
/// This service supports querying, creating, updating, and deleting employee records,
/// and maps persistence models to response DTOs returned by the API.
/// </remarks>
public class EmployeeService(EmployeeContext context, IDepartmentLifecycle departmentLifecycle) : IEmployeeService
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

    /// <summary>
    /// Determines whether the specified exception represents a SQL Server unique constraint violation.
    /// </summary>
    /// <param name="exception">The exception to evaluate.</param>
    /// <returns><see langword="true"/> when the exception represents a unique constraint violation; otherwise, <see langword="false"/>.</returns>
    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.GetBaseException() is SqlException sqlException &&
               // 2601: duplicate key in a unique index
               // 2627: violation of a unique constraint or primary key
               (sqlException.Number == 2601 || sqlException.Number == 2627);
    }

    /// <summary>
    /// Trims the specified string value and ensures it is not null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string value to normalize and validate. Leading and trailing white-space characters are removed.</param>
    /// <param name="fieldName">The name of the field associated with the value. Used in the exception message if the value is invalid.</param>
    /// <returns>The trimmed string value if it is not null, empty, or white-space.</returns>
    /// <exception cref="ArgumentException">Thrown if the trimmed value is null, empty, or consists only of white-space characters.</exception>
    private static string NormalizeRequired(string value, string fieldName)
    {
        var normalizedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
            throw new ArgumentException($"{fieldName} is required.");

        return normalizedValue;
    }

    /// <summary>
    /// Creates a new employee record and returns the corresponding response data transfer object.
    /// </summary>
    /// <remarks>If the specified department does not exist, it is created as part of this
    /// operation.</remarks>
    /// <param name="employee">An object containing the details of the employee to create, including name, position, department name, and
    /// salary. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an EmployeeResponseDto with the
    /// details of the newly created employee.</returns>
    private async Task<EmployeeResponseDto> CreateCore(EmployeeDto employee)
    {
        var normalizedName = NormalizeRequired(employee.Name, "Name");
        var normalizedPosition = NormalizeRequired(employee.Position, "Position");
        var department = await departmentLifecycle.GetOrCreateAsync(employee.DepartmentName);

        var newEmployee = new Employee
        {
            Name = normalizedName,
            Position = normalizedPosition,
            Department = department,
            Salary = employee.Salary
        };

        context.Employees.Add(newEmployee);
        await context.SaveChangesAsync();

        return MapToResponse(newEmployee);
    }

    /// <summary>
    /// Updates the details of an existing employee with the specified values.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="updatedEmployee">An object containing the updated employee information. The department name, name, position, and salary fields
    /// are used to update the employee record.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an EmployeeResponseDto with the
    /// updated employee details.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no employee with the specified id exists.</exception>
    private async Task<EmployeeResponseDto> UpdateCore(int id, EmployeeDto updatedEmployee)
    {
        var employee = await context.Employees
            .Include(currentEmployee => currentEmployee.Department)
            .FirstOrDefaultAsync(currentEmployee => currentEmployee.Id == id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {id} not found.");

        var normalizedName = NormalizeRequired(updatedEmployee.Name, "Name");
        var normalizedPosition = NormalizeRequired(updatedEmployee.Position, "Position");
        var previousDepartmentId = employee.DepartmentId;
        var department = await departmentLifecycle.GetOrCreateAsync(updatedEmployee.DepartmentName);

        employee.Name = normalizedName;
        employee.Position = normalizedPosition;
        employee.Department = department;
        employee.Salary = updatedEmployee.Salary;

        if (previousDepartmentId != department.Id)
            await departmentLifecycle.DeleteIfUnusedAsync(previousDepartmentId, employee.Id);

        await context.SaveChangesAsync();

        return MapToResponse(employee);
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
        // Dapper.Contrib version kept temporarily for reference.
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
        try
        {
            return await CreateCore(employee);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            context.ChangeTracker.Clear();
            return await CreateCore(employee);
        }
    }

    /// <inheritdoc />
    public async Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeDto updatedEmployee)
    {
        try
        {
            return await UpdateCore(id, updatedEmployee);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            context.ChangeTracker.Clear();
            return await UpdateCore(id, updatedEmployee);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {id} not found.");

        var departmentId = employee.DepartmentId;

        context.Employees.Remove(employee);

        await departmentLifecycle.DeleteIfUnusedAsync(departmentId, id);

        await context.SaveChangesAsync();
    }
}
