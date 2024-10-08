namespace Habr.DataAccess.Entities
{
    public class Post
    {
        public Post() 
        { 
            Comments = new HashSet<Comment>();
            PostRates = new HashSet<PostRate>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPublished { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? PublicationDate { get; set; }
        public double? Rating { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostRate> PostRates { get; set; }
    }
}
