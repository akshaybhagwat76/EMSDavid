using System.Collections.Generic;

namespace Ems.Tenants.Dashboard.Dto
{
    public class GetSalesSummaryOutput
    {
        public GetSalesSummaryOutput(List<SalesSummaryData> salesSummary)
        {
            SalesSummary = salesSummary;
        }

        public List<SalesSummaryData> SalesSummary { get; set; }
    }

    public class GetAgedReceivablesOutput
    {
        public GetAgedReceivablesOutput(List<AgedReceivablesData> agedReceivables)
        {
            AgedReceivables = agedReceivables;
        }

        public List<AgedReceivablesData> AgedReceivables { get; set; }
    }

    public class GetRevenueForecastOutput
    {
        public GetRevenueForecastOutput(List<RevenueForecastData> revenueForecast)
        {
            RevenueForecast = revenueForecast;
        }

        public List<RevenueForecastData> RevenueForecast { get; set; }
    }

}