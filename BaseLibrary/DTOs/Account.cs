using System.ComponentModel.DataAnnotations;
using BaseLibrary.Helpers;

namespace BaseLibrary.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }
    }

    public record LoginDto(
        [Required, EmailAddress] string Email, 
        [Required] string Password
    );

    public record UserSession(
        string? Id, 
        string? Name, 
        string? Email, 
        string? Role
    );

    public record LoginResponse(
        bool Flag, 
        string Message, 
        string Token,
        string RefreshToken
    );
    public record GeneralResponse(
        bool Flag, 
        string Message
    );

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class ChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class GoogleLoginDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
        public UserType? UserType { get; set; }
    }
}
