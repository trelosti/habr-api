using Habr.Common.DTO.Users;

namespace Habr.Common.DTO.Posts
{
    public class PostListItemAuthorDTO
    {
        public string Title { get; set; }
        public DateTime? PublishedAt { get; set; }
        public PostAuthorDTO Author { get; set; }
        public double? Rating { get; set; }
    }
}
