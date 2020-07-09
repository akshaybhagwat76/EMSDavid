using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllBillingEventsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxBillingEventDateFilter { get; set; }
		public DateTime? MinBillingEventDateFilter { get; set; }

		public string TriggeredByFilter { get; set; }

		public string PurposeFilter { get; set; }

		public int WasInvoiceGeneratedFilter { get; set; }


		 public string LeaseAgreementTitleFilter { get; set; }

		 		 public string VendorChargeReferenceFilter { get; set; }

		 		 public string BillingEventTypeTypeFilter { get; set; }

		 
    }
}