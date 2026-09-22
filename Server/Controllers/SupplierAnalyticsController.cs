using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Helpers; // Added for ArticleStatus
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supplier")]
    public class SupplierAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SupplierAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<SupplierDashboardDto>> GetDashboard()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == userId); // Corrected OwnerId to UserId
            if (supplier == null) return NotFound("Profil fournisseur introuvable.");

            var articles = await _context.Articles
                .Where(a => a.SupplierId == supplier.Id)
                .Include(a => a.Category)
                .ToListAsync();

            var dashboard = new SupplierDashboardDto();
            
            // Données brutes
            var totalViews = articles.Sum(a => a.ViewsCount);
            // On simule ContactsCount car il n'est pas encore peuplé partout, on prend aléatoire ou 0
            var totalContacts = articles.Sum(a => a.ContactsCount); 
            
            // Constantes simulées
            decimal subscriptionCost = 49.99m; // Prix fictif de l'abonnement mensuel

            // --- 1. OVERVIEW ---
            dashboard.Overview.TotalViews = totalViews;
            dashboard.Overview.ActiveArticles = articles.Count(a => a.Status == ArticleStatus.Published);
            dashboard.Overview.SubscriptionLimit = 50; 
            
            dashboard.Overview.EngagementRate = totalViews > 0 
                ? Math.Round((double)totalContacts / totalViews * 100, 2) 
                : 0;

            // Estimation Revenue (Prix moyen * Contacts * Taux conversion moyen 20%)
            var avgPrice = articles.Where(a => a.Price.HasValue).Average(a => a.Price) ?? 0;
            dashboard.Overview.PotentialRevenue = (decimal)totalContacts * avgPrice;
            dashboard.Overview.ConversionRate = totalContacts > 0 ? 15.5 : 0; // Taux fictif

            dashboard.Overview.RevenueTrend = "+12%";
            dashboard.Overview.ViewsTrend = "+8%";
            dashboard.Overview.ConversionTrend = "+2.4%";
            dashboard.Overview.EngagementTrend = "-1%";


            // --- 2. PERFORMANCE ---
            // Simulation de données mensuelles pour le graphique
            var random = new Random();
            var months = new[] { "Jan", "Fev", "Mar", "Avr", "Mai", "Juin" };
            foreach (var m in months)
            {
                // Tendance croissante simulée
                var baseView = totalViews / 6; 
                var monthViews = random.Next((int)(baseView * 0.8), (int)(baseView * 1.5));
                dashboard.Performance.MonthlyEvolution.Add(new MonthlyStat 
                { 
                    Month = m, 
                    Views = Math.Max(5, monthViews), 
                    Contacts = Math.Max(0, (int)(monthViews * 0.05)) 
                });
            }

            dashboard.Performance.TopArticles = articles
                .OrderByDescending(a => a.ViewsCount)
                .Take(5)
                .Select(a => new ArticlePerformance { Title = a.Title, Views = a.ViewsCount, Contacts = a.ContactsCount })
                .ToList();

            dashboard.Performance.CategoryStats = articles
                .Where(a => a.Category != null)
                .GroupBy(a => a.Category!.Name)
                .Select(g => new CategoryDistribution { Name = g.Key, Count = g.Count() })
                .ToList();

            // --- 3. COMPTABILITÉ ---
            // CAC Estimé
            var leads = Math.Max(1, totalContacts);
            dashboard.Accounting.CostPerLead = Math.Round(subscriptionCost / leads, 2);
            
            // ROI
            // On suppose que 10% des leads convertissent en vente réelle
            var realSalesVolume = dashboard.Overview.PotentialRevenue * 0.1m;
            if (subscriptionCost > 0)
            {
                dashboard.Accounting.SubscriptionROI = Math.Round((double)((realSalesVolume - subscriptionCost) / subscriptionCost * 100), 2);
            }
            
            dashboard.Accounting.CAC = dashboard.Accounting.CostPerLead * 10; // Hypothèse 1 client pour 10 leads
            dashboard.Accounting.AverageQuoteValue = avgPrice;

            var articleCount = Math.Max(1, articles.Count);
            dashboard.Accounting.ArticleCosts = articles.Select(a => new ArticleCostAnalysis
            {
                Title = a.Title,
                // Coût par vue = (Coût Abo / Nb Articles) / Vues
                CostPerView = a.ViewsCount > 0 ? Math.Round((subscriptionCost / articleCount) / a.ViewsCount, 4) : 0,
                IsProfitable = a.ContactsCount > 0 // Simple indicateur
            }).Take(5).ToList();


            // --- 4. INSIGHTS ---
            if (totalViews < 100) dashboard.Insights.Alerts.Add("⚠️ Vos articles manquent de visibilité. Améliorez les mots-clés.");
            if (dashboard.Overview.EngagementRate < 0.5) dashboard.Insights.Alerts.Add("📉 Beaucoup de vues mais peu de contacts. Vérifiez vos prix.");
            
            if (dashboard.Performance.CategoryStats.Any())
            {
                var topCat = dashboard.Performance.CategoryStats.OrderByDescending(c => c.Count).First();
                dashboard.Insights.Recommendations.Add($"💡 La catégorie '{topCat.Name}' attire le plus d'attention. Publiez plus d'offres ici.");
            }
            dashboard.Insights.Recommendations.Add("📅 Le mardi matin est le moment idéal pour publier de nouveaux produits.");

            return Ok(dashboard);
        }
    }
}
