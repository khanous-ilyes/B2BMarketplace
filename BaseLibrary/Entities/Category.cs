using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        public int DomainId { get; set; }
        
        [ForeignKey("DomainId")]
        public Domain? Domain { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation
        public List<Article>? Articles { get; set; }
    }
}
