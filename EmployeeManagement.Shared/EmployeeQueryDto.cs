using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Shared;

/// <summary>
/// Represents a data transfer object containing basic information about an employee, including name, position,
/// department, and salary.
/// </summary>
public sealed record EmployeeQueryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;

    [RegularExpression(@"^(?i)(asc|desc)$", ErrorMessage = "SortBySalary must be either 'asc' or 'desc'.")]
    public string? SortBySalary { get; set; }

    [RegularExpression(@"^(?i)(asc|desc)$", ErrorMessage = "SortByName must be either 'asc' or 'desc'.")]
    public string? SortByName { get; set; }

    [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
    public string? Department { get; set; }

    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Position must contain only letters and spaces.")]
    public string? Position { get; set; }

    [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters.")]
    public string? SearchTerm { get; set; }
}