using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace StreamNest.Application.DTOs
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile? File { get; set; }
    }

    public class UploadImagesDto
    {
        public List<IFormFile> Files { get; set; }
    }
}
