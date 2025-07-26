using Microsoft.AspNetCore.Http;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.DTOs
{


    public record CreateVideoDto
    {
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public Genre Genre { get; init; }
        public AgeRating AgeRating { get; init; }
        public int VideoYear { get; init; } = DateTime.UtcNow.Year; 
        public IFormFile? VideoFile { get; init; }
        public List<string> Tags { get; init; } = new List<string>();
    }
    public record VideoDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Genre { get; init; } = string.Empty;
        public string AgeRating { get; init; } = string.Empty;
        public string VideoUrl { get; init; } = string.Empty;
        public string ThumbnailUrl { get; init; } = string.Empty;
        public int VideoYear { get; init; } = DateTime.UtcNow.Year;
        public DateTime UploadedAt { get; init; } = DateTime.UtcNow;
        // public DateTime UploadedAt { get; init; }
        public string UserId { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public List<string> Tags { get; init; } = new List<string>();
    }
}