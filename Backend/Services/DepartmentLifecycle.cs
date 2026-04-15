using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

/// <summary>
/// Applies department lifecycle rules used by employee workflows.
/// </summary>
public class DepartmentLifecycle(EmployeeContext context) : IDepartmentLifecycle
{
    /// <summary>
    /// Trims leading and trailing whitespace and validates that the department name is not empty.
    /// </summary>
    /// <param name="departmentName">The department name to normalize.</param>
    /// <returns>The normalized department name.</returns>
    /// <exception cref="ArgumentException">Thrown when the department name is empty or whitespace.</exception>
    public static string NormalizeDepartmentName(string departmentName)
    {
        var normalizedDepartmentName = departmentName.Trim();

        if (string.IsNullOrWhiteSpace(normalizedDepartmentName))
            throw new ArgumentException("Department name is required.");

        return normalizedDepartmentName;
    }

    /// <summary>
    /// Resolves an existing department or creates a tracked one when it does not exist.
    /// </summary>
    /// <param name="departmentName">The department name to resolve.</param>
    /// <returns>The resolved department.</returns>
    public async Task<Department> GetOrCreateAsync(string departmentName)
    {
        var normalizedDepartmentName = NormalizeDepartmentName(departmentName);

        var existingDepartment = await context.Departments
            .FirstOrDefaultAsync(currentDepartment => currentDepartment.Name == normalizedDepartmentName);

        if (existingDepartment != null)
            return existingDepartment;

        var newDepartment = new Department
        {
            Name = normalizedDepartmentName
        };

        context.Departments.Add(newDepartment);
        return newDepartment;
    }

    /// <summary>
    /// Deletes a department when it is no longer referenced by any employee.
    /// </summary>
    /// <param name="departmentId">The department identifier to evaluate.</param>
    /// <param name="excludedEmployeeId">An optional employee identifier to exclude from the reference check.</param>
    public async Task DeleteIfUnusedAsync(int departmentId, int? excludedEmployeeId = null)
    {
        var hasOtherEmployees = await context.Employees.AnyAsync(currentEmployee =>
            currentEmployee.DepartmentId == departmentId &&
            (!excludedEmployeeId.HasValue || currentEmployee.Id != excludedEmployeeId.Value));

        if (hasOtherEmployees)
            return;

        var department = await context.Departments.FindAsync(departmentId);

        if (department != null)
            context.Departments.Remove(department);
    }
}
