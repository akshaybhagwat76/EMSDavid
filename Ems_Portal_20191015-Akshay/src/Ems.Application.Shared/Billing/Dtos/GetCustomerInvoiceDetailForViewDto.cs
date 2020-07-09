namespace Ems.Billing.Dtos
{
    public class GetCustomerInvoiceDetailForViewDto
    {
		public CustomerInvoiceDetailDto CustomerInvoiceDetail { get; set; }

		public string CustomerInvoiceDescription { get; set;}

        public string ItemTypeType { get; set; }

        public string UomUnitOfMeasurement { get; set; }

        public string ActionWorkOrderAction { get; set; }

        public string LeaseItemItem { get; set; }
    }
}