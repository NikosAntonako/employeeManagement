namespace Backend.Dtos;

/// <summary>
/// Represents a department returned by the API.
/// </summary>
public sealed record DepartmentDto(
    int Id,
    string Name
);
