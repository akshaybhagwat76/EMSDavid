namespace Ems.Quotations.Dtos
{
    public class GetRfqForViewDto
    {
		public RfqDto Rfq { get; set; }

		public string RfqTypeType { get; set;}

		public string AssetOwnerName { get; set;}

		public string CustomerName { get; set;}

		public string AssetClassClass { get; set;}

		public string IncidentDescription { get; set;}

		public string VendorName { get; set;}

		public string UserName { get; set;}


    }
}