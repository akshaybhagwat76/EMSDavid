using Abp.Application.Services.Dto;
using System;

namespace Ems.Vendors.Dtos
{
    public class GetAllVendorsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string NameFilter { get; set; }

		public string IdentifierFilter { get; set; }

		public string VendorLoc8GUIDFilter { get; set; }


		 public string SsicCodeCodeFilter { get; set; }

		 		 public string CurrencyCodeFilter { get; set; }

		 
    }
}