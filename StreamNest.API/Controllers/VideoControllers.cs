using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using StreamNest.Application.Services.Contracts;
using StreamNest.Application.DTOs;

[ApiController]
[Route("api/video")]
public class VideosController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideosController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    /// <summary>
    /// Uploads a video to Cloudinary.
    /// </summary>
    /// <param name="file">The video file to upload.</param>
    /// <returns>The uploaded video result.</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Uploads a video",
        Description = "Upload a video file"
    )]
    public async Task<IActionResult> Upload([FromForm] FileUploadDto uploadDto)
    {
        if (uploadDto.File == null || uploadDto.File.Length == 0)
            return BadRequest(new { message = "No file uploaded or file is empty." });

        var result = await _videoService.UploadVideoAsync(uploadDto.File);

        if (result.Error != null)
            return BadRequest(new { message = result.Error.Message });

        return Ok(new
        {
            message = "Video uploaded successfully.",
            publicId = result.PublicId,
            url = result.SecureUrl?.ToString()
        });
    }

    /// <summary>
    /// Uploads multiple videos to Cloudinary.
    /// </summary>
    /// <param name="files">The video files to upload.</param>
    /// <returns>The uploaded video results.</returns>
    [HttpPost("upload-multiple")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Uploads multiple videos",
        Description = "Upload multiple video files"
    )]
    public async Task<IActionResult> UploadVideos([FromForm] UploadImagesDto uploadImagesDto)
    {
        if (uploadImagesDto.Files == null || !uploadImagesDto.Files.Any())
            return BadRequest(new { message = "No files uploaded or files are empty." });

        var results = await _videoService.UploadVideosAsync(uploadImagesDto.Files);

        var cloudName = _videoService.CloudName;
        var uploadedVideos = results.Select(result => new
        {
            publicId = result.PublicId,
            url = result.SecureUrl?.ToString(),
            thumbnailUrl = $"https://res.cloudinary.com/{cloudName}/video/upload/so_2/{result.PublicId}.jpg"
        }).ToList();

        return Ok(new
        {
            message = "Videos uploaded successfully.",
            videos = uploadedVideos
        });
    }

    /// <summary>
    /// Deletes a video from Cloudinary.
    /// </summary>
    /// <param name="publicId">The public ID of the video to delete.</param>
    /// <returns>Deletion result.</returns>
    [HttpDelete("{publicId}")]
    public async Task<IActionResult> Delete(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return BadRequest(new { message = "Invalid Public ID." });

        try
        {
            var result = await _videoService.DeleteVideoAsync(publicId);

            if (result.Error != null)
                return BadRequest(new { message = result.Error.Message });

            return Ok(new { message = "Video deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An error occurred while deleting the video.", error = ex.Message });
        }
    }
}