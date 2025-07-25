namespace StreamNest.Application.DTOs
{
    public record CreateCommentDto
    {
        public string Content { get; init; } = string.Empty;
        public Guid VideoId { get; init; }
    }
    public record CommentResponseDto
    {
        public Guid Id { get; init; }
        public string Content { get; init; } = string.Empty;
        public Guid VideoId { get; init; }
        public string UserProfileId { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}