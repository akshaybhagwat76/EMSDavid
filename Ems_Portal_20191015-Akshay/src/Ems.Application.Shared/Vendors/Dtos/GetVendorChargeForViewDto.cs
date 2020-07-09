namespace Ems.Vendors.Dtos
{
    public class GetVendorChargeForViewDto
    {
		public VendorChargeDto VendorCharge { get; set; }

		public string VendorName { get; set;}

		public string SupportContractTitle { get; set;}

		public string WorkOrderSubject { get; set;}

		public string VendorChargeStatusStatus { get; set;}


    }
}