using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Client : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        public string? Siret { get; set; }
        public string? Sector { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        
        // Contact Information
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactName { get; set; }

        // Rating Statistics (calculated fields)
        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public int TotalContactRequests { get; set; } = 0;
        public int CompletedTransactions { get; set; } = 0;
        public int SuccessfulTransactions { get; set; } = 0;

        // Navigation
        public List<QuoteRequest>? QuoteRequests { get; set; }
        public List<Favorite>? Favorites { get; set; }
        public List<SavedSearch>? SavedSearches { get; set; }
        public List<ContactRequest>? ContactRequests { get; set; }
        public List<Review>? ReviewsReceived { get; set; }
    }
}
