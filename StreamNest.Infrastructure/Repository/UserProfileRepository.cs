using Microsoft.EntityFrameworkCore;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Infrastructure.Repository
{
    public class UserProfileRepository : RepositoryBase<User>, IUserProfileRepository
    {
        public UserProfileRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<User?> GetUserProfileAsync(string userId, bool trackChanges) => await FindByCondition(c => c.Id.Equals(userId), trackChanges).SingleOrDefaultAsync();

        
    }
}