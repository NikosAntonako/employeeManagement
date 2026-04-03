namespace Backend.Dtos;

/// <summary>
/// Represents a paged result containing a collection of employee responses and the total number of pages available.
/// </summary>
public sealed class PagedResultDto
{
    public IReadOnlyCollection<EmployeeResponseDto> Items { get; init; } = [];
    public int TotalPages { get; init; }
}
