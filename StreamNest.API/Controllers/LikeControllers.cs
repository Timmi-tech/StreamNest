using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNest.API.Controllers
{
    [Route("api/Likes")]
    [ApiController]
    [Authorize]
    public class LikeControllers : ControllerBase
    {
        private readonly IServiceManager? _service;

        public LikeControllers(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Like or Unlike a video (toggle).
        /// </summary>
        /// <param name="createLikeDto">DTO containing the VideoId to like/unlike.</param>
        /// <returns>Returns the updated like status.</returns>
        /// <response code="200">Returns the result of the like toggle action.</response>
        /// <response code="400">If the request body is null or invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Toggle like/unlike on a video", Description = "Allows an authenticated user to like or unlike a video.")]
        [SwaggerResponse(200, "Like toggle successful")]
        [SwaggerResponse(400, "Like data is null or invalid")]
        [SwaggerResponse(401, "User not authenticated")]
        public async Task<IActionResult> ToggleLike([FromBody] CreateLikeDto createLikeDto)
        {
            if (createLikeDto == null)
                return BadRequest("Like data is null");

            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var likeResponse = await _service.LikeService.ToggleLikeAsync(createLikeDto, userId);
            return Ok(likeResponse);
        }

        /// <summary>
        /// Get the like status of a video by the authenticated user.
        /// </summary>
        /// <param name="videoId">The ID of the video.</param>
        /// <returns>True if the user has liked the video, false otherwise.</returns>
        /// <response code="200">Returns the like status.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("{videoId:guid}")]
        [SwaggerOperation(Summary = "Check like status on a video", Description = "Checks if the authenticated user has liked a specific video.")]
        [SwaggerResponse(200, "Like status retrieved successfully")]
        [SwaggerResponse(401, "User not authenticated")]
        public async Task<IActionResult> GetLikeStatus(Guid videoId)
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var likeStatus = await _service.LikeService.GetLikeStatusAsync(videoId, userId);
            return Ok(likeStatus);
        }

        /// <summary>
        /// Extracts the authenticated user's ID from JWT claims.
        /// </summary>
        /// <returns>User ID as a string if present; otherwise null.</returns>
        private string? GetUserIdFromClaims()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
