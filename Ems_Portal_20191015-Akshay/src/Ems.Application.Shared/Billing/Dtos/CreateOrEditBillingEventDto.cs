
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditBillingEventDto : EntityDto<int?>
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