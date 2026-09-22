using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Conversation : BaseEntity
    {
        public int? ArticleId { get; set; }
        
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }

        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        public int SupplierId { get; set; }
        
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        public string? Subject { get; set; }
        
        [Required]
        public ConversationStatus Status { get; set; } = ConversationStatus.Open;

        public DateTime? LastMessageAt { get; set; }

        public List<Message>? Messages { get; set; }
    }
}
