using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllBillingRulesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public int IsParentFilter { get; set; }

		public int? MaxParentBillingRuleRefIdFilter { get; set; }
		public int? MinParentBillingRuleRefIdFilter { get; set; }

		public decimal? MaxChargePerUnitFilter { get; set; }
		public decimal? MinChargePerUnitFilter { get; set; }


		 public string BillingRuleTypeTypeFilter { get; set; }

		 		 public string UsageMetricMetricFilter { get; set; }

		 		 public string LeaseAgreementTitleFilter { get; set; }

		 		 public string VendorNameFilter { get; set; }

		 		 public string LeaseItemItemFilter { get; set; }

		 		 public string CurrencyCodeFilter { get; set; }

		 
    }
}