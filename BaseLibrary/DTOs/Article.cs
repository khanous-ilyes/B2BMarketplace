using BaseLibrary.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class CreateArticleDto
    {
        [Required]
        public ArticleType ArticleType { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public decimal? Price { get; set; }
        public PriceType? PriceType { get; set; }
        public string Currency { get; set; } = "EUR";
        public int? Stock { get; set; }
        public string? DeliveryTime { get; set; }
        public string? Location { get; set; }
        public string? Specifications { get; set; }
        public bool IsContactPublic { get; set; } = false;
    }

    public class UpdateArticleDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public PriceType? PriceType { get; set; }
        public int? Stock { get; set; }
        public string? DeliveryTime { get; set; }
        public string? Location { get; set; }
        public string? Specifications { get; set; }
        public ArticleStatus? Status { get; set; }
        public bool? IsContactPublic { get; set; }
    }

    public class ArticleDto
    {
        public int Id { get; set; }
        public Guid? PublicId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ArticleType ArticleType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Specifications { get; set; }
        public decimal? Price { get; set; }
        public PriceType? PriceType { get; set; }
        public string Currency { get; set; } = "EUR";
        public int? Stock { get; set; }
        public string? DeliveryTime { get; set; }
        public string? Location { get; set; }
        public ArticleStatus Status { get; set; }
        public int ViewsCount { get; set; }
        public int ContactsCount { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? MediaUrls { get; set; }
        
        // Public Contact Info
        public bool IsContactPublic { get; set; }
        public string? SupplierPhone { get; set; }
        public string? SupplierEmail { get; set; }

        // Trust Badge
        public bool IsVerified { get; set; }
    }

    public class ArticleListDto
    {
        public int Id { get; set; }
        public Guid? PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public ArticleType ArticleType { get; set; }
        public decimal? Price { get; set; }
        public ArticleStatus Status { get; set; }
        public int ViewsCount { get; set; }
        public int ContactsCount { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

    public class SupplierArticleFilter
    {
        public string? Search { get; set; }
        public ArticleStatus? Status { get; set; }
        public ArticleType? ArticleType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDesc { get; set; } = true;
    }

    /// <summary>
    /// Filtres avancés pour la recherche Marketplace
    /// </summary>
    public class MarketplaceSearchFilter
    {
        // Recherche texte
        public string? Query { get; set; }
        
        // Filtres par catégorie/domaine
        public int? CategoryId { get; set; }
        public int? DomainId { get; set; }
        public int? SupplierId { get; set; }
        
        // Filtres par type
        public ArticleType? ArticleType { get; set; }
        
        // Filtres par prix
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        
        // Filtre localisation
        public string? Location { get; set; }
        
        // Filtre stock
        public bool? InStockOnly { get; set; }
        
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        
        // Tri: relevance, price_asc, price_desc, date_desc, popularity
        public string SortBy { get; set; } = "relevance";
    }

    /// <summary>
    /// Résultat enrichi pour le marketplace avec métadonnées de filtrage
    /// </summary>
    public class MarketplaceSearchResult
    {
        public List<MarketplaceArticleDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        // Métadonnées pour les filtres dynamiques
        public decimal? MinPriceAvailable { get; set; }
        public decimal? MaxPriceAvailable { get; set; }
        public List<CategoryCount>? CategoryCounts { get; set; }
    }

    public class CategoryCount
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int Count { get; set; }
    }

    /// <summary>
    /// DTO enrichi pour l'affichage Marketplace
    /// </summary>
    public class MarketplaceArticleDto
    {
        public int Id { get; set; }
        public Guid? PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Catégorie
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? DomainId { get; set; }
        public string? DomainName { get; set; }
        
        // Type
        public ArticleType ArticleType { get; set; }
        
        // Prix
        public decimal? Price { get; set; }
        public PriceType? PriceType { get; set; }
        public string Currency { get; set; } = "EUR";
        
        // Stock & Livraison
        public bool InStock { get; set; }
        public string? DeliveryTime { get; set; }
        public string? Location { get; set; }
        
        // Fournisseur
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        
        // Stats
        public int ViewsCount { get; set; }
        public int ContactsCount { get; set; }
        
        // Media
        public string? ThumbnailUrl { get; set; }
        
        // Dates
        public DateTime? PublishedAt { get; set; }

        // Geolocation & Rating
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? SupplierRating { get; set; }
        public string? SupplierCompanyName { get; set; }

        // Public Contact
        public bool IsContactPublic { get; set; }
        public string? SupplierPhone { get; set; }
        public string? SupplierEmail { get; set; }

        // Trust Badge
        public bool IsVerified { get; set; }
    }
}
