using Habr.Common.Enums;

namespace Habr.DataAccess.Entities
{
    public class User
    {
        public User() 
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public UserRole UserRole { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
