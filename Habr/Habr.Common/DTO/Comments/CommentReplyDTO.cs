namespace Habr.Common.DTO.Comments
{
    public class CommentReplyDTO
    {
        public string Text { get; set; }
        // Временное поле, можно передавать, например, в полезной нагрузке токена
        public int UserId { get; set; }
        public int ParentCommentId { get; set; }
        public int PostId { get; set; }
    }
}
