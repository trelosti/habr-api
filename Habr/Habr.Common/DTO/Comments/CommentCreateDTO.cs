namespace Habr.Common.DTO.Comments
{
    public class CommentCreateDTO
    {
        public string Text { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
