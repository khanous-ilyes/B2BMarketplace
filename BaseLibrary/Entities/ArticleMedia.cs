using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class ArticleMedia : BaseEntity
    {
        [Required]
        public int ArticleId { get; set; }
        
        [ForeignKey("ArticleId")]
        public Article? Article { get; set; }

        [Required]
        public MediaType MediaType { get; set; }

        [Required]
        public string FileUrl { get; set; } = string.Empty;
        
        public string? FileName { get; set; }
        public int? FileSize { get; set; }
        public string? MimeType { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}
