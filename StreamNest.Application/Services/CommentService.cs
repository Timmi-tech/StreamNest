using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Contracts;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.Services
{
    internal class CommentService : ICommentService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public CommentService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CommentResponseDto> CreateCommentAsync(CreateCommentDto createCommentDto, string userProfileId)
        {
            if (createCommentDto == null)
                throw new ArgumentNullException(nameof(createCommentDto));

            var comment = new Comment
            {
                Content = createCommentDto.Content,
                VideoId = createCommentDto.VideoId,
                UserId = userProfileId,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Comment.AddComment(comment);
            await _repository.SaveAsync();



            var commentResponseDto = new CommentResponseDto
            {
                Id = comment.Id,
                Content = comment.Content,
                VideoId = comment.VideoId,
                UserProfileId = comment.UserId,
                CreatedAt = comment.CreatedAt

            };
            return commentResponseDto;
        }
        public async Task<List<CommentResponseDto>> GetCommentsByVideoIdAsync(Guid videoId, bool trackChanges)
        {
            var comments = await _repository.Comment.GetCommentsByVideoIdAsync(videoId, trackChanges);
            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Content = c.Content,
                VideoId = c.VideoId,
                UserProfileId = c.UserId,
                UserName = c.User.UserName ?? string.Empty,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
        public async Task DeleteCommentAsync(Guid commentId, string userProfileId, bool trackChanges)
        {
            var comment = await _repository.Comment.GetCommentByIdAsync(commentId, trackChanges);
            if (comment == null)
                throw new KeyNotFoundException($"Comment with ID {commentId} not found.");

            if (comment.UserId != userProfileId)
                throw new UnauthorizedAccessException("You do not have permission to delete this comment.");

            _repository.Comment.DeleteComment(comment);
            await _repository.SaveAsync();
        }



    }
}