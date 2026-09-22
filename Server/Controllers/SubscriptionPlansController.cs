using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubscriptionPlansController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PLAN MANAGEMENT (ADMIN)
        // ==========================================

        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<List<SubscriptionPlan>>> GetPlans()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive && p.DeletedAt == null)
                .OrderBy(p => p.PriorityLevel)
                .ToListAsync();
            return Ok(plans);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<ActionResult<List<SubscriptionPlan>>> GetAllPlans()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.DeletedAt == null)
                .OrderByDescending(p => p.PriorityLevel)
                .ToListAsync();
            return Ok(plans);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<ActionResult<SubscriptionPlan>> CreatePlan(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionPlan>> GetPlan(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<IActionResult> UpdatePlan(int id, SubscriptionPlan plan)
        {
            if (id != plan.Id) return BadRequest();
            _context.Entry(plan).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null) return NotFound();
            
            // Soft delete
            plan.DeletedAt = DateTime.UtcNow;
            plan.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================
        // SUBSCRIPTION REQUESTS (SUPPLIER -> ADMIN)
        // ==========================================

        [HttpPost("request")]
        [Authorize(Roles = "Supplier")]
        public async Task<ActionResult> RequestSubscription([FromBody] int planId) // Body: planId
        {
            // Get Supplier Id from User
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var supplier = await _context.Suppliers
                .Include(s => s.Subscriptions)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (supplier == null) return BadRequest("Supplier profile not found.");

            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return NotFound("Plan not found.");

            // Check if existing pending request
            var pendingSub = supplier.Subscriptions?.FirstOrDefault(s => s.Status == SubscriptionStatus.Pending);
            if (pendingSub != null)
            {
                // Update existing request
                pendingSub.PlanId = planId;
                pendingSub.Plan = plan; 
                pendingSub.MaxPublications = plan.MaxPublications; // Update snapshot limit
                pendingSub.EndDate = DateTime.UtcNow.AddMonths(plan.DurationMonths); // Refresh duration
                pendingSub.CreatedAt = DateTime.UtcNow; // Refresh date to separate from old request
                await _context.SaveChangesAsync();
                return Ok(new { message = "Subscription request updated.", subscriptionId = pendingSub.Id });
            }

            // Create new request
            var newSub = new Subscription
            {
                SupplierId = supplier.Id,
                PlanId = planId,
                Status = SubscriptionStatus.Pending,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(plan.DurationMonths),
                MaxPublications = plan.MaxPublications,
                AutoRenew = true
            };

            _context.Subscriptions.Add(newSub);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Subscription requested successfully.", subscriptionId = newSub.Id });
        }

        [HttpGet("my-subscription")]
        [Authorize(Roles = "Supplier")]
        public async Task<ActionResult<Subscription>> GetMySubscription()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == userId);
            if (supplier == null) return NotFound();

            var sub = await _context.Subscriptions
                .Include(s => s.Plan)
                .Where(s => s.SupplierId == supplier.Id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync(); // Get latest

            return Ok(sub);
        }

        // ==========================================
        // APPROVAL WORKFLOW (ADMIN)
        // ==========================================

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<ActionResult<List<Subscription>>> GetPendingSubscriptions()
        {
            var subs = await _context.Subscriptions
                .Include(s => s.Supplier)
                .Include(s => s.Plan)
                .Where(s => s.Status == SubscriptionStatus.Pending)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return Ok(subs);
        }

        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<IActionResult> ApproveSubscription(int id)
        {
            var sub = await _context.Subscriptions.Include(s => s.Plan).FirstOrDefaultAsync(s => s.Id == id);
            if (sub == null) return NotFound();

            sub.Status = SubscriptionStatus.Active;
            sub.StartDate = DateTime.UtcNow;
            // Recalculate EndDate based on real start date
            if (sub.Plan != null)
                sub.EndDate = sub.StartDate.AddMonths(sub.Plan.DurationMonths);
            else
                sub.EndDate = sub.StartDate.AddMonths(1);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Subscription approved." });
        }

        [HttpPost("reject/{id}")]
        [Authorize(Roles = "Admin,server_admin")]
        public async Task<IActionResult> RejectSubscription(int id)
        {
            var sub = await _context.Subscriptions.FindAsync(id);
            if (sub == null) return NotFound();

            sub.Status = SubscriptionStatus.Rejected;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Subscription rejected." });
        }
    }
}
