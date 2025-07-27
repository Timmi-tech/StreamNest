using StreamNest.Application.DTOs;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.Services.Contracts
{
    public interface IVideoPostService
    {
        Task<VideoDto> CreateVideoAsync(CreateVideoDto createVideoDto, string userId);
        Task<IEnumerable<VideoDto>> GetAllVideosAsync(bool trackChanges);
        Task<VideoDto?> GetVideoByIdAsync(Guid videoPostId, bool trackChanges);
        Task<IEnumerable<VideoDto>> GetVideosByUserIdAsync(string creatorId, bool trackChanges);
        Task<List<VideoDto>> SearchVideosAsync(string? query, Genre? genre, int? year);
        Task DeleteVideoAsync(Guid videoPostId, string userId);
    }
}