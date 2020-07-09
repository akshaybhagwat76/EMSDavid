
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditBillingEventDetailDto : EntityDto<int?>
    {

		public bool RuleExecutedSuccessfully { get; set; }
		
		
		public string Exception { get; set; }
		
		
		 public int? BillingRuleId { get; set; }
		 
		 		 public int? LeaseItemId { get; set; }
		 
		 		 public int? BillingEventId { get; set; }
		 
		 
    }
}