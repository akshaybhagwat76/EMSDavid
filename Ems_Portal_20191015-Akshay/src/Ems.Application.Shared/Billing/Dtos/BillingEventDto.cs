
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class BillingEventDto : EntityDto
    {
		public DateTime BillingEventDate { get; set; }

		public string TriggeredBy { get; set; }

		public string Purpose { get; set; }

		public bool WasInvoiceGenerated { get; set; }


		 public int? LeaseAgreementId { get; set; }

		 		 public int? VendorChargeId { get; set; }

		 		 public int BillingEventTypeId { get; set; }

		 
    }
}