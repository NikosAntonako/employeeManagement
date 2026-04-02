using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public sealed class EmployeeQueryDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;

        public string? SortBySalary { get; set; }
        public string? SortByName { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? SearchTerm { get; set; }
    }
}
