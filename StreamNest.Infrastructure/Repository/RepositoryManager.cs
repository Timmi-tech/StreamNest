using StreamNest.Domain.Contracts;

namespace StreamNest.Infrastructure.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly Lazy<IUserProfileRepository>?_userProfileRepository;
        private readonly Lazy<IVideoRepository>? _videoRepository;
        private readonly Lazy<ICommentRepository>? _commentRepository;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext ?? throw new ArgumentNullException(nameof(repositoryContext));
            _userProfileRepository = new Lazy<IUserProfileRepository>(() => new UserProfileRepository(repositoryContext));
            _videoRepository = new Lazy<IVideoRepository>(() => new VideoRepository(repositoryContext));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(repositoryContext));
        }

        public IUserProfileRepository User => _userProfileRepository.Value;
        public IVideoRepository Video => _videoRepository.Value;
        public ICommentRepository Comment => _commentRepository.Value;

        public async Task SaveAsync()
        {
            await _repositoryContext.SaveChangesAsync();
        }
    }
}