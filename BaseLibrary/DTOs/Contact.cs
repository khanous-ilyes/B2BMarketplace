using BaseLibrary.Helpers;

namespace BaseLibrary.DTOs
{
    // ============ Contact Request DTOs ============

    public class CreateContactRequestDto
    {
        public int ArticleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? Quantity { get; set; }
    }

    public class RespondContactRequestDto
    {
        public bool Accept { get; set; }
        public string? ResponseMessage { get; set; }
    }

    public class ContactRequestDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientCompanyName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierCompanyName { get; set; } = string.Empty;
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; } = string.Empty;
        public string? ArticleThumbnail { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ResponseMessage { get; set; }
        public ContactRequestStatus Status { get; set; }
        public int? Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool ClientConfirmedCompletion { get; set; }
        public bool SupplierConfirmedCompletion { get; set; }
        public bool? IsSuccessful { get; set; }
        public bool CanReview { get; set; }
        public bool HasClientReview { get; set; }
        public bool HasSupplierReview { get; set; }

        // Contact info (only shown when accepted)
        public SupplierContactInfo? SupplierContact { get; set; }
        public ClientContactInfo? ClientContact { get; set; }
    }

    public class SupplierContactInfo
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        public string? Website { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LogoUrl { get; set; }
        public bool IsVerified { get; set; }
    }

    public class ClientContactInfo
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
    }

    // ============ Review DTOs ============

    public class CreateReviewDto
    {
        public int ContactRequestId { get; set; }
        public int Rating { get; set; }  // 1-5
        public string? Comment { get; set; }
        public ReviewTags Tags { get; set; } = ReviewTags.None;
        public bool IsTransactionSuccessful { get; set; } = true;
    }

    public class ReviewDto
    {
        public int Id { get; set; }
        public int ContactRequestId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public bool IsFromClient { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public ReviewTags Tags { get; set; }
        public bool IsTransactionSuccessful { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ArticleTitle { get; set; }
    }

    // ============ Statistics DTOs ============

    public class SupplierStatsDto
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalContactRequests { get; set; }
        public int AcceptedContactRequests { get; set; }
        public double AcceptanceRate { get; set; }  // %
        public int CompletedTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public double SuccessRate { get; set; }  // %
        public double AverageResponseTimeHours { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LogoUrl { get; set; }
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        public DateTime JoinedAt { get; set; }
        public List<ReviewDto>? RecentReviews { get; set; }
        public bool IsVerified { get; set; }
    }

    public class ClientStatsDto
    {
        public int ClientId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalContactRequests { get; set; }
        public int CompletedTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public double SuccessRate { get; set; }
    }

    // ============ Profile Update DTOs ============

    public class UpdateSupplierContactDto
    {
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class UpdateClientContactDto
    {
        public string? PhoneCountryCode { get; set; }
        public string? Phone { get; set; }
        public string? WhatsApp { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactName { get; set; }
    }
}
