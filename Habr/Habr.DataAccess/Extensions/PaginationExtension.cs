using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Extensions
{
    public static class PaginationExtension
    {
        public static async Task<Tuple<List<T>, int>> PaginateAsync<T>(this IQueryable<T> source, int pageSize, int pageNumber)
        {
            if (pageSize > 0 && pageNumber > 0)
            {
                var result =  await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                return new Tuple<List<T>, int>(result, await source.CountAsync());
            }

            return new Tuple<List<T>, int>(new List<T>(), await source.CountAsync());
        }
    }
}
