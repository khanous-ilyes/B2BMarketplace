using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Message : BaseEntity
    {
        [Required]
        public int ConversationId { get; set; }
        
        [ForeignKey("ConversationId")]
        public Conversation? Conversation { get; set; }

        [Required]
        public int SenderId { get; set; } // UserId of sender
        
        [ForeignKey("SenderId")]
        public User? Sender { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public bool HasAttachment { get; set; } = false;
    }
}
