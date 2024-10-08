using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services
{
    public class RatingService : IRatingService
    {
        private readonly DataContext _dataContext;

        public RatingService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task ComputePostsRatingsAsync()
        {
            await _dataContext.Posts
                .Include(x => x.PostRates)
                .Where(x => x.IsPublished && x.PostRates.Count() > 0)
                .Select(x => new { x.Rating, Sum = x.PostRates.Sum(x => x.Value), Count = (double)x.PostRates.Count() })
                .ExecuteUpdateAsync(x => x.SetProperty(
                    p => p.Rating,
                    p => Math.Truncate(p.Sum / p.Count * 100) / 100));
        }
    }
}
