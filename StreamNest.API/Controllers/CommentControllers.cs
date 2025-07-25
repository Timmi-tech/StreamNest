using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StreamNest.Application.DTOs;
using StreamNest.Application.Services.Contracts;

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
        [HttpPost]
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
        [HttpGet("{videoId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetCommentsByVideoId(Guid videoId)
        {
            var comments = await _service.CommentService.GetCommentsByVideoIdAsync(videoId, trackChanges: false);

            if (comments == null || !comments.Any())
                return NotFound(new { Message = "No comments found for this video." });        
            return Ok(comments);
        }
        [HttpDelete("{commentId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");
            await _service.CommentService.DeleteCommentAsync(commentId, userId, trackChanges: false);
            return NoContent();
        }
    
        private string? GetUserIdFromClaims()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

    }
    
}