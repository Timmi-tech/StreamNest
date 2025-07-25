namespace StreamNest.Domain.Entities.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        // Core content
        public string Content { get; set; } = string.Empty;

        // Link to Video
        public Guid VideoId { get; set; }
        public Video Video { get; set; } = null!;

        // Link to User (Commenter)
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}