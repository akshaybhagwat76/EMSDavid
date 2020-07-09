namespace Ems.Billing.Dtos
{
    public class GetBillingRuleForViewDto
    {
		public BillingRuleDto BillingRule { get; set; }

		public string BillingRuleTypeType { get; set;}

		public string UsageMetricMetric { get; set;}

		public string LeaseAgreementTitle { get; set;}

		public string VendorName { get; set;}

		public string LeaseItemItem { get; set;}

		public string CurrencyCode { get; set;}


    }
}