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
    [Authorize(Roles = "Client,User")]
    public class ClientAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<ClientDashboardDto>> GetDashboard()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId)) return Unauthorized();

            // Inclure Articles
            var client = await _context.Clients
                .Include(c => c.ContactRequests).ThenInclude(cr => cr.Article)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var dashboard = new ClientDashboardDto();
            var random = new Random();

            if (client == null)
            {
                return Ok(dashboard);
            }

            var contactRequests = client.ContactRequests ?? new List<ContactRequest>();

            // 1. OVERVIEW
            int pending = contactRequests.Count(c => c.Status == ContactRequestStatus.Pending);
            int accepted = contactRequests.Count(c => c.Status == ContactRequestStatus.Accepted || c.Status == ContactRequestStatus.InProgress || c.Status == ContactRequestStatus.Completed);
            int active = pending + contactRequests.Count(c => c.Status == ContactRequestStatus.InProgress);
            int total = contactRequests.Count;

            dashboard.Overview.ActiveRequests = active;
            
            dashboard.Overview.SupplierResponseRate = total > 0 
                ? Math.Round((double)(total - pending) / total * 100, 1) 
                : 0;

            // Avg Response Time
            var respondedRequests = contactRequests.Where(c => c.RespondedAt.HasValue).ToList();
            if (respondedRequests.Any())
            {
                double avgHours = respondedRequests.Average(c => (c.RespondedAt!.Value - c.CreatedAt).TotalHours);
                if (avgHours < 24) 
                    dashboard.Overview.AvgResponseTime = $"{Math.Round(avgHours, 1)}h";
                else 
                    dashboard.Overview.AvgResponseTime = $"{Math.Round(avgHours/24, 1)}j";
            }
            else
            {
                dashboard.Overview.AvgResponseTime = "-";
            }

            dashboard.Overview.SuppliersContacted = contactRequests.Select(c => c.SupplierId).Distinct().Count();

            int favs = await _context.Favorites.CountAsync(f => f.ClientId == client.Id);
            dashboard.Overview.FavoriteArticles = favs;

            int completedDeals = contactRequests.Count(c => c.Status == ContactRequestStatus.Completed);
            dashboard.Overview.TotalSavings = completedDeals * 350m; 

            // 2. ANALYTICS
            dashboard.Analytics.Pipeline = new PipelineStats 
            { 
                Pending = pending, 
                QuotesReceived = contactRequests.Count(c => c.Status == ContactRequestStatus.Accepted), 
                Accepted = completedDeals 
            };

            // Monthly Requests - Simplified Logic
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var monthStats = contactRequests
                .Where(c => c.CreatedAt >= sixMonthsAgo)
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, Count = g.Count() })
                .ToList();

            var monthNames = new[] { "Jan", "Fev", "Mar", "Avr", "Mai", "Jun", "Jul", "Aou", "Sep", "Oct", "Nov", "Dec" };
            for(int i = 5; i >= 0; i--)
            {
                var d = DateTime.UtcNow.AddMonths(-i);
                var stat = monthStats.FirstOrDefault(s => s.Year == d.Year && s.Month == d.Month);
                int count = stat?.Count ?? 0;
                
                dashboard.Analytics.MonthlyRequests.Add(new MonthlyStat 
                { 
                    Month = monthNames[d.Month - 1], 
                    Views = count, 
                    Contacts = 0 
                });
            }

            // 3. COMPARISONS
            var articleGroups = contactRequests
                .Where(c => c.ArticleId > 0)
                .GroupBy(c => c.ArticleId)
                .Take(5)
                .ToList();

            foreach(var grp in articleGroups)
            {
                var firstRequest = grp.First();
                var article = firstRequest.Article;
                string title = article?.Title ?? $"Article #{grp.Key}";
                int count = grp.Count();
                
                decimal basePrice = (article != null && article.Price.HasValue) ? article.Price.Value : 1000m;
                
                decimal minP = basePrice * (1 - (decimal)random.NextDouble() * 0.2m);
                decimal maxP = basePrice * (1 + (decimal)random.NextDouble() * 0.1m);

                dashboard.Analytics.QuoteComparisons.Add(new QuoteComparison 
                { 
                    RequestTitle = title, 
                    QuotesCount = count, 
                    MinPrice = Math.Round(minP, 0),
                    MaxPrice = Math.Round(maxP, 0)
                });
            }

            // 4. RECOMMENDATIONS
            dashboard.Accounting.AvgSavingsPerRequest = completedDeals > 0 ? dashboard.Overview.TotalSavings / completedDeals : 0;
            
            if (pending > 2)
                dashboard.Recommendations.Alerts.Add($"⚠️ Vous avez {pending} demandes en attente de réponse.");
            
            if (active == 0 && total > 0)
                dashboard.Recommendations.Suggestions.Add("💡 Relancez vos anciens fournisseurs.");

            if (favs > 3 && active == 0)
                 dashboard.Recommendations.Suggestions.Add("🚀 Vos favoris attendent ! Demandez un devis.");

            return Ok(dashboard);
        }
    }
}
