
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class BillingEventDetailDto : EntityDto
    {
		public bool RuleExecutedSuccessfully { get; set; }

		public string Exception { get; set; }


		 public int? BillingRuleId { get; set; }

		 		 public int? LeaseItemId { get; set; }

		 		 public int? BillingEventId { get; set; }

		 
    }
}