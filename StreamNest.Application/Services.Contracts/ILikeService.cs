using StreamNest.Application.DTOs;

namespace StreamNest.Application.Services.Contracts
{
    public interface ILikeService
    {
        Task<LikeResponseDto> ToggleLikeAsync(CreateLikeDto createLikeDto, string userId);
        Task<LikeResponseDto> GetLikeStatusAsync(Guid videoId, string userId);
    }
}