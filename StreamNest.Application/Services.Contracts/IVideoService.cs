using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace StreamNest.Application.Services.Contracts
{
    public interface IVideoService
    {
        Task<VideoUploadResult> UploadVideoAsync(IFormFile videoFile);
        Task<List<VideoUploadResult>> UploadVideosAsync(List<IFormFile> videoFiles);
        Task<DeletionResult> DeleteVideoAsync(string publicId);
        string CloudName { get; }
        // Task<string> GetVideoUrlAsync(string publicId);
        // Task<IEnumerable<string>> GetAllVideosAsync();
    }
}