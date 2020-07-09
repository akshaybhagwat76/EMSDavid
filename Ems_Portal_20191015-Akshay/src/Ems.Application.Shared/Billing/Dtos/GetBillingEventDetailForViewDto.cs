namespace Ems.Billing.Dtos
{
    public class GetBillingEventDetailForViewDto
    {
		public BillingEventDetailDto BillingEventDetail { get; set; }

		public string BillingRuleName { get; set;}

		public string LeaseItemItem { get; set;}

		public string BillingEventPurpose { get; set;}


    }
}