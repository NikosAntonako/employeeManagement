namespace Frontend.Models;

/// <summary>
/// Represents a single page of results returned from a paginated query, including the items for the current page and
/// the total number of pages available.
/// </summary>
public class PagedResult
{
    public IReadOnlyCollection<EmployeeViewModel> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
