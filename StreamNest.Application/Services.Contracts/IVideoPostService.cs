using StreamNest.Application.DTOs;

namespace StreamNest.Application.Services.Contracts
{
    public interface IVideoPostService
    {
        Task<VideoDto> CreateVideoAsync(CreateVideoDto createVideoDto, string userId);
        Task<IEnumerable<VideoDto>> GetAllVideosAsync(bool trackChanges);
        Task<VideoDto?> GetVideoByIdAsync(Guid videoPostId, bool trackChanges);
        Task<IEnumerable<VideoDto>> GetVideosByUserIdAsync(string creatorId, bool trackChanges);
        Task DeleteVideoAsync(Guid videoPostId, string userId);
    }
}