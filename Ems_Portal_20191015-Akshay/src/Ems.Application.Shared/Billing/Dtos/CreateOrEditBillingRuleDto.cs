
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditBillingRuleDto : EntityDto<int?>
    {

		[Required]
		public string Name { get; set; }
		
		
		public bool IsParent { get; set; }
		
		
		public int? ParentBillingRuleRefId { get; set; }
		
		
		public decimal? ChargePerUnit { get; set; }
		
		
		public string DefaultInvoiceDescription { get; set; }
		
		
		 public int BillingRuleTypeId { get; set; }
		 
		 		 public int UsageMetricId { get; set; }
		 
		 		 public int? LeaseAgreementId { get; set; }
		 
		 		 public int? VendorId { get; set; }
		 
		 		 public int? LeaseItemId { get; set; }
		 
		 		 public int? CurrencyId { get; set; }
		 
		 
    }
}