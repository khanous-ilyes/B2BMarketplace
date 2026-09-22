using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class QuoteRequest : BaseEntity
    {
        [Required]
        public int ArticleId { get; set; }
        
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }

        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public int? ConversationId { get; set; }
        
        [ForeignKey("ConversationId")]
        public Conversation? Conversation { get; set; }

        public int? Quantity { get; set; }
        public string? Details { get; set; }
        
        public DateTime? RequiredDate { get; set; }

        [Required]
        public QuoteRequestStatus Status { get; set; } = QuoteRequestStatus.Pending;

        public DateTime? RespondedAt { get; set; }
    }
}
