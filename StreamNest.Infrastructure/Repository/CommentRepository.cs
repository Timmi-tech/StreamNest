using Microsoft.EntityFrameworkCore;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Infrastructure.Repository
{
    public class CommentRepository : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public void AddComment(Comment comment) => Create(comment);

        public async Task<IEnumerable<Comment>> GetCommentsByVideoIdAsync(Guid videoId, bool trackChanges)
        {
            return await FindByCondition(c => c.VideoId == videoId, trackChanges)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        public async Task<Comment?> GetCommentByIdAsync(Guid commentId, bool trackChanges)
        {
            return await FindByCondition(c => c.Id == commentId, trackChanges).FirstOrDefaultAsync();
        }

        public void DeleteComment(Comment comment)
        {
            Delete(comment);
        }

    }
}