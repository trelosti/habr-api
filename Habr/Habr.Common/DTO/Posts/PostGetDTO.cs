using Habr.Common.DTO.Comments;

namespace Habr.Common.DTO.Posts
{
    public class PostGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime? PublishedAt { get; set; }
        public double? Rating { get; set; }
        public List<CommentGetDTO> Comments { get; set; }
    }
}
