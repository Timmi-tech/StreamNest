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

        public ServiceManager
        (
            ILoggerManager logger,
            IRepositoryManager repositoryManager,
            UserManager<User> userManager,
            IOptions<JwtConfiguration> configuration,
            Cloudinary cloudinary
        )
        {
            _authenticationService = new Lazy<IAuthenticationService>(() =>
                new AuthenticationService(logger, userManager, configuration));
            _videoService = new Lazy<IVideoService>(() => new VideoService(cloudinary));
        }
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IVideoService VideoService => _videoService.Value;
    }
}