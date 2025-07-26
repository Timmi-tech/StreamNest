using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using StreamNest.Application.Services.Contracts;

namespace StreamNest.Application.Services
{
    public class VideoService : IVideoService
    {
        private readonly Cloudinary _cloudinary;
        private static readonly string[] AllowedExtensions = { ".mp4", ".mov", ".avi", ".wmv", ".flv", ".mkv", ".webm" };

        public VideoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
        }

        public async Task<VideoUploadResult> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type. Allowed types are: MP4, MOV, AVI, WMV, FLV, MKV, WEBM.");

            var uploadResult = new VideoUploadResult();

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "StreamNest_Videos"
                    // You can add more video transformations here if needed
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error uploading video to Cloudinary.", ex);
            }

            return uploadResult;
        }

        public async Task<List<VideoUploadResult>> UploadVideosAsync(List<IFormFile> files)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("No files provided.");

            var allowedExtensions = new HashSet<string>(AllowedExtensions);

            var uploadTasks = files
                .Where(file => file != null && file.Length > 0 &&
                               allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                .Select(async file =>
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "StreamNest_Videos"
                    };
                    return await _cloudinary.UploadAsync(uploadParams).ConfigureAwait(false);
                });

            var results = await Task.WhenAll(uploadTasks);
            return results.Where(r => r != null).ToList();
        }

        public async Task<DeletionResult> DeleteVideoAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("Public ID cannot be null or empty.");

            try
            {
                var deleteParams = new DeletionParams(publicId) { ResourceType = ResourceType.Video };
                return await _cloudinary.DestroyAsync(deleteParams).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error deleting video from Cloudinary.", ex);
            }
        }
        public string CloudName => _cloudinary.Api.Account.Cloud;
    }
}