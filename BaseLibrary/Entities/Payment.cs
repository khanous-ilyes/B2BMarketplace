using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        public int SubscriptionId { get; set; }
        
        [ForeignKey("SubscriptionId")]
        public Subscription? Subscription { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "EUR";
        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? InvoiceUrl { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Metadata { get; set; } // JSON
        public string? ErrorMessage { get; set; }
    }
}
