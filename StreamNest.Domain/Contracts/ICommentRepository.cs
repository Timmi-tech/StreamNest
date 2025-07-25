using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsByVideoIdAsync(Guid videoId, bool trackChanges);
        void DeleteComment(Comment comment);
        Task<Comment?> GetCommentByIdAsync(Guid commentId, bool trackChanges);
    }
}