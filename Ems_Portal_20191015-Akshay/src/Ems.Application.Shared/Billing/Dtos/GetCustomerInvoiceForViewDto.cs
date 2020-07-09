using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Customers.Dtos;
using Ems.Organizations.Dtos;

namespace Ems.Billing.Dtos
{
    public class GetCustomerInvoiceForViewDto
    {
		public CustomerInvoiceDto CustomerInvoice { get; set; }

		public string CustomerName { get; set;}

        public string WorkOrderSubject { get; set; }

        public string EstimateTitle { get; set; }

        public string CurrencyCode { get; set;}

		public string BillingRuleName { get; set;}

		public string BillingEventPurpose { get; set;}

		public string InvoiceStatusStatus { get; set;}

        public PagedResultDto<GetCustomerInvoiceDetailForViewDto> CustomerInvoiceDetails { get; set; }

        public AssetOwnerDto AssetOwnerInfo { get; set; }

        public CustomerDto CustomerInfo { get; set; }

        public AddressDto CustomerAddress { get; set; }

        public ContactDto CustomerContact { get; set; }

        public string AuthenticationKey { get; set; }
    }
}