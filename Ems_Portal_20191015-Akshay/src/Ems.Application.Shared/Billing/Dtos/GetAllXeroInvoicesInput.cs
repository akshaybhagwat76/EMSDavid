using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllXeroInvoicesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public int XeroInvoiceCreatedFilter { get; set; }

		public string ApiResponseFilter { get; set; }

		public int FailedFilter { get; set; }

		public string ExceptionFilter { get; set; }

		public string XeroReferenceFilter { get; set; }


		 public string CustomerInvoiceCustomerReferenceFilter { get; set; }

		 
    }
}