using BaseLibrary.Entities;
using BaseLibrary.DTOs;

namespace ServerLibrary.Repositories.Contracts
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategories(bool includeDomain = false);
        Task<List<Domain>> GetDomains();
        Task<Category?> GetCategoryById(int id);
        
        // Admin Only actions usually, but interface just defines capability
        Task<GeneralResponse> AddCategory(Category category);
        Task<GeneralResponse> UpdateCategory(Category category);
        Task<GeneralResponse> DeleteCategory(int id);

        Task<GeneralResponse> AddDomain(Domain domain);
        Task<GeneralResponse> UpdateDomain(Domain domain);
        Task<GeneralResponse> DeleteDomain(int id);
    }
}
