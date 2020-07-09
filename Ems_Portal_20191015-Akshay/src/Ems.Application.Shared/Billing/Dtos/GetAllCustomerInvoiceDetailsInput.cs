using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllCustomerInvoiceDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DescriptionFilter { get; set; }

		public decimal? MaxQuantityFilter { get; set; }
		public decimal? MinQuantityFilter { get; set; }

		public decimal? MaxUnitPriceFilter { get; set; }
		public decimal? MinUnitPriceFilter { get; set; }

		public decimal? MaxGrossFilter { get; set; }
		public decimal? MinGrossFilter { get; set; }

		public decimal? MaxTaxFilter { get; set; }
		public decimal? MinTaxFilter { get; set; }

		public decimal? MaxNetFilter { get; set; }
		public decimal? MinNetFilter { get; set; }

		public decimal? MaxDiscountFilter { get; set; }
		public decimal? MinDiscountFilter { get; set; }

		public decimal? MaxChargeFilter { get; set; }
		public decimal? MinChargeFilter { get; set; }

		public int? MaxBillingRuleRefIdFilter { get; set; }
		public int? MinBillingRuleRefIdFilter { get; set; }


		 public string LeaseItemItemFilter { get; set; }

		 		 public string CustomerInvoiceDescriptionFilter { get; set; }

		 
    }
}