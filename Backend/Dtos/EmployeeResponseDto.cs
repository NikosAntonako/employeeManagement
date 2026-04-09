namespace Backend.Dtos;

/// <summary>
/// Represents the data transfer object containing employee details for API responses.
/// </summary>
public sealed record EmployeeResponseDto(
    int Id,
    string Name,
    string Position,
    string Department,
    decimal? Salary);
