using BaseLibrary.Entities;
using BaseLibrary.Helpers;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Seul l'admin peut accéder
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/users?type=Supplier&status=Pending
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] UserType? type, [FromQuery] UserStatus? status)
        {
            var query = _context.Users
                .Include(u => u.Supplier)
                .Include(u => u.Client)
                .AsQueryable();

            if (type.HasValue)
                query = query.Where(u => u.UserType == type.Value);
            
            if (status.HasValue)
                query = query.Where(u => u.Status == status.Value);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new 
                {
                    u.Id,
                    u.Email,
                    u.UserType,
                    u.Status,
                    u.CreatedAt,
                    Name = u.UserType == UserType.Supplier ? u.Supplier.CompanyName : (u.UserType == UserType.Client ? u.Client.CompanyName : "Admin"),
                    Details = u.UserType == UserType.Supplier ? u.Supplier : null // Plus de détails si nécessaire
                })
                .ToListAsync();

            return Ok(users);
        }

        // PUT: api/admin/users/{id}/approve
        [HttpPut("users/{id}/approve")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new GeneralResponse(false, "Utilisateur non trouvé"));

            if (user.Status == UserStatus.Active)
                return BadRequest(new GeneralResponse(false, "Utilisateur déjà actif"));

            // Store previous status for message
            var previousStatus = user.Status;
            
            // Allow activation from Pending or Suspended status
            user.Status = UserStatus.Active;
            await _context.SaveChangesAsync();

            var action = previousStatus == UserStatus.Pending ? "approuvé" : "réactivé";
            return Ok(new GeneralResponse(true, $"Utilisateur {action} avec succès"));
        }

        // PUT: api/admin/users/{id}/ban
        [HttpPut("users/{id}/ban")]
        public async Task<IActionResult> BanUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new GeneralResponse(false, "Utilisateur non trouvé"));

            user.Status = UserStatus.Suspended;
            await _context.SaveChangesAsync();

            return Ok(new GeneralResponse(true, "Utilisateur suspendu"));
        }

        // GET: api/admin/users/{id} - Get detailed user info for debugging
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _context.Users
                .Include(u => u.Supplier)
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound(new GeneralResponse(false, "Utilisateur non trouvé"));

            return Ok(new
            {
                user.Id,
                user.Email,
                user.UserType,
                user.Status,
                StatusName = user.Status.ToString(),
                user.CreatedAt,
                Name = user.UserType == UserType.Supplier ? user.Supplier?.CompanyName : 
                       (user.UserType == UserType.Client ? user.Client?.CompanyName : "Admin")
            });
        }
    }
}
