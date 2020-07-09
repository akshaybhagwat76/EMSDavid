namespace Ems.Billing.Dtos
{
    public class GetCustomerInvoiceDetailForEditOutput
    {
		public CreateOrEditCustomerInvoiceDetailDto CustomerInvoiceDetail { get; set; }

		public string CustomerInvoiceDescription { get; set;}

        public string ItemTypeType { get; set; }

        public string UomUnitOfMeasurement { get; set; }

        public string ActionWorkOrderAction { get; set; }

        public string LeaseItemItem { get; set; }
    }
}