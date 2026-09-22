using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Favorite : BaseEntity
    {
        [Required]
        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Required]
        public int ArticleId { get; set; }
        
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }
    }
}
