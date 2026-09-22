using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Domain : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // Navigation
        public List<Category>? Categories { get; set; }
    }
}
