using BaseLibrary.DTOs;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        // ============ Helper Methods ============

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        private int? GetSupplierIdFromToken()
        {
            var supplierIdClaim = User.FindFirst("SpecificId");
            if (supplierIdClaim != null && int.TryParse(supplierIdClaim.Value, out int supplierId))
                return supplierId;
            return null;
        }

        private int? GetClientIdFromToken()
        {
            var clientIdClaim = User.FindFirst("SpecificId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int clientId))
                return clientId;
            return null;
        }

        private bool IsClient() 
        {
            return User.Claims.Any(c => 
                (c.Type == ClaimTypes.Role || 
                 c.Type == "role" || 
                 c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") 
                && c.Value == "Client");
        }

        private bool IsSupplier() 
        {
            return User.Claims.Any(c => 
                (c.Type == ClaimTypes.Role || 
                 c.Type == "role" || 
                 c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") 
                && c.Value == "Supplier");
        }

        // ============ Contact Requests ============

        /// <summary>
        /// Create a contact request for an article (Client only)
        /// </summary>
        [HttpPost("request")]
        [Authorize]
        public async Task<IActionResult> CreateContactRequest([FromBody] CreateContactRequestDto dto)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null || !IsClient())
                return Unauthorized(new GeneralResponse(false, "Seuls les clients peuvent envoyer des demandes de contact"));

            var result = await _contactRepository.CreateContactRequest(clientId.Value, dto);
            return Ok(result);
        }

        /// <summary>
        /// Respond to a contact request (Supplier only)
        /// </summary>
        [HttpPost("request/{id}/respond")]
        [Authorize]
        public async Task<IActionResult> RespondToContactRequest(int id, [FromBody] RespondContactRequestDto dto)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null || !IsSupplier())
                return Unauthorized(new GeneralResponse(false, "Seuls les fournisseurs peuvent répondre aux demandes"));

            var result = await _contactRepository.RespondToContactRequest(id, supplierId.Value, dto);
            return Ok(result);
        }

        /// <summary>
        /// Confirm completion of a transaction
        /// </summary>
        [HttpPost("request/{id}/confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmCompletion(int id, [FromQuery] bool isSuccessful = true)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _contactRepository.ConfirmCompletion(id, userId.Value, IsClient(), isSuccessful);
            return Ok(result);
        }

        /// <summary>
        /// Cancel a contact request
        /// </summary>
        [HttpPost("request/{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelContactRequest(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _contactRepository.CancelContactRequest(id, userId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific contact request
        /// </summary>
        [HttpGet("request/{id}")]
        [Authorize]
        public async Task<IActionResult> GetContactRequest(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var request = await _contactRepository.GetContactRequestById(id, userId.Value);
            if (request == null)
                return NotFound(new GeneralResponse(false, "Demande non trouvée"));

            return Ok(request);
        }

        /// <summary>
        /// Get contact requests for the current supplier
        /// </summary>
        [HttpGet("requests/supplier")]
        [Authorize]
        public async Task<IActionResult> GetSupplierContactRequests(
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null || !IsSupplier())
                return Unauthorized(new GeneralResponse(false, "Accès refusé"));

            ContactRequestStatus? statusEnum = status.HasValue ? (ContactRequestStatus)status.Value : null;
            var result = await _contactRepository.GetContactRequestsForSupplier(supplierId.Value, statusEnum, page, pageSize, search);
            return Ok(result);
        }

        /// <summary>
        /// Get contact requests for the current client
        /// </summary>
        [HttpGet("requests/client")]
        [Authorize]
        public async Task<IActionResult> GetClientContactRequests(
            [FromQuery] int? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null || !IsClient())
                return Unauthorized(new GeneralResponse(false, "Accès refusé"));

            ContactRequestStatus? statusEnum = status.HasValue ? (ContactRequestStatus)status.Value : null;
            var result = await _contactRepository.GetContactRequestsForClient(clientId.Value, statusEnum, page, pageSize, search);
            return Ok(result);
        }

        // ============ Reviews ============

        /// <summary>
        /// Create a review for a completed transaction
        /// </summary>
        [HttpPost("review")]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var result = await _contactRepository.CreateReview(userId.Value, dto);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews for a supplier (public)
        /// </summary>
        [HttpGet("reviews/supplier/{supplierId}")]
        public async Task<IActionResult> GetSupplierReviews(
            int supplierId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var reviews = await _contactRepository.GetReviewsForSupplier(supplierId, page, pageSize);
            return Ok(reviews);
        }

        /// <summary>
        /// Get reviews for a client
        /// </summary>
        [HttpGet("reviews/client/{clientId}")]
        [Authorize]
        public async Task<IActionResult> GetClientReviews(
            int clientId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var reviews = await _contactRepository.GetReviewsForClient(clientId, page, pageSize);
            return Ok(reviews);
        }

        // ============ Statistics ============

        /// <summary>
        /// Get supplier statistics (public)
        /// </summary>
        [HttpGet("stats/supplier/{supplierId}")]
        public async Task<IActionResult> GetSupplierStats(int supplierId)
        {
            var stats = await _contactRepository.GetSupplierStats(supplierId);
            if (stats == null)
                return NotFound(new GeneralResponse(false, "Fournisseur non trouvé"));

            return Ok(stats);
        }

        /// <summary>
        /// Get client statistics
        /// </summary>
        [HttpGet("stats/client/{clientId}")]
        [Authorize]
        public async Task<IActionResult> GetClientStats(int clientId)
        {
            var stats = await _contactRepository.GetClientStats(clientId);
            if (stats == null)
                return NotFound(new GeneralResponse(false, "Client non trouvé"));

            return Ok(stats);
        }

        // ============ Profile ============

        // GET: api/contact/profile/supplier
        // GET: api/contact/profile/supplier
        [HttpGet("profile/supplier")]
        [Authorize]
        public async Task<IActionResult> GetMySupplierProfile()
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null || !IsSupplier()) return Unauthorized();

            var result = await _contactRepository.GetSupplierFullProfile(supplierId.Value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/contact/profile/client
        [HttpGet("profile/client")]
        [Authorize] 
        public async Task<IActionResult> GetMyClientProfile()
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null || !IsClient()) return Unauthorized();

            var result = await _contactRepository.GetClientFullProfile(clientId.Value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Update supplier contact information
        /// </summary>
        [HttpPut("profile/supplier")]
        [Authorize]
        public async Task<IActionResult> UpdateSupplierContact([FromBody] UpdateSupplierContactDto dto)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null || !IsSupplier())
                return Unauthorized(new GeneralResponse(false, "Accès refusé"));

            var result = await _contactRepository.UpdateSupplierContact(supplierId.Value, dto);
            return Ok(result);
        }

        /// <summary>
        /// Update client contact information
        /// </summary>
        [HttpPut("profile/client")]
        [Authorize]
        public async Task<IActionResult> UpdateClientContact([FromBody] UpdateClientContactDto dto)
        {
            var clientId = GetClientIdFromToken();
            if (clientId == null || !IsClient())
                return Unauthorized(new GeneralResponse(false, "Accès refusé"));

            var result = await _contactRepository.UpdateClientContact(clientId.Value, dto);
            return Ok(result);
        }
    }
}
