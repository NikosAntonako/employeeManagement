namespace Backend.Dtos;

/// <summary>
/// Represents the data transfer object containing employee details for API responses.
/// </summary>
/// <remarks>
/// This DTO is typically used to return employee information from service or controller methods. It is
/// immutable and intended for read-only scenarios.
/// </remarks>
public sealed record EmployeeResponseDto(
    int Id,
    string Name,
    string Position,
    string Department,
    decimal? Salary);
