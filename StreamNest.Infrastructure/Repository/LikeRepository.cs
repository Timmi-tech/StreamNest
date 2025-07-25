using Microsoft.EntityFrameworkCore;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Infrastructure.Repository
{
    public class LikeRepository : RepositoryBase<Like>, ILikeRepository
    {
        public LikeRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public void CreateLike(Like like) => Create(like);
        public void DeleteLike(Like like) => Delete(like);
        public async Task<IEnumerable<Like>> GetLikesAsync(Guid videoId, bool trackChanges)
        {
            return await FindByCondition(l => l.VideoId == videoId, trackChanges)
                .Include(l => l.User)
                .OrderByDescending(l => l.LikedAt)
                .ToListAsync();
        }
        public async Task<Like> GetLikeAsync(Guid videoId, string userId, bool trackChanges)
        {
            return await FindByCondition(l => l.VideoId == videoId && l.UserId == userId, trackChanges)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetLikeCountAsync(Guid videoId, bool trackChanges)
        {
            return await FindByCondition(l => l.VideoId == videoId, trackChanges)
                .CountAsync();
        }
    }
        
}