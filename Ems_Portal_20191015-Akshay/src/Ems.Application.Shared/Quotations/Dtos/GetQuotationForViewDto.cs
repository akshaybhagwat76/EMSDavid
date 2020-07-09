namespace Ems.Quotations.Dtos
{
    public class GetQuotationForViewDto
    {
		public QuotationDto Quotation { get; set; }

		public string SupportContractTitle { get; set;}

		public string QuotationStatusStatus { get; set;}

		public string WorkOrderSubject { get; set;}

        public string AssetReference { get; set; }

        public string AssetClassClass { get; set; }

        public string SupportTypeType { get; set; }

        public string SupportItemDescription { get; set; }

    }
}