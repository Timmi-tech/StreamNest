namespace StreamNest.Application.DTOs
{
    public record CreateLikeDto
    {
        public Guid VideoId { get; init; }
    }

    public record LikeResponseDto
    {
        public Guid VideoId { get; init; }
        public string UserId { get; init; } = string.Empty;
        public bool IsLiked { get; init; }
        public int TotalLikes { get; init; }
        public DateTime LikedAt { get; init; }
    }
}