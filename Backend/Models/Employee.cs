namespace Backend.Models;

/// <summary>
/// Represents an Employee entity in the Employee Management application.
/// This class maps to the Employees table in the database.
/// </summary>
public class Employee
{
    // Id is being set server side from Controller automatically
    // Cannot be set by POST, safe to not be required
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Position { get; set; }
    public required string Department { get; set; }
    public decimal Salary { get; set; }
}
