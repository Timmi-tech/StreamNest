using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        void DeleteComment(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsByVideoIdAsync(Guid videoId, bool trackChanges);
        Task<Comment?> GetCommentByIdAsync(Guid commentId, bool trackChanges);
    }
}