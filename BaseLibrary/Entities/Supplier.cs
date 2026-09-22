using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Supplier : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required]
        public string Siret { get; set; } = string.Empty;
        
        public string? LegalForm { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        
        // Contact Information
        public string? PhoneCountryCode { get; set; } // Ex: +33, +212, +1
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? Certifications { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Rating Statistics (calculated fields)
        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public int TotalContactRequests { get; set; } = 0;
        public int AcceptedContactRequests { get; set; } = 0;
        public int CompletedTransactions { get; set; } = 0;
        public int SuccessfulTransactions { get; set; } = 0;
        public double AverageResponseTimeHours { get; set; } = 0;

        // Trust Badge / Verification
        public Helpers.VerificationStatus VerificationStatus { get; set; } = Helpers.VerificationStatus.None;
        public string? VerificationDocumentsJson { get; set; } // JSON array of uploaded doc URLs
        public string? VerificationNote { get; set; } // Message from supplier
        public string? VerificationAdminNote { get; set; } // Feedback from admin
        public DateTime? VerificationRequestedAt { get; set; }
        public DateTime? VerificationResolvedAt { get; set; }

        // Navigation
        public List<Article>? Articles { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<ContactRequest>? ContactRequests { get; set; }
        public List<Review>? ReviewsReceived { get; set; }
    }
}
