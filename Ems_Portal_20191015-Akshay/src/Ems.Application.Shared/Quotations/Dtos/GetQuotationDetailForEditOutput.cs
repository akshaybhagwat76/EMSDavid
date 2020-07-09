namespace Ems.Quotations.Dtos
{
    public class GetQuotationDetailForEditOutput
    {
		public CreateOrEditQuotationDetailDto QuotationDetail { get; set; }

		public string ItemTypeType { get; set;}

		public string QuotationTitle { get; set;}

		public string UomUnitOfMeasurement { get; set;}

    }
}