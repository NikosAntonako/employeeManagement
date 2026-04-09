using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

/// <summary>
/// Represents the data required to create a department.
/// </summary>
public sealed record CreateDepartmentInput
{
    [Required(ErrorMessage = "Department name is required.")]
    [MinLength(1, ErrorMessage = "Department name cannot be empty.")]
    [StringLength(50, ErrorMessage = "Department name cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department name must contain only letters and spaces.")]
    public required string Name { get; set; }
}
