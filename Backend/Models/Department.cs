namespace Backend.Models;

public class Department
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Employee> Employees { get; set; } = [];
}
