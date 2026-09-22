using System.Collections.Generic;

namespace BaseLibrary.DTOs
{
    public class SupplierDashboardDto
    {
        public OverviewDto Overview { get; set; } = new();
        public PerformanceAnalysisDto Performance { get; set; } = new();
        public AccountingAnalyticsDto Accounting { get; set; } = new();
        public InsightsDto Insights { get; set; } = new();
    }

    public class OverviewDto
    {
        public decimal PotentialRevenue { get; set; } // Somme devis en cours
        public double ConversionRate { get; set; } // Devis assignés / Total devis
        public int TotalViews { get; set; } // Consultations ce mois
        public double EngagementRate { get; set; } // Contacts / Vues
        public int ActiveArticles { get; set; } // Articles publiés
        public int SubscriptionLimit { get; set; } = 50; // Limite exemple
        
        // Trends strings (e.g. "+12%")
        public string RevenueTrend { get; set; } = "0%";
        public string ConversionTrend { get; set; } = "0%";
        public string ViewsTrend { get; set; } = "0%";
        public string EngagementTrend { get; set; } = "0%";
    }

    public class PerformanceAnalysisDto
    {
        public List<MonthlyStat> MonthlyEvolution { get; set; } = new();
        public List<ArticlePerformance> TopArticles { get; set; } = new();
        public List<CategoryDistribution> CategoryStats { get; set; } = new();
        public List<ConversionTrend> ConversionTrends { get; set; } = new();
    }

    public class AccountingAnalyticsDto
    {
        public decimal CAC { get; set; } // Coût acquisition client
        public double SubscriptionROI { get; set; } // ROI 
        public decimal CostPerLead { get; set; } // Coût par lead
        public decimal AverageQuoteValue { get; set; } // Valeur moyenne devis
        public List<ArticleCostAnalysis> ArticleCosts { get; set; } = new();
    }

    public class InsightsDto
    {
        public List<string> Alerts { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    // Helper classes
    public class MonthlyStat { public string Month { get; set; } = ""; public int Views { get; set; } public int Contacts { get; set; } }
    public class ArticlePerformance { public string Title { get; set; } = ""; public int Views { get; set; } public int Contacts { get; set; } }
    public class CategoryDistribution { public string Name { get; set; } = ""; public int Count { get; set; } }
    public class ConversionTrend { public string Date { get; set; } = ""; public int Rate { get; set; } }
    public class ArticleCostAnalysis { public string Title { get; set; } = ""; public decimal CostPerView { get; set; } public bool IsProfitable { get; set; } }
}
