using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        public string? TargetAudience { get; set; } // Domaine cible
        public int PriorityLevel { get; set; } = 0; // Priorité
        
        public int DurationMonths { get; set; } = 1;
        public int? MaxPublications { get; set; } // Null = Unlimited
        public string? Features { get; set; } // JSON
        
        public bool IsActive { get; set; } = true;
    }
}
