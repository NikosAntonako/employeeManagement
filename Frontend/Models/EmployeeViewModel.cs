namespace Frontend.Models;

/// <summary>
/// Represents an employee with identifying and organizational information for use in presentation layers.
/// </summary>
/// <remarks>This view model is typically used to transfer employee data between the business logic and user
/// interface layers. It contains only the fields necessary for display or editing in UI scenarios and omits
/// domain-specific behavior.</remarks>
public class EmployeeViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal? Salary { get; set; }
}
