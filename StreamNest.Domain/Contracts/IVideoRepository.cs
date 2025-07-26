using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface IVideoRepository
    {
        void CreateVideo(Video video);
        Task<IEnumerable<Video>> GetAllVideosAsync(bool trackChanges, int pageNumber = 1, int pageSize = 20);
        Task<Video> GetVideoByIdAsync(Guid videoPostId, bool trackChanges);
        Task<IEnumerable<Video>> GetVideosByUserIdAsync(string creatorId, bool trackChanges);
        Task<IEnumerable<Video>> SearchVideosAsync(string? query, Genre? genre, int? year);
        void DeletePost(Video video);
        
    }
}