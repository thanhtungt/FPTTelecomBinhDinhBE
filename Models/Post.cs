namespace FPTTelecomBE.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public string Category { get; set; } = string.Empty; // "khuyen-mai", "tin-tuc", "huong-dan"

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    }
}
