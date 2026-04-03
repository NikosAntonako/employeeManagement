namespace Frontend.Models;

public class PagedResult
{
    public IReadOnlyCollection<EmployeeViewModel> Items { get; set; } = [];
    public int TotalPages { get; set; }
}
