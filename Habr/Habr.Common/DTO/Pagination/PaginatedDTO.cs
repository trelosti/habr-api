namespace Habr.Common.DTO.Pagination
{
    public class PaginatedDTO<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1 && CurrentPage <= TotalPages + 1;
        public bool HasNextPage => CurrentPage >= 0 && CurrentPage < TotalPages;
        public List<T> Items { get; set; } = new List<T>();
    }
}
