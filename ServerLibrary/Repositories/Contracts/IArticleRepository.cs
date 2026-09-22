using BaseLibrary.DTOs;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IArticleRepository
    {
        Task<GeneralResponse> CreateArticle(int supplierId, CreateArticleDto dto);
        Task<GeneralResponse> UpdateArticle(int articleId, int supplierId, UpdateArticleDto dto);
        Task<GeneralResponse> DeleteArticle(int articleId, int supplierId);
        Task<GeneralResponse> PublishArticle(int articleId, int supplierId);
        Task<ArticleDto?> GetArticleById(int articleId);
        Task<List<ArticleListDto>> GetArticlesBySupplier(int supplierId);
        Task<PaginatedResult<ArticleListDto>> GetArticlesBySupplierPaginated(int supplierId, SupplierArticleFilter filter);
        Task<List<ArticleListDto>> SearchArticles(string? query, int? categoryId, int? domainId, int? supplierId = null, int page = 1, int pageSize = 20);
        Task<MarketplaceSearchResult> SearchArticlesAdvanced(MarketplaceSearchFilter filter);
        Task<GeneralResponse> IncrementViews(int articleId);
    }
}
