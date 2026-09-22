using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class SavedSearch : BaseEntity
    {
        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        public string SearchName { get; set; } = string.Empty;
        
        [Required]
        public string SearchParams { get; set; } = string.Empty; // JSON
        
        public bool AlertEnabled { get; set; } = false;
    }
}
