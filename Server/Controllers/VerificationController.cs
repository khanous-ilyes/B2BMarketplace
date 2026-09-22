using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Services.Contracts;
using System.Security.Claims;
using System.Text.Json;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;

        public VerificationController(ApplicationDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        private int? GetSupplierIdFromToken()
        {
            var specificIdClaim = User.FindFirst("SpecificId")?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (roleClaim != "Supplier" || string.IsNullOrEmpty(specificIdClaim))
                return null;

            if (int.TryParse(specificIdClaim, out int supplierId))
                return supplierId;

            return null;
        }

        // ──────── SUPPLIER ENDPOINTS ────────

        /// <summary>
        /// GET /api/verification/my-status — Get current verification status for supplier
        /// </summary>
        [HttpGet("my-status")]
        [Authorize]
        public async Task<IActionResult> GetMyVerificationStatus()
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound();

            return Ok(new
            {
                status = supplier.VerificationStatus.ToString(),
                statusCode = (int)supplier.VerificationStatus,
                requestedAt = supplier.VerificationRequestedAt,
                resolvedAt = supplier.VerificationResolvedAt,
                adminNote = supplier.VerificationAdminNote,
                documents = string.IsNullOrEmpty(supplier.VerificationDocumentsJson)
                    ? new string[0]
                    : JsonSerializer.Deserialize<string[]>(supplier.VerificationDocumentsJson)
            });
        }

        /// <summary>
        /// POST /api/verification/request — Supplier submits trust badge request with documents
        /// </summary>
        [HttpPost("request")]
        [Authorize]
        [RequestSizeLimit(20 * 1024 * 1024)] // 20MB total
        public async Task<IActionResult> RequestVerification(
            [FromForm] string? note,
            [FromForm] List<IFormFile>? documents)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound();

            // Can only request if None or Rejected
            if (supplier.VerificationStatus == VerificationStatus.Pending ||
                supplier.VerificationStatus == VerificationStatus.UnderReview)
                return BadRequest(new GeneralResponse(false, "Une demande de vérification est déjà en cours."));

            if (supplier.VerificationStatus == VerificationStatus.Approved)
                return BadRequest(new GeneralResponse(false, "Votre compte est déjà vérifié."));

            // Upload documents
            var docUrls = new List<string>();
            if (documents != null && documents.Count > 0)
            {
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "application/pdf" };

                foreach (var doc in documents)
                {
                    if (doc.Length > 5 * 1024 * 1024)
                        return BadRequest(new GeneralResponse(false, $"Le fichier '{doc.FileName}' dépasse 5MB."));

                    if (!allowedTypes.Contains(doc.ContentType.ToLower()))
                        return BadRequest(new GeneralResponse(false, $"Type non supporté pour '{doc.FileName}'. Formats acceptés: JPG, PNG, WebP, PDF"));

                    var folder = $"verification/{supplierId}";
                    using var stream = doc.OpenReadStream();
                    var url = await _storageService.UploadFileAsync(stream, doc.FileName, doc.ContentType, folder);
                    docUrls.Add(url);
                }
            }

            supplier.VerificationStatus = VerificationStatus.Pending;
            supplier.VerificationNote = note;
            supplier.VerificationDocumentsJson = JsonSerializer.Serialize(docUrls);
            supplier.VerificationRequestedAt = DateTime.UtcNow;
            supplier.VerificationResolvedAt = null;
            supplier.VerificationAdminNote = null;

            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, "Votre demande de vérification a été envoyée. L'équipe va examiner vos documents."));
        }

        // ──────── ADMIN ENDPOINTS ────────

        /// <summary>
        /// GET /api/verification/admin/requests — List all verification requests for admin
        /// </summary>
        [HttpGet("admin/requests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVerificationRequests([FromQuery] string? status)
        {
            var query = _context.Suppliers
                .Include(s => s.User)
                .Where(s => s.VerificationStatus != VerificationStatus.None);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<VerificationStatus>(status, true, out var parsed))
            {
                query = query.Where(s => s.VerificationStatus == parsed);
            }

            var rawResults = await query
                .OrderByDescending(s => s.VerificationRequestedAt)
                .Select(s => new
                {
                    supplierId = s.Id,
                    companyName = s.CompanyName,
                    siret = s.Siret,
                    email = s.User != null ? s.User.Email : "",
                    phone = s.Phone,
                    status = s.VerificationStatus.ToString(),
                    statusCode = (int)s.VerificationStatus,
                    note = s.VerificationNote,
                    adminNote = s.VerificationAdminNote,
                    requestedAt = s.VerificationRequestedAt,
                    resolvedAt = s.VerificationResolvedAt,
                    logoUrl = s.LogoUrl,
                    documentsJson = s.VerificationDocumentsJson,
                    // Stats for admin to evaluate
                    totalArticles = s.Articles != null ? s.Articles.Count(a => a.DeletedAt == null) : 0,
                    totalReviews = s.TotalReviews,
                    averageRating = s.AverageRating,
                    completedTransactions = s.CompletedTransactions
                })
                .ToListAsync();

            var results = rawResults.Select(s => new
            {
                s.supplierId,
                s.companyName,
                s.siret,
                s.email,
                s.phone,
                s.status,
                s.statusCode,
                s.note,
                s.adminNote,
                s.requestedAt,
                s.resolvedAt,
                s.logoUrl,
                documents = string.IsNullOrEmpty(s.documentsJson)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<string[]>(s.documentsJson),
                s.totalArticles,
                s.totalReviews,
                s.averageRating,
                s.completedTransactions
            });

            return Ok(results);
        }

        /// <summary>
        /// PUT /api/verification/admin/{supplierId}/approve — Admin approves verification
        /// </summary>
        [HttpPut("admin/{supplierId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveVerification(int supplierId, [FromBody] AdminVerificationAction? action)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound(new GeneralResponse(false, "Fournisseur non trouvé"));

            supplier.VerificationStatus = VerificationStatus.Approved;
            supplier.VerificationAdminNote = action?.AdminNote ?? "Vérification approuvée.";
            supplier.VerificationResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"Le fournisseur '{supplier.CompanyName}' a été vérifié avec succès."));
        }

        /// <summary>
        /// PUT /api/verification/admin/{supplierId}/reject — Admin rejects verification
        /// </summary>
        [HttpPut("admin/{supplierId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectVerification(int supplierId, [FromBody] AdminVerificationAction? action)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound(new GeneralResponse(false, "Fournisseur non trouvé"));

            supplier.VerificationStatus = VerificationStatus.Rejected;
            supplier.VerificationAdminNote = action?.AdminNote ?? "Votre demande n'a pas été approuvée.";
            supplier.VerificationResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, $"La demande du fournisseur '{supplier.CompanyName}' a été refusée."));
        }

        /// <summary>
        /// PUT /api/verification/admin/{supplierId}/review — Admin marks as under review
        /// </summary>
        [HttpPut("admin/{supplierId}/review")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkUnderReview(int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return NotFound(new GeneralResponse(false, "Fournisseur non trouvé"));

            supplier.VerificationStatus = VerificationStatus.UnderReview;
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, "Marqué en cours de vérification."));
        }
    }

    public class AdminVerificationAction
    {
        public string? AdminNote { get; set; }
    }
}
