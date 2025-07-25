namespace StreamNest.Domain.Entities.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        // Link to Video
        public Guid VideoId { get; set; }
        public Video Video { get; set; } = null!;

        // Link to User (Liker)
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        // Metadata
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}