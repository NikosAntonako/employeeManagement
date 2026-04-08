namespace Backend.Dtos;

/// <summary>
/// Represents a paged result containing a collection of employee responses and the total number of pages available.
/// </summary>
public sealed record PagedResultDto(
    IReadOnlyCollection<EmployeeResponseDto> Items,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize);