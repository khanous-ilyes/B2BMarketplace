using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Subscription : BaseEntity
    {
        [Required]
        public int SupplierId { get; set; }
        
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Required]
        public int PlanId { get; set; }
        
        [ForeignKey("PlanId")]
        public SubscriptionPlan? Plan { get; set; }

        [Required]
        public SubscriptionStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AutoRenew { get; set; } = true;
        public int? MaxPublications { get; set; } // Snapshot from plan
        public DateTime? CancelledAt { get; set; }

        public List<Payment>? Payments { get; set; }
    }
}
