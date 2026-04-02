namespace Backend.Dtos;

/// <summary>
/// Represents a paged result containing a collection of items and the total number of pages available.
/// </summary>
/// <remarks>Use this type to return paginated data from queries or APIs, providing both the current page's items
/// and the total number of pages for client-side navigation or display.</remarks>
/// <typeparam name="T">The type of the items contained in the paged result.</typeparam>
public sealed class PagedResultDto<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int TotalPages { get; init; }
}
