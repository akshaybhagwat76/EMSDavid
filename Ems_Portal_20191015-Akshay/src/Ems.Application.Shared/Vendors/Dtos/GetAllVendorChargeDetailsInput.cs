using Abp.Application.Services.Dto;
using System;

namespace Ems.Vendors.Dtos
{
    public class GetAllVendorChargeDetailsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string InvoiceDetailFilter { get; set; }

		public decimal? MaxQuantityFilter { get; set; }
		public decimal? MinQuantityFilter { get; set; }

		public decimal? MaxUnitPriceFilter { get; set; }
		public decimal? MinUnitPriceFilter { get; set; }

		public decimal? MaxTaxFilter { get; set; }
		public decimal? MinTaxFilter { get; set; }

		public decimal? MaxSubTotalFilter { get; set; }
		public decimal? MinSubTotalFilter { get; set; }



    }
}