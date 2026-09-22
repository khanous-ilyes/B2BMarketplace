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
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,server_admin")] 
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminAnalyticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
        {
            var dashboard = new AdminDashboardDto();
            var today = DateTime.UtcNow;

            // 1. REAL DATABASE METRICS
            // -------------------------
            var totalUsers = await _context.Users.CountAsync();
            var suppliersCount = await _context.Suppliers.CountAsync();
            var articlesCount = await _context.Articles.CountAsync();
            var publishedArticles = await _context.Articles.CountAsync(a => a.Status == ArticleStatus.Published);
            var draftArticles = await _context.Articles.CountAsync(a => a.Status == ArticleStatus.Draft);

            // Clients = Users who are not Suppliers (Simple Logic)
            // Or use Client table if populated. Using User count is safer for general Overview.
            var clientsCount = Math.Max(0, totalUsers - suppliersCount);

            dashboard.Overview.TotalUsers = totalUsers;
            dashboard.Overview.ActiveSuppliers = suppliersCount;
            dashboard.Overview.ActiveClients = clientsCount;
            dashboard.Overview.ActiveArticles = publishedArticles;

            // 2. FINANCIALS (CALCULATED)
            // ---------------------------
            // Hypothesis: 
            // - Subscription = 49.00€ / month / supplier
            // - Commission = 0 (Subscription Model)
            decimal avgSubscriptionPrice = 49.00m;
            
            dashboard.Overview.MRR = suppliersCount * avgSubscriptionPrice;
            dashboard.Overview.ARR = dashboard.Overview.MRR * 12;
            dashboard.Overview.ARPU = avgSubscriptionPrice; // Avg Revenue Per User (Supplier)
            
            // LTV (Lifetime Value) = ARPU * Lifetime (e.g. 24 months)
            dashboard.Overview.LTV = dashboard.Overview.ARPU * 24; 
            dashboard.Overview.CAC = 150m; // Cost Acquisition Customer (Simulated fixed constant)
            
            if (dashboard.Overview.CAC > 0)
                dashboard.Overview.LTV_CAC_Ratio = (double)Math.Round(dashboard.Overview.LTV / dashboard.Overview.CAC, 2);

            // Revenue Growth Curve (Simulated based on Current MRR)
            // We project backwards: assuming we grew 10% per month to reach current MRR
            for(int i = 11; i >= 0; i--)
            {
               var date = today.AddMonths(-i);
               // Growth factor: 0.9^i (decay backwards)
               decimal factor = (decimal)Math.Pow(0.9, i); 
               
               // If MRR is 0 (no suppliers), use a small dummy value for graph aesthetics or 0
               decimal value = dashboard.Overview.MRR > 0 ? dashboard.Overview.MRR * factor : 0;
               
               dashboard.Financials.RevenueGrowth.Add(new RevenueStat 
               { 
                   Month = date.ToString("MMM"), 
                   Revenue = Math.Round(value, 0) 
               });
            }

            dashboard.Financials.GrossMargin = 95; // Software Margin
            dashboard.Financials.BurnRate = 1200; // Server costs etc
            dashboard.Financials.RunwayMonths = dashboard.Overview.MRR > dashboard.Financials.BurnRate 
                ? 99 
                : (int)((10000) / (dashboard.Financials.BurnRate - dashboard.Overview.MRR + 1)); // 10k cash / net burn

            // 3. MARKETPLACE (REAL)
            // ---------------------
            dashboard.Marketplace.CatalogSize = publishedArticles;
            dashboard.Marketplace.ModerationPending = draftArticles;
            dashboard.Overview.MatchingRate = articlesCount > 0 
                ? Math.Round((double)publishedArticles / articlesCount * 100, 1) 
                : 0;

            // 4. USERS FUNNEL (HYBRID)
            // ------------------------
            // Visiteurs (Simulé) -> Inscrits (Reel) -> Fournisseurs (Reel)
            dashboard.Users.Funnel.Add(new StageCount { Stage = "Visiteurs Site", Users = totalUsers * 15 + 100, ConversionRate = 100 });
            dashboard.Users.Funnel.Add(new StageCount { Stage = "Inscrits (Total)", Users = totalUsers, ConversionRate = 6.6 }); // 1/15 approx
            dashboard.Users.Funnel.Add(new StageCount { Stage = "Fournisseurs", Users = suppliersCount, ConversionRate = totalUsers > 0 ? Math.Round((double)suppliersCount/totalUsers*100, 1) : 0 });
            dashboard.Users.Funnel.Add(new StageCount { Stage = "Clients", Users = clientsCount, ConversionRate = totalUsers > 0 ? Math.Round((double)clientsCount/totalUsers*100, 1) : 0 });

            dashboard.Users.Engagement.DAU = (int)(totalUsers * 0.2); // 20% active daily
            dashboard.Users.Engagement.MAU = (int)(totalUsers * 0.6); // 60% active monthly

            // 5. MARKETING & TECH (STATIC FOR DEMO)
            // -------------------------------------
            // Hard to get real data without Google Analytics integration
            dashboard.Marketing.AcquisitionChannels.Add(new ChannelStat { Source = "Direct", Visits = 450, Conversion = 2.5, ROI = 0 });
            dashboard.Marketing.AcquisitionChannels.Add(new ChannelStat { Source = "SEO", Visits = 1200, Conversion = 1.8, ROI = 100 });
            
            dashboard.Technical.Uptime = 99.99;
            dashboard.Technical.AvgResponseTimeMs = 85; 
            dashboard.Technical.OpenSupportTickets = 0; // Or check a Tickets table if it existed

            return Ok(dashboard);
        }
    }
}
