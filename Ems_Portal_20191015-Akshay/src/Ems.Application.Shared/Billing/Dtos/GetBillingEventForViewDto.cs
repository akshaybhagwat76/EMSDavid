namespace Ems.Billing.Dtos
{
    public class GetBillingEventForViewDto
    {
		public BillingEventDto BillingEvent { get; set; }

		public string LeaseAgreementTitle { get; set;}

		public string VendorChargeReference { get; set;}

		public string BillingEventTypeType { get; set;}


    }
}