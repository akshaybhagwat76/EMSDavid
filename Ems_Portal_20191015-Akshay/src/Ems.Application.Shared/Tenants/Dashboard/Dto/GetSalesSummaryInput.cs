namespace Ems.Tenants.Dashboard.Dto
{
    public class GetSalesSummaryInput
    {
        public SalesSummaryDatePeriod SalesSummaryDatePeriod { get; set; }
    }

    public class GetAgedReceivablesInput
    {
        public AgedReceivablesDatePeriod AgedReceivablesDatePeriod { get; set; }
    }
}