using Backend.Models;

namespace Backend.Services;

/// <summary>
/// Defines methods for managing the lifecycle of departments, including retrieval, creation, and conditional deletion
/// operations.
/// </summary>
public interface IDepartmentLifecycle
{
    /// <summary>
    /// Retrieves an existing department by name or creates a new one if it does not exist.
    /// </summary>
    /// <param name="departmentName">The name of the department to retrieve or create. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the existing or newly created
    /// department.</returns>
    Task<Department> GetOrCreateAsync(string departmentName);

    /// <summary>
    /// Deletes the specified department if it is not associated with any employees, optionally excluding a specific
    /// employee from the check.
    /// </summary>
    /// <param name="departmentId">The unique identifier of the department to evaluate for deletion.</param>
    /// <param name="excludedEmployeeId">The identifier of an employee to exclude from the usage check. If specified, this employee will not be
    /// considered when determining if the department is unused. Can be null.</param>
    /// <returns>A task that represents the asynchronous delete operation. The task completes when the operation has finished.</returns>
    Task DeleteIfUnusedAsync(int departmentId, int? excludedEmployeeId = null);
}
