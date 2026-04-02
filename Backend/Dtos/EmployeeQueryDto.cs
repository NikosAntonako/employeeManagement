using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{

    /// <summary>
    /// Represents the set of query parameters used to filter, sort, and paginate employee search results.
    /// </summary>
    /// <remarks>This data transfer object is typically used to encapsulate user-supplied criteria for
    /// employee list queries, such as pagination settings, sorting options, and filtering by department or position.
    /// All properties are optional except for page number and page size, which must be positive integers.</remarks>
    public sealed class EmployeeQueryDto
    {
        [Range(1, 100)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SortBySalary { get; set; }
        public string? SortByName { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? SearchTerm { get; set; }
    }
}
