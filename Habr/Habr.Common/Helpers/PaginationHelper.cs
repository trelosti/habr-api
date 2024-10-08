namespace Habr.Common.Helpers
{
    public static class PaginationHelper
    {
        public static int CountTotalPages(int totalPages, int pageSize)
        {
            return pageSize == 0 ? 0 : (int)Math.Ceiling(totalPages / (double)pageSize);
        }
    }
}
