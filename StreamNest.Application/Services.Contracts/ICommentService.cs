using StreamNest.Application.DTOs;

namespace StreamNest.Application.Services.Contracts
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateCommentAsync(CreateCommentDto createCommentDto, string userProfileId);
        Task<List<CommentResponseDto>> GetCommentsByVideoIdAsync(Guid videoId, bool trackChanges);
        Task DeleteCommentAsync(Guid commentId, string userProfileId, bool trackChanges);
    }
}