namespace Frontend.Models;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int TotalPages { get; set; }
}
