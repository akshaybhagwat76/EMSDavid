using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllBillingEventDetailsForExcelInput
    {
		public string Filter { get; set; }

		public int RuleExecutedSuccessfullyFilter { get; set; }

		public string ExceptionFilter { get; set; }


		 public string BillingRuleNameFilter { get; set; }

		 		 public string LeaseItemItemFilter { get; set; }

		 		 public string BillingEventPurposeFilter { get; set; }

		 
    }
}