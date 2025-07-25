using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNest.API.Controllers
{
    [Route("api/Comments")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly IServiceManager? _service;

        public CommentsController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Create a new comment for a video.
        /// </summary>
        /// <param name="createCommentDto">The comment data including video ID and comment content.</param>
        /// <returns>Returns the created comment object.</returns>
        /// <response code="201">Comment created successfully.</response>
        /// <response code="400">If comment data is null or invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a comment", Description = "Allows an authenticated user to create a comment for a video.")]
        [SwaggerResponse(201, "Comment created successfully")]
        [SwaggerResponse(400, "Comment data is null or invalid")]
        [SwaggerResponse(401, "User not authenticated")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            if (createCommentDto == null)
                return BadRequest("Comment data is null");

            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var commentDto = await _service.CommentService.CreateCommentAsync(createCommentDto, userId);

            return CreatedAtAction(nameof(GetCommentsByVideoId), new { videoId = createCommentDto.VideoId }, commentDto);
        }

        /// <summary>
        /// Retrieve all comments for a specific video.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>Returns a list of comments for the specified video.</returns>
        /// <response code="200">Comments found and returned.</response>
        /// <response code="404">No comments found for the given video.</response>
        [HttpGet("{videoId:guid}")]
        [SwaggerOperation(Summary = "Get comments by video ID", Description = "Retrieves all comments for a given video.")]
        [SwaggerResponse(200, "Comments retrieved successfully", Type = typeof(List<CommentResponseDto>))]
        [SwaggerResponse(404, "Video not found")]
        public async Task<IActionResult> GetCommentsByVideoId(Guid videoId)
        {
            var videoExists = await _service.VideoPostService.GetVideoByIdAsync(videoId, trackChanges: false);
            if (videoExists == null)
                return NotFound(new { Message = "Video not found." });

            var comments = await _service.CommentService.GetCommentsByVideoIdAsync(videoId, trackChanges: false);

            return Ok(comments);
        }

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>No content if deleted successfully.</returns>
        /// <response code="204">Comment deleted successfully.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpDelete("{commentId:guid}")]
        [SwaggerOperation(Summary = "Delete a comment", Description = "Allows an authenticated user to delete their comment.")]
        [SwaggerResponse(204, "Comment deleted successfully")]
        [SwaggerResponse(401, "User not authenticated")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            await _service.CommentService.DeleteCommentAsync(commentId, userId, trackChanges: false);
            return NoContent();
        }

        /// <summary>
        /// Get the user ID from JWT claims.
        /// </summary>
        /// <returns>User ID as string or null if not found.</returns>
        private string? GetUserIdFromClaims()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
