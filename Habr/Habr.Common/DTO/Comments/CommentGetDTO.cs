using Habr.Common.DTO.Users;

namespace Habr.Common.DTO.Comments
{
    public class CommentGetDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public UserInfoDTO User { get; set; }
        public int ParentCommentId {  get; set; }
    }
}
