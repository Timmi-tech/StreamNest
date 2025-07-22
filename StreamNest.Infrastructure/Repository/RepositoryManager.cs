using StreamNest.Domain.Contracts;

namespace StreamNest.Infrastructure.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IUserProfileRepository>?_userProfileRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext ?? throw new ArgumentNullException(nameof(repositoryContext));
            _userProfileRepository = new Lazy<IUserProfileRepository>(() => new UserProfileRepository(repositoryContext));
        }

        public IUserProfileRepository User => _userProfileRepository.Value;

        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}