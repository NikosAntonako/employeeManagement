namespace Backend.Dtos;

/// <summary>
/// Represents the data transfer object containing employee details for API responses.
/// </summary>
/// <remarks>This DTO is typically used to return employee information from service or controller methods. It is
/// immutable and intended for read-only scenarios.</remarks>
public sealed class EmployeeResponseDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public decimal? Salary { get; init; }
}
