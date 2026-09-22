using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Article : BaseEntity
    {
        [Required]
        public int SupplierId { get; set; }
        
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Required]
        public int CategoryId { get; set; }
        
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        public ArticleType ArticleType { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Slug { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;

        public string? Specifications { get; set; } // JSON

        public decimal? Price { get; set; }
        public PriceType? PriceType { get; set; }
        public string Currency { get; set; } = "EUR";
        
        public int? Stock { get; set; }
        public string? DeliveryTime { get; set; }
        public string? Location { get; set; }

        public bool IsContactPublic { get; set; } = false;

        [Required]
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

        public int ViewsCount { get; set; } = 0;
        public int ContactsCount { get; set; } = 0;
        
        public DateTime? PublishedAt { get; set; }

        // Navigation
        public List<ArticleMedia>? Media { get; set; }
        public List<Conversation>? Conversations { get; set; }
        public List<QuoteRequest>? QuoteRequests { get; set; }
        public List<Favorite>? FavoritedBy { get; set; }
    }
}
