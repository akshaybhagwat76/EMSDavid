using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllCurrenciesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string CodeFilter { get; set; }

		public string NameFilter { get; set; }

		public string SymbolFilter { get; set; }

		public string CountryFilter { get; set; }

		public string BaseCountryFilter { get; set; }



    }
}