using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using StreamNest.Domain.Entities.Models;

namespace StreamNest.Application.DTOs
{
    public record UserForRegistrationDto
    {
        [Required(ErrorMessage = "Firstname is required")]
        public string Firstname { get; init; } = string.Empty;
        [Required(ErrorMessage = "Lastname is required")]
        public string Lastname { get; init; } = string.Empty;
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; init; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; init; } = string.Empty;
        public UserRole Role { get; init; } = UserRole.Consumer;
    }

    public record UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }

    }

    public record TokenDto(string AccessToken, string RefreshToken);

} 