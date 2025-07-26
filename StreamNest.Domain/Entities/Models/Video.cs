using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamNest.Domain.Entities.Models
{
    public class Video
    {
        [Key]
        public Guid Id { get; set; }

        // Core content
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Classification
        public Genre Genre { get; set; }
        public AgeRating AgeRating { get; set; }
        [Range(1900, 2100)]
        public int VideoYear { get; set; }

        // Tags (as many-to-many)
        public List<VideoTag> VideoTags { get; set; } = new();

        // Video data
        public string VideoUrl { get; set; } = string.Empty;  // Cloudinary URL
        public string ThumbnailUrl { get; set; } = string.Empty; // Cloudinary URL for thumbnail
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        // 🔥 Link to User (Creator only)
        public string CreatorId { get; set; } = string.Empty;

        [ForeignKey(nameof(CreatorId))]
        public User Creator { get; set; } = null!;

        // 🔥 Link to Comments
        public List<Comment> Comments { get; set; } = new();
        // 🔥 Link to Likes
        public List<Like> Likes { get; set; } = new();

    }
}