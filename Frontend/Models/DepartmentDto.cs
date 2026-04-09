namespace Frontend.Models;

/// <summary>
/// Represents a department returned by the backend API.
/// </summary>
public sealed record DepartmentDto(
    int Id,
    string Name
);
