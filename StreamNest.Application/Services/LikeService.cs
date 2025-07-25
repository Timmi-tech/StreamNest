using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.Services
{
    internal class LikeService : ILikeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public LikeService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<LikeResponseDto> ToggleLikeAsync(CreateLikeDto createLikeDto, string userId)
        {
            if (createLikeDto == null)
                throw new ArgumentNullException(nameof(createLikeDto));

            var existinglike = await _repository.Like.GetLikeAsync(createLikeDto.VideoId, userId, false);
            if (existinglike != null)
            {
                _repository.Like.DeleteLike(existinglike);
            }
            else
            {
                var like = new Like
                {
                    VideoId = createLikeDto.VideoId,
                    UserId = userId,
                    LikedAt = DateTime.UtcNow
                };
                _repository.Like.CreateLike(like);
            }

            await _repository.SaveAsync();

            var totalLikes = await _repository.Like.GetLikeCountAsync(createLikeDto.VideoId, false);

            return new LikeResponseDto
            {
                VideoId = createLikeDto.VideoId,
                UserId = userId,
                TotalLikes = totalLikes,
                IsLiked = existinglike == null

            };
        }
        public async Task<LikeResponseDto> GetLikeStatusAsync(Guid videoId, string userId)
        {
            var like = await _repository.Like.GetLikeAsync(videoId, userId, false);
            var totalLikes = await _repository.Like.GetLikeCountAsync(videoId, false);

            return new LikeResponseDto
            {
                VideoId = videoId,
                UserId = userId,
                TotalLikes = totalLikes,
                IsLiked = like != null
            };
        }

    }
}
