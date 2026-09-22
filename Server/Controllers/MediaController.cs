using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Services.Contracts;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IStorageService _storageService;

        public MediaController(ApplicationDbContext context, IWebHostEnvironment environment, IStorageService storageService)
        {
            _context = context;
            _environment = environment;
            _storageService = storageService;
        }

        // POST: api/media/upload/{articleId}
        [HttpPost("upload/{articleId}")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit per file
        public async Task<IActionResult> UploadMedia(string articleId, IFormFile file)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            // Verify article belongs to supplier (supports int ID or GUID PublicId)
            var article = await ResolveArticle(articleId, supplierId);
            if (article == null)
                return NotFound(new GeneralResponse(false, "Article non trouvé"));

            int realArticleId = article.Id;

            if (file == null || file.Length == 0)
                return BadRequest(new GeneralResponse(false, "Aucun fichier fourni"));

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(new GeneralResponse(false, "Type de fichier non autorisé. Utilisez JPG, PNG, GIF ou WebP"));

            // Upload via Storage Service
            string fileUrl;
            using (var stream = file.OpenReadStream())
            {
               fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType, $"articles/{realArticleId}");
            }

            // Get current max display order
            var maxOrder = await _context.ArticleMedias
                .Where(m => m.ArticleId == realArticleId)
                .MaxAsync(m => (int?)m.DisplayOrder) ?? -1;

            // Save to database
            var media = new ArticleMedia
            {
                ArticleId = realArticleId,
                MediaType = MediaType.Image,
                FileUrl = fileUrl,
                FileName = file.FileName,
                FileSize = (int)file.Length,
                MimeType = file.ContentType,
                DisplayOrder = maxOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            _context.ArticleMedias.Add(media);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                flag = true,
                message = "Image uploadée avec succès",
                media = new
                {
                    id = media.Id,
                    fileUrl = media.FileUrl,
                    fileName = media.FileName,
                    displayOrder = media.DisplayOrder
                }
            });
        }

        // POST: api/media/upload-multiple/{articleId}
        [HttpPost("upload-multiple/{articleId}")]
        [Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50MB total limit
        public async Task<IActionResult> UploadMultipleMedia(int articleId, List<IFormFile> files)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == articleId && a.SupplierId == supplierId);
            if (article == null)
                return NotFound(new GeneralResponse(false, "Article non trouvé"));

            if (files == null || files.Count == 0)
                return BadRequest(new GeneralResponse(false, "Aucun fichier fourni"));

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            
            var maxOrder = await _context.ArticleMedias
                .Where(m => m.ArticleId == articleId)
                .MaxAsync(m => (int?)m.DisplayOrder) ?? -1;

            var uploadedMedia = new List<object>();

            foreach (var file in files)
            {
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    continue;

                string fileUrl;
                using (var stream = file.OpenReadStream())
                {
                   fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType, $"articles/{articleId}");
                }

                maxOrder++;

                var media = new ArticleMedia
                {
                    ArticleId = articleId,
                    MediaType = MediaType.Image,
                    FileUrl = fileUrl,
                    FileName = file.FileName,
                    FileSize = (int)file.Length,
                    MimeType = file.ContentType,
                    DisplayOrder = maxOrder,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ArticleMedias.Add(media);
                uploadedMedia.Add(new
                {
                    id = media.Id,
                    fileUrl = media.FileUrl,
                    fileName = media.FileName,
                    displayOrder = media.DisplayOrder
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                flag = true,
                message = $"{uploadedMedia.Count} image(s) uploadée(s)",
                media = uploadedMedia
            });
        }

        // GET: api/media/article/{articleId}
        [HttpGet("article/{articleId}")]
        public async Task<IActionResult> GetArticleMedia(string articleId)
        {
            var article = await ResolveArticle(articleId);
            if (article == null) return NotFound("Article not found");

            var media = await _context.ArticleMedias
                .Where(m => m.ArticleId == article.Id)
                .OrderBy(m => m.DisplayOrder)
                .Select(m => new
                {
                    m.Id,
                    m.FileUrl,
                    m.FileName,
                    m.FileSize,
                    m.MimeType,
                    m.DisplayOrder
                })
                .ToListAsync();

            return Ok(media);
        }

        // DELETE: api/media/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var media = await _context.ArticleMedias
                .Include(m => m.Article)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (media == null)
                return NotFound(new GeneralResponse(false, "Média non trouvé"));

            if (media.Article?.SupplierId != supplierId)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            // Delete file via Service
            await _storageService.DeleteFileAsync(media.FileUrl);

            // Try delete local file if legacy
            if (!media.FileUrl.StartsWith("http"))
            {
                 var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, media.FileUrl.TrimStart('/'));
                 if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            _context.ArticleMedias.Remove(media);
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, "Image supprimée"));
        }

        // PUT: api/media/reorder/{articleId}
        [HttpPut("reorder/{articleId}")]
        [Authorize]
        public async Task<IActionResult> ReorderMedia(int articleId, [FromBody] List<int> mediaIds)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var article = await _context.Articles.FirstOrDefaultAsync(a => a.Id == articleId && a.SupplierId == supplierId);
            if (article == null)
                return NotFound(new GeneralResponse(false, "Article non trouvé"));

            var medias = await _context.ArticleMedias
                .Where(m => m.ArticleId == articleId && mediaIds.Contains(m.Id))
                .ToListAsync();

            for (int i = 0; i < mediaIds.Count; i++)
            {
                var media = medias.FirstOrDefault(m => m.Id == mediaIds[i]);
                if (media != null)
                {
                    media.DisplayOrder = i;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, "Ordre mis à jour"));
        }

        // POST: api/media/upload-supplier-logo
        [HttpPost("upload-supplier-logo")]
        [Authorize]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5MB limit
        public async Task<IActionResult> UploadSupplierLogo(IFormFile file)
        {
            var supplierId = GetSupplierIdFromToken();
            if (supplierId == null)
                return Unauthorized(new GeneralResponse(false, "Non autorisé"));

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId);
            if (supplier == null)
                return NotFound(new GeneralResponse(false, "Fournisseur non trouvé"));

            if (file == null || file.Length == 0)
                return BadRequest(new GeneralResponse(false, "Aucun fichier fourni"));

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return BadRequest(new GeneralResponse(false, "Type de fichier non autorisé."));

            string fileUrl;
            using (var stream = file.OpenReadStream())
            {
                fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType, $"suppliers/{supplierId}");
            }

            if (!string.IsNullOrEmpty(supplier.LogoUrl) && supplier.LogoUrl.StartsWith("http"))
            {
                // Optionally delete the old file
                try { await _storageService.DeleteFileAsync(supplier.LogoUrl); } catch { }
            }
            // For local storage we also could have a check, but since we always return fileUrl from storage service, we save it.

            supplier.LogoUrl = fileUrl;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                flag = true,
                message = "Image de profil mise à jour",
                logoUrl = supplier.LogoUrl
            });
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

        private async Task<Article?> ResolveArticle(string articleIdInput, int? supplierId = null)
        {
            if (Guid.TryParse(articleIdInput, out Guid publicId))
            {
                var query = _context.Articles.AsQueryable();
                if (supplierId.HasValue)
                    query = query.Where(a => a.SupplierId == supplierId.Value && a.PublicId == publicId);
                else
                    query = query.Where(a => a.PublicId == publicId);
                    
                return await query.FirstOrDefaultAsync();
            }
            else if (int.TryParse(articleIdInput, out int id))
            {
                var query = _context.Articles.AsQueryable();
                 if (supplierId.HasValue)
                    query = query.Where(a => a.SupplierId == supplierId.Value && a.Id == id);
                else
                    query = query.Where(a => a.Id == id);

                return await query.FirstOrDefaultAsync();
            }
            return null;
        }
    }
}
