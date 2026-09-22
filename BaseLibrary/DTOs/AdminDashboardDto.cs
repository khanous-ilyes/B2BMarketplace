using System.Collections.Generic;

namespace BaseLibrary.DTOs
{
    public class AdminDashboardDto
    {
        public AdminOverviewDto Overview { get; set; } = new();
        public AdminFinancialsDto Financials { get; set; } = new();
        public AdminUsersDto Users { get; set; } = new();
        public AdminMarketplaceDto Marketplace { get; set; } = new();
        public AdminMarketingDto Marketing { get; set; } = new();
        public AdminTechnicalDto Technical { get; set; } = new();
    }

    public class AdminOverviewDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSuppliers { get; set; }
        public int ActiveClients { get; set; }
        public decimal MRR { get; set; } // Monthly Recurring Revenue
        public decimal ARR { get; set; } // Annual Recurring Revenue
        public double ChurnRate { get; set; }
        public decimal ARPU { get; set; } // Avg Revenue Per User
        public decimal LTV { get; set; } // Lifetime Value
        public decimal CAC { get; set; } // Customer Acquisition Cost
        public double LTV_CAC_Ratio { get; set; } // LTV/CAC
        public int ActiveArticles { get; set; }
        public double MatchingRate { get; set; } // %
    }

    public class AdminFinancialsDto
    {
        public List<RevenueStat> RevenueGrowth { get; set; } = new(); // 12 mois
        public decimal GrossMargin { get; set; } // %
        public decimal EBITDA { get; set; }
        public decimal BurnRate { get; set; }
        public int RunwayMonths { get; set; }
        public List<ScenarioForecast> Forecasts { get; set; } = new();
    }



    public class AdminUsersDto
    {
        public List<StageCount> Funnel { get; set; } = new(); // 7 etapes
        public List<CohortData> Cohorts { get; set; } = new(); // Retention
        public UserEngagement Engagement { get; set; } = new(); // DAU/MAU, NPS
        public List<SegmentAnalysis> Segmentation { get; set; } = new();
    }

    public class AdminMarketplaceDto
    {
        public int CatalogSize { get; set; }
        public int TransactionsLastMonth { get; set; }
        public double CatalogQualityScore { get; set; }
        public int ModerationPending { get; set; }
    }

    public class AdminMarketingDto
    {
        public List<ChannelStat> AcquisitionChannels { get; set; } = new();
        public List<CampaignStat> Campaigns { get; set; } = new();
    }

    public class AdminTechnicalDto
    {
        public double Uptime { get; set; } // 99.9%
        public double AvgResponseTimeMs { get; set; }
        public int OpenSupportTickets { get; set; }
        public List<string> CriticalAlerts { get; set; } = new();
    }

    // Helpers
    public class ScenarioForecast { public string Scenario { get; set; } = ""; public List<decimal> MonthlyValues { get; set; } = new(); }
    public class StageCount { public string Stage { get; set; } = ""; public int Users { get; set; } public double ConversionRate { get; set; } }
    public class CohortData { public string Month { get; set; } = ""; public List<double> RetentionRates { get; set; } = new(); }
    public class UserEngagement { public double DAU { get; set; } public double MAU { get; set; } public double Stickiness => MAU > 0 ? DAU / MAU : 0; public int NPS { get; set; } }
    public class SegmentAnalysis { public string SegmentName { get; set; } = ""; public int Count { get; set; } public decimal RevenueShare { get; set; } }
    public class ChannelStat { public string Source { get; set; } = ""; public int Visits { get; set; } public double Conversion { get; set; } public decimal ROI { get; set; } }
    public class CampaignStat { public string Name { get; set; } = ""; public decimal Cost { get; set; } public int Leads { get; set; } }
    public class RevenueStat { public string Month { get; set; } = ""; public decimal Revenue { get; set; } }
}
