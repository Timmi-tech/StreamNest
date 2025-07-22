using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;

namespace StreamNest.Application.Services
{
    internal sealed class UserProfileService : IUserProfileService
    {
        private readonly IRepositoryManager? _repository;
        private readonly ILoggerManager? _logger;

        public UserProfileService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;

        }
        public async Task<UserProfileDto> GetUserProfileAsync(string userId, bool trackChanges)
        {
            var user = await _repository.User.GetUserProfileAsync(userId, trackChanges);
            if (user is null)
            {
                throw new UserProfileNotFoundException(userId);
            }

            var userDto = new UserProfileDto
            {
                Id = user.Id,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                Email = user.Email,
                Username = user.UserName,
                Role = user.Role.ToString(),

            };
            return userDto;
        }

        public async Task UpdateProfileAsync(string userId, UserUpdateProfileDto userUpdateProfileDto, bool trackChanges)
        {
            var userUpdateEntity = await _repository.User.GetUserProfileAsync(userId, trackChanges);
            if (userUpdateEntity is null)
            {
                throw new UserProfileNotFoundException(userId);
            }
            userUpdateEntity.FirstName = userUpdateProfileDto.Firstname;
            userUpdateEntity.LastName = userUpdateProfileDto.Lastname;
            userUpdateEntity.UserName = userUpdateProfileDto.Username;

            await _repository.SaveAsync();
        }
    }
    
}