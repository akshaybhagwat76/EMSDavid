using System.Collections.Generic;

namespace Ems.Tenants.Dashboard.Dto
{
    public class GetDashboardDataOutput
    {
        public int NewIncidents { get; set; }
        public int NewIncidentsChange { get; set; }

        public int NewWorkOrders { get; set; }
        public int NewWorkOrdersChange { get; set; }

        public int NewEstimates { get; set; }
        public int NewEstimatesChange { get; set; }

        public int NewQuotations { get; set; }
        public int NewQuotationsChange { get; set; }

        public List<AgedReceivablesData> AgedReceivables { get; set; }
        public List<RevenueForecastData> RevenueForecast { get; set; }

        public string AgedReceivablesDate1 { get; set; }
        public string AgedReceivablesDate2 { get; set; }
        public string AgedReceivablesDate3 { get; set; }
        public string AgedReceivablesDate4 { get; set; }
        public int AgedReceivablesTotal1 { get; set; }
        public int AgedReceivablesTotal2 { get; set; }
        public int AgedReceivablesTotal3 { get; set; }
        public int AgedReceivablesTotal4 { get; set; }



        public int TotalProfit { get; set; }

        public int NewFeedbacks { get; set; }

        public int NewOrders { get; set; }

        public int NewUsers { get; set; }
        public int NewUsersChange { get; set; }

        public List<SalesSummaryData> SalesSummary { get; set; }

        public int TotalSales { get; set; }

        public int Revenue { get; set; }

        public int Expenses { get; set; }

        public int Growth { get; set; }

        public int TransactionPercent { get; set; }


        public int NewVisitPercent { get; set; }

        public int BouncePercent { get; set; }

        public int[] DailySales { get; set; }

        public int[] ProfitShares { get; set; }

    }
}