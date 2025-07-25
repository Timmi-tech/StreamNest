using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;
using StreamNest.Domain.Entities.ConfigurationsModels;
using CloudinaryDotNet;



namespace StreamNest.Application.Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IVideoService> _videoService;
        private readonly Lazy<IUserProfileService> _userProfileService;
        private readonly Lazy<IVideoPostService> _videoPostService;
        private readonly Lazy<ICommentService> _commentService;
        private readonly Lazy<ILikeService> _likeService;

        public ServiceManager
        (
            ILoggerManager logger,
            IRepositoryManager repositoryManager,
            UserManager<User> userManager,
            IOptions<JwtConfiguration> configuration,
            Cloudinary cloudinary
        )
        {
            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(logger, userManager, configuration));
            _videoService = new Lazy<IVideoService>(() => new VideoService(cloudinary));
            _userProfileService = new Lazy<IUserProfileService>(() => new UserProfileService(repositoryManager, logger));
            _videoPostService = new Lazy<IVideoPostService>(() => new VideoPostService(repositoryManager, logger, VideoService));
            _commentService = new Lazy<ICommentService>(() => new CommentService(repositoryManager, logger));
            _likeService = new Lazy<ILikeService>(() => new LikeService(repositoryManager, logger));
        }
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IVideoService VideoService => _videoService.Value;
        public IUserProfileService UserProfileService => _userProfileService.Value;
        public IVideoPostService VideoPostService => _videoPostService.Value;
        public ICommentService CommentService => _commentService.Value;
        public ILikeService LikeService => _likeService.Value;
    }
}