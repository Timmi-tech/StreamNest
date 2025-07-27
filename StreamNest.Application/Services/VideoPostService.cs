using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.Services
{
    internal class VideoPostService : IVideoPostService
    {
        private readonly IRepositoryManager? _repository;
        private readonly ILoggerManager? _logger;
        private readonly IVideoService? _videoService;

        public VideoPostService(IRepositoryManager repository, ILoggerManager logger, IVideoService videoService)
        {
            _repository = repository;
            _logger = logger;
            _videoService = videoService;
        }

        public async Task<VideoDto> CreateVideoAsync(CreateVideoDto createVideoDto, string userId)
        {
            if (createVideoDto == null)
                throw new ArgumentNullException(nameof(createVideoDto));

            // Validate video file
            if (createVideoDto.VideoFile == null || createVideoDto.VideoFile.Length == 0)
                throw new ArgumentException("No video file provided.");

            // Upload video to Cloudinary (or your video service)
            var uploadResult = await _videoService.UploadVideoAsync(createVideoDto.VideoFile);
            var thumbnailUrl = $"https://res.cloudinary.com/{_videoService.CloudName}/video/upload/so_2/{uploadResult.PublicId}.jpg";


            if (uploadResult.Error != null)
                throw new ApplicationException($"Video upload failed: {uploadResult.Error.Message}");

            // Create Video entity
            var video = new Video
            {
                Title = createVideoDto.Title,
                Description = createVideoDto.Description,
                Genre = createVideoDto.Genre,
                AgeRating = createVideoDto.AgeRating,
                VideoYear = createVideoDto.VideoYear,
                UploadDate = DateTime.UtcNow,
                VideoUrl = uploadResult.SecureUrl.ToString(),
                ThumbnailUrl = thumbnailUrl,
                CreatorId = userId,
                VideoTags = createVideoDto.Tags.Select(tag => new VideoTag { Tag = new Tag { Name = tag } }).ToList()
            };

            // Save to database
            _repository.Video.CreateVideo(video);
            await _repository.SaveAsync();

            // Map to DTO
            var videoDto = new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre.ToString(),
                AgeRating = video.AgeRating.ToString(),
                VideoYear = video.VideoYear,
                UploadedAt = video.UploadDate,
                VideoUrl = video.VideoUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                UserId = video.CreatorId,
                Tags = video.VideoTags.Select(vt => vt.Tag.Name).ToList()
            };

            return videoDto;
        }

        public async Task<IEnumerable<VideoDto>> GetAllVideosAsync(bool trackChanges)
        {
            var videos = await _repository.Video.GetAllVideosAsync(trackChanges);
            return videos.Select(video => new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre.ToString(),
                AgeRating = video.AgeRating.ToString(),
                VideoYear = video.VideoYear,
                UploadedAt = video.UploadDate,
                VideoUrl = video.VideoUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                UserId = video.CreatorId,
                UserName = video.Creator.UserName ?? string.Empty,
                Tags = video.VideoTags.Select(vt => vt.Tag.Name).ToList()
            });
        }
        public async Task<VideoDto> GetVideoByIdAsync(Guid videoId, bool trackChanges)
        {
            var video = await _repository.Video.GetVideoByIdAsync(videoId, trackChanges);
            if (video == null)
                throw new ApplicationException($"Video with ID {videoId} not found.");

            return new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre.ToString(),
                AgeRating = video.AgeRating.ToString(),
                VideoYear = video.VideoYear,
                UploadedAt = video.UploadDate,
                VideoUrl = video.VideoUrl,
                ThumbnailUrl = video.ThumbnailUrl,
                UserId = video.CreatorId,
                UserName = video.Creator.UserName ?? string.Empty,
                Tags = video.VideoTags.Select(vt => vt.Tag.Name).ToList()
            };
        }

        public async Task<IEnumerable<VideoDto>> GetVideosByUserIdAsync(string userId, bool trackChanges)
        {
            var videos = await _repository.Video.GetVideosByUserIdAsync(userId, trackChanges);
            return videos.Select(video => new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Genre = video.Genre.ToString(),
                AgeRating = video.AgeRating.ToString(),
                VideoYear = video.VideoYear,
                UploadedAt = video.UploadDate,
                VideoUrl = video.VideoUrl,
                UserId = video.CreatorId,
                UserName = video.Creator.UserName ?? string.Empty,
                Tags = video.VideoTags.Select(vt => vt.Tag.Name).ToList()
            });
        }
        public async Task DeleteVideoAsync(Guid videoId, string userId)
        {
            var video = await _repository.Video.GetVideoByIdAsync(videoId, trackChanges: false);
            if (video == null)
                throw new KeyNotFoundException($"Video with ID {videoId} not found.");

            if (video.CreatorId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this video.");

            _repository.Video.DeletePost(video);
            await _repository.SaveAsync();
        }
        public async Task<List<VideoDto>> SearchVideosAsync(string? query, Genre? genre, int? year)
        {
            var videos = await _repository.Video.SearchVideosAsync(query, genre, year);

            var result = videos.Select(v => new VideoDto
            {
                Id = v.Id,
                Title = v.Title,
                Description = v.Description,
                Genre = v.Genre.ToString(),
                AgeRating = v.AgeRating.ToString(),
                VideoUrl = v.VideoUrl,
                ThumbnailUrl = v.ThumbnailUrl,
                VideoYear = v.VideoYear,
                UploadedAt = v.UploadDate,
                UserId = v.CreatorId.ToString(),
                UserName = v.Creator?.UserName ?? "",
                Tags = v.VideoTags.Select(t => t.Tag.Name).ToList()
            }).ToList();

            return result;
        }
    }
}