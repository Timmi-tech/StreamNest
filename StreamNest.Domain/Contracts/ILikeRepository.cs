using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Like>> GetLikesAsync(Guid videoId, bool trackChanges);
        Task<Like> GetLikeAsync(Guid videoId, string userId, bool trackChanges);
        Task<int> GetLikeCountAsync(Guid videoId, bool trackChanges);
        void CreateLike(Like like);
        void DeleteLike(Like like);
        
    }
}