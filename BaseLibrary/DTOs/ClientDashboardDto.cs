using System.Collections.Generic;

namespace BaseLibrary.DTOs
{
    public class ClientDashboardDto
    {
        public ClientOverviewDto Overview { get; set; } = new();
        public ClientAnalyticsDto Analytics { get; set; } = new();
        public ClientAccountingDto Accounting { get; set; } = new();
        public ClientRecommendationsDto Recommendations { get; set; } = new();
    }

    public class ClientOverviewDto
    {
        public int ActiveRequests { get; set; }
        public double SupplierResponseRate { get; set; } // %
        public string AvgResponseTime { get; set; } = string.Empty; // "2h 30m"
        public decimal TotalSavings { get; set; }
        public int SuppliersContacted { get; set; }
        public int FavoriteArticles { get; set; }
    }

    public class ClientAnalyticsDto
    {
        public PipelineStats Pipeline { get; set; } = new();
        public List<QuoteComparison> QuoteComparisons { get; set; } = new();
        public List<MonthlyStat> MonthlyRequests { get; set; } = new();
    }

    public class ClientAccountingDto
    {
        public decimal AvgSavingsPerRequest { get; set; }
        public double AvgSavingsRate { get; set; } // %
        public double PlatformROI { get; set; } // %
        public List<SupplierPerformance> TopSuppliers { get; set; } = new();
        public BudgetStatus Budget { get; set; } = new();
    }

    public class ClientRecommendationsDto
    {
        public List<string> Alerts { get; set; } = new();
        public List<string> Suggestions { get; set; } = new();
    }

    // Helpers
    public class PipelineStats { public int Pending { get; set; } public int QuotesReceived { get; set; } public int Accepted { get; set; } }
    public class QuoteComparison { public string RequestTitle { get; set; } = ""; public int QuotesCount { get; set; } public decimal MinPrice { get; set; } public decimal MaxPrice { get; set; } }
    public class SupplierPerformance { public string Name { get; set; } = ""; public double Rating { get; set; } public string PriceLevel { get; set; } = ""; }
    public class BudgetStatus { public decimal Allocated { get; set; } public decimal Spent { get; set; } public decimal Forecast { get; set; } }
}
