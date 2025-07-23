using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using StreamNest.Application.Services.Contracts;
using StreamNest.Application.DTOs;

namespace StreamNest.API.Controllers
{
    [Route("api/UserProfile")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IServiceManager _service;

        public UserProfileController(IServiceManager service)
        {
            _service = service;
        }

        /// <summary>
        /// Gets a user profile by user ID.
        /// </summary>
        /// <param name="id">The user ID to fetch the profile for.</param>
        /// <returns>User profile details if found.</returns>
        /// <response code="200">Returns the user profile</response>
        /// <response code="400">If the user ID is null or empty</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get user profile by ID", Description = "Retrieves a user profile by its unique identifier.")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new { message = "User ID is required." });
            var user = await _service.UserProfileService.GetUserProfileAsync(id, trackChanges: false);
            if (user == null)
                return NotFound(new { message = "User not found." });
            return Ok(user);
        }

        /// <summary>
        /// Gets the authenticated user's profile.
        /// </summary>
        /// <returns>The current authenticated user's profile.</returns>
        /// <response code="200">Returns the user profile</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpGet]
        [SwaggerOperation(Summary = "Get authenticated user's profile", Description = "Fetches the currently authenticated user's profile.")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser()
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User is not authenticated or token is invalid." });

            var user = await _service.UserProfileService.GetUserProfileAsync(userId, trackChanges: false);return Ok(user);
        }
         /// <summary>
        /// Updates the authenticated user's profile.
        /// </summary>
        /// <param name="userUpdateProfile">The updated profile information.</param>
        /// <returns>No content if the update is successful.</returns>
        /// <response code="204">Profile updated successfully</response>
        /// <response code="400">If the input is null or invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [HttpPut]
        [SwaggerOperation(Summary = "Update authenticated user's profile", Description = "Updates the profile of the currently authenticated user.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UserUpdateProfileDto userUpdateProfile)
        {
            if (userUpdateProfile is null)
            {
                return BadRequest("UserProfileForUpdateDto object is null");
            }
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User is not authenticated or token is invalid." });

            await _service.UserProfileService.UpdateProfileAsync(userId, userUpdateProfile, trackChanges: true);

            return NoContent();
        }

        private string? GetUserIdFromClaims()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
