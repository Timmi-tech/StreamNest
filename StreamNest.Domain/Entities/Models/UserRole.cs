using System.ComponentModel.DataAnnotations;

namespace StreamNest.Domain.Entities.Models
{
    public enum UserRole
    {
        
        [Display(Name = "Consumer (Can view videos)")]
        Consumer = 0,

        [Display(Name = "Creator (Can upload videos)")]
        Creator = 1
    }
}