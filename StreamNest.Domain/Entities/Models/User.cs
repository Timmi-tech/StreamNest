using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StreamNest.Domain.Entities.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public UserRole Role { get; set; } = UserRole.Consumer;


        public ICollection<Video> Videos { get; set; } = new List<Video>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();


    }
}