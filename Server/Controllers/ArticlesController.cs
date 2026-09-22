using BaseLibrary.DTOs;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;
using System.Security.Claims;
using ServerLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ApplicationDbContext _context;

        public ArticlesController(IArticleRepository articleRepository, ApplicationDbContext context)
        {
            _articleRepository = articleRepository;
            _context = context;
        }

        // POST: api/articles
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto dto)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Vous devez être un fournisseur pour créer un article"));

            // Check Publication Limit
            var (allowed, message) = await CheckPublicationLimit(supplierId.Value);
            if (!allowed)
                return BadRequest(new GeneralResponse(false, message));

            var result = await _articleRepository.CreateArticle(supplierId.Value, dto);
            return Ok(result);
        }

        // PUT: api/articles/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody] UpdateArticleDto dto)
        {
            int? internalId = await ResolveArticleId(id);
            if (!internalId.HasValue) return NotFound(new GeneralResponse(false, "Article non trouvé"));

            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _articleRepository.UpdateArticle(internalId.Value, supplierId.Value, dto);
            return Ok(result);
        }

        // DELETE: api/articles/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            int? internalId = await ResolveArticleId(id);
            if (!internalId.HasValue) return NotFound(new GeneralResponse(false, "Article non trouvé"));
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _articleRepository.DeleteArticle(internalId.Value, supplierId.Value);
            return Ok(result);
        }

        // POST: api/articles/{id}/publish
        [HttpPost("{id}/publish")]
        [Authorize]
        public async Task<IActionResult> PublishArticle(string id)
        {
            int? internalId = await ResolveArticleId(id);
            if (!internalId.HasValue) return NotFound(new GeneralResponse(false, "Article non trouvé"));
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _articleRepository.PublishArticle(internalId.Value, supplierId.Value);
            return Ok(result);
        }

        // GET: api/articles/{id}
        // GET: api/articles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(string id)
        {
            int? internalId = await ResolveArticleId(id);
            if (!internalId.HasValue) return NotFound(new GeneralResponse(false, "Article non trouvé"));

            // Increment views
            await _articleRepository.IncrementViews(internalId.Value);

            var article = await _articleRepository.GetArticleById(internalId.Value);
            if (article == null)
                return NotFound(new GeneralResponse(false, "Article non trouvé"));

            return Ok(article);
        }

        // GET: api/articles/my
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyArticles()
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var articles = await _articleRepository.GetArticlesBySupplier(supplierId.Value);
            return Ok(articles);
        }

        // GET: api/articles/my/paginated
        [HttpGet("my/paginated")]
        [Authorize]
        public async Task<IActionResult> GetMyArticlesPaginated(
            [FromQuery] string? search,
            [FromQuery] int? status,
            [FromQuery] int? articleType,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool sortDesc = true)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var filter = new SupplierArticleFilter
            {
                Search = search,
                Status = status.HasValue ? (ArticleStatus)status.Value : null,
                ArticleType = articleType.HasValue ? (ArticleType)articleType.Value : null,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc
            };

            var result = await _articleRepository.GetArticlesBySupplierPaginated(supplierId.Value, filter);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchArticles(
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] int? domainId,
            [FromQuery] int? supplierId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var articles = await _articleRepository.SearchArticles(q, categoryId, domainId, supplierId, page, pageSize);
            return Ok(articles);
        }

        /// <summary>
        /// Recherche avancée Marketplace avec filtres professionnels
        /// </summary>
        [HttpGet("marketplace/search")]
        public async Task<IActionResult> MarketplaceSearch(
            [FromQuery] string? q,
            [FromQuery] int? categoryId,
            [FromQuery] int? domainId,
            [FromQuery] int? supplierId,
            [FromQuery] int? articleType,
            [FromQuery] decimal? priceMin,
            [FromQuery] decimal? priceMax,
            [FromQuery] string? location,
            [FromQuery] bool? inStockOnly,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string sortBy = "relevance")
        {
            var filter = new MarketplaceSearchFilter
            {
                Query = q,
                CategoryId = categoryId,
                DomainId = domainId,
                SupplierId = supplierId,
                ArticleType = articleType.HasValue ? (ArticleType)articleType.Value : null,
                PriceMin = priceMin,
                PriceMax = priceMax,
                Location = location,
                InStockOnly = inStockOnly,
                Page = page,
                PageSize = Math.Min(pageSize, 50), // Max 50 items per page
                SortBy = sortBy
            };

            var result = await _articleRepository.SearchArticlesAdvanced(filter);
            return Ok(result);
        }

        private async Task<int?> ResolveArticleId(string input)
        {
             if (Guid.TryParse(input, out Guid publicId))
            {
                var article = await _context.Articles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.PublicId == publicId);
                return article?.Id;
            }
            else if (int.TryParse(input, out int id))
            {
                return id;
            }
            return null;
        }

        private int? GetSupplierIdFromToken()
        {
            var specificIdClaim = User.FindFirst("SpecificId")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (roleClaim != "Supplier" || string.IsNullOrEmpty(specificIdClaim))
                return null;

            if (int.TryParse(specificIdClaim, out int supplierId))
                return supplierId;

            return null;
        }

        private async Task<(bool allowed, string message)> CheckPublicationLimit(int supplierId)
        {
            // Count active articles (Status != Archived and DeletedAt == null)
            var currentCount = await _context.Articles
                .CountAsync(a => a.SupplierId == supplierId && a.DeletedAt == null && a.Status != ArticleStatus.Archived);

            // Get Active Subscription
            var activeSub = await _context.Subscriptions
                .Where(s => s.SupplierId == supplierId && s.Status == SubscriptionStatus.Active && s.EndDate > DateTime.UtcNow)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (activeSub != null)
            {
                // If MaxPublications is null, it means unlimited
                if (!activeSub.MaxPublications.HasValue) return (true, "");
                
                int subLimit = activeSub.MaxPublications.Value;
                if (currentCount >= subLimit)
                {
                    return (false, $"Limite d'abonnement atteinte ({currentCount}/{subLimit}). Mettez à niveau votre offre pour ajouter plus d'articles.");
                }
                return (true, "");
            }

            // Default Free Limit (No active subscription)
            int defaultLimit = 10;
            if (currentCount >= defaultLimit)
            {
                return (false, $"Limite gratuite atteinte ({currentCount}/{defaultLimit}). Abonnez-vous pour publier plus d'articles.");
            }

            return (true, "");
        }
    }
}
