using Abp.Application.Services.Dto;
using System;

namespace Ems.Vendors.Dtos
{
    public class GetAllVendorChargesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public DateTime? MaxDateIssuedFilter { get; set; }
		public DateTime? MinDateIssuedFilter { get; set; }

		public DateTime? MaxDateDueFilter { get; set; }
		public DateTime? MinDateDueFilter { get; set; }

		public decimal? MaxTotalTaxFilter { get; set; }
		public decimal? MinTotalTaxFilter { get; set; }

		public decimal? MaxTotalPriceFilter { get; set; }
		public decimal? MinTotalPriceFilter { get; set; }


		 public string VendorNameFilter { get; set; }

		 		 public string SupportContractTitleFilter { get; set; }

		 		 public string WorkOrderSubjectFilter { get; set; }

		 		 public string VendorChargeStatusStatusFilter { get; set; }

		 
    }
}