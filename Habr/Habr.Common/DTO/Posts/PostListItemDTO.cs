namespace Habr.Common.DTO.Posts
{
    public class PostListItemDTO
    {
        public string Title { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime? PublishedAt { get; set; }
        public double? Rating { get; set; }
    }
}
