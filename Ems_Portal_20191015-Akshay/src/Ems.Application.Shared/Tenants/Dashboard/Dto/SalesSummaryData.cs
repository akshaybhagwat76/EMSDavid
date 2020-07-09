namespace Ems.Tenants.Dashboard.Dto
{
    public class SalesSummaryData
    {
        public string Period { get; set; }
        public int Sales { get; set; }
        public int Profit { get; set; }

        public SalesSummaryData(string period, int sales, int profit)
        {
            Period = period;
            Sales = sales;
            Profit = profit;

        }
    }

    public class RevenueForecastData
    {
        public string Period { get; set; }
        public int Revenue { get; set; }

        public RevenueForecastData(string period, int revenue)
        {
            Period = period;
            Revenue = revenue;
        }
    }


    public class AgedReceivablesData
    {
        public string Period { get; set; }
        public int Current { get; set; }
        public int Over30 { get; set; }
        public int Over60 { get; set; }
        public int Over90 { get; set; }
        public int Over120 { get; set; }

        public AgedReceivablesData(string period, int current, int over30, int over60, int over90, int over120)
        {
            Period = period;
            Current = current;
            Over30 = over30;
            Over60 = over60;
            Over90 = over90;
            Over120 = over120;
        }
    }
}