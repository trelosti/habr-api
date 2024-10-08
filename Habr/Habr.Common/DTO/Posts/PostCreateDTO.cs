namespace Habr.Common.DTO.Posts
{
    public class PostCreateDTO
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPublished { get; set; }
    }
}
