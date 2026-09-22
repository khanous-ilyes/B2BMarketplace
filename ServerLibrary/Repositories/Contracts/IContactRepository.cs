using BaseLibrary.DTOs;
using BaseLibrary.Helpers;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IContactRepository
    {
        // Contact Requests
        Task<GeneralResponse> CreateContactRequest(int clientId, CreateContactRequestDto dto);
        Task<GeneralResponse> RespondToContactRequest(int contactRequestId, int supplierId, RespondContactRequestDto dto);
        Task<GeneralResponse> ConfirmCompletion(int contactRequestId, int userId, bool isClient, bool isSuccessful);
        Task<GeneralResponse> CancelContactRequest(int contactRequestId, int userId);
        
        Task<ContactRequestDto?> GetContactRequestById(int id, int userId);
        Task<PaginatedResult<ContactRequestDto>> GetContactRequestsForSupplier(int supplierId, ContactRequestStatus? status, int page, int pageSize, string? search = null);
        Task<PaginatedResult<ContactRequestDto>> GetContactRequestsForClient(int clientId, ContactRequestStatus? status, int page, int pageSize, string? search = null);

        // Reviews
        Task<GeneralResponse> CreateReview(int userId, CreateReviewDto dto);
        Task<List<ReviewDto>> GetReviewsForUser(int userId, int page, int pageSize);
        Task<List<ReviewDto>> GetReviewsForSupplier(int supplierId, int page, int pageSize);
        Task<List<ReviewDto>> GetReviewsForClient(int clientId, int page, int pageSize);

        // Statistics
        Task<SupplierStatsDto?> GetSupplierStats(int supplierId);
        Task<ClientStatsDto?> GetClientStats(int clientId);
        Task UpdateSupplierStats(int supplierId);
        Task UpdateClientStats(int clientId);

        // Profile
        Task<GeneralResponse> UpdateSupplierContact(int supplierId, UpdateSupplierContactDto dto);
        Task<GeneralResponse> UpdateClientContact(int clientId, UpdateClientContactDto dto);
        Task<UpdateSupplierContactDto?> GetSupplierFullProfile(int supplierId);
        Task<UpdateClientContactDto?> GetClientFullProfile(int clientId);
    }
}
