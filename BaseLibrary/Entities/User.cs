using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Pending;

        public bool EmailVerified { get; set; } = false;
        public string? VerificationToken { get; set; } // For email verification
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public string? IpAddress { get; set; }

        // Navigation properties
        public Supplier? Supplier { get; set; }
        public Client? Client { get; set; }
    }
}
