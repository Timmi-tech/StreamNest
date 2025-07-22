using StreamNest.Domain.Entities.Models;

namespace StreamNest.Domain.Contracts
{
    public interface IUserProfileRepository
    {
        Task<User?> GetUserProfileAsync(string userId, bool trackChanges);
    }
}