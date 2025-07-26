using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using StreamNest.Domain.Entities.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StreamNest.API.Controllers
{
    [Route("api/VideoPosts")]
    [ApiController]
    [Authorize]


    public class VideoPostController : ControllerBase
    {
        private readonly IServiceManager _service;

        public VideoPostController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Creates a new video post.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a user with the "Creator" role to upload a new video post with required metadata.
        /// It accepts a `multipart/form-data` request containing the video file and related details.
        /// </remarks>
        /// <param name="videoPostDto">The video post data including title, description, tags, and video file.</param>
        /// <returns>Returns the created video post information or an error message.</returns>
        /// <response code="201">Video post created successfully.</response>
        /// <response code="400">Invalid input or creation failed.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Creator")]
        [SwaggerOperation(
            Summary = "Create a new video post",
            Description = "Allows a creator to upload a new video post with metadata and video file."
        )]
        [SwaggerResponse(StatusCodes.Status201Created, "Video post created successfully", typeof(VideoDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request or failed to create video post.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]

        public async Task<IActionResult> CreateVideoPost([FromForm] CreateVideoDto videoPostDto)
        {
            if (videoPostDto == null)
                return BadRequest(new { message = "Video post data is required." });

            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." });

            var result = await _service.VideoPostService.CreateVideoAsync(videoPostDto, userId);

            if (result == null)
                return BadRequest(new { message = "Failed to create video post." });

            return CreatedAtAction(nameof(CreateVideoPost), new { id = result.Id }, result);
        }
        /// <summary>
        /// Retrieves all video posts.
        /// </summary>
        /// <remarks>
        /// This endpoint allows any authenticated user to retrieve a list of all video posts.
        /// </remarks>
        /// <returns>Returns a list of video posts.</returns>
        /// <response code="200">List of video posts retrieved successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get all video posts",
            Description = "Retrieves a list of all video posts."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "List of video posts retrieved successfully", typeof(IEnumerable<VideoDto>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _service.VideoPostService.GetAllVideosAsync(trackChanges: false);
            return Ok(videos);
        }
        /// <summary>
        /// Retrieves a video post by its ID.
        /// </summary>
        /// <remarks>
        /// This endpoint allows any authenticated user to retrieve a specific video post by its ID.
        /// </remarks>
        /// <param name="videoPostId">The unique identifier of the video post.</param>
        /// <returns>Returns the video post information.</returns>
        /// <response code="200">Video post retrieved successfully.</response>
        /// <response code="404">Video post not found.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{videoPostId:guid}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Get video post by ID",
            Description = "Retrieves a specific video post by its ID."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Video post retrieved successfully", typeof(VideoDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Video post not found.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
        public async Task<IActionResult> GetVideoById(Guid videoPostId)
        {
            var video = await _service.VideoPostService.GetVideoByIdAsync(videoPostId, trackChanges: false);
            if (video == null)
                return NotFound(new { message = $"Video post with ID {videoPostId} not found." });

            return Ok(video);
        }
        /// <summary>
        /// Retrieves all video posts created by a specific user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows any authenticated user to retrieve a list of video posts created by a specific user.
        /// </remarks>
        /// <param name="userId">The unique identifier of the user whose video posts are
        /// to be retrieved.</param>
        /// <returns>Returns a list of video posts created by the specified user.</returns>
        /// <response code="200">List of video posts retrieved successfully.</response>
        /// <response code="404">User not found or has no video posts.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("me")]
        [Authorize(Roles = "Creator")]
        [SwaggerOperation(
            Summary = "Get video posts by user ID",
            Description = "Retrieves all video posts created by a specific user."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "List of video posts retrieved successfully", typeof(IEnumerable<VideoDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found or has no video posts.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
        public async Task<IActionResult> GetmyVideos()
        {

            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." });
            var videos = await _service.VideoPostService.GetVideosByUserIdAsync(userId, trackChanges: false);
            if (videos == null || !videos.Any())
                return NotFound(new { message = $"No video posts found for user with ID {userId}." });

            return Ok(videos);
        }
        /// <summary>
        /// Deletes a video post by its ID.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a user with the "Creator" role to delete a specific video post by its ID.
        /// </remarks>
        /// <param name="videoPostId">The unique identifier of the video post to be deleted.</param>
        /// <returns>Returns a success message if the video post is deleted.</returns>
        /// <response code="204">Video post deleted successfully.</response>
        /// <response code="404">Video post not found.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{videoPostId:guid}")]
        [Authorize(Roles = "Creator")]
        [SwaggerOperation(
            Summary = "Delete a video post",
            Description = "Allows a creator to delete a specific video post by its ID."
        )]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Video post deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Video post not found.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
        public async Task<IActionResult> DeleteVideoPost(Guid videoPostId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." });

                await _service.VideoPostService.DeleteVideoAsync(videoPostId, userId);
                return NoContent();
        }

        /// <summary>
        /// Searches for videos based on query parameters.
        /// </summary>
        /// <remarks>
        /// This endpoint allows any authenticated user to search for videos by title, genre, or year
        /// using optional query parameters.
        /// </remarks>
        /// <param name="query">Optional search query for video title.</param>
        /// <param name="genre">Optional genre to filter videos.</param>
        /// <param name="year">Optional year to filter videos.</param>
        /// <returns>Returns a list of videos matching the search criteria.</returns>
        /// <response code="200">List of videos matching the search criteria.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="403">User is not authorized to perform this action.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("search")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Search videos",
            Description = "Searches for videos based on title, genre, or year."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "List of videos matching the search criteria", typeof(IEnumerable<Video>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "User not authenticated.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "User does not have the required role.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.")]
        public async Task<IActionResult> SearchVideos([FromQuery] string? query, [FromQuery] Genre? genre, [FromQuery] int? year)
        {
            var videos = await _service.VideoPostService.SearchVideosAsync(query, genre, year);
            return Ok(videos);
        }
        private string? GetUserIdFromClaims()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

    }
}