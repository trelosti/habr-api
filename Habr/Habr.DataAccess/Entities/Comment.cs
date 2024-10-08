﻿namespace Habr.DataAccess.Entities
{
    public class Comment
    {
        public Comment() 
        {
            Replies = new HashSet<Comment>();
        }
        public int Id { get; set; } 
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
