using Abp.Application.Services.Dto;
using System;

namespace Ems.Finance.Dtos
{
    public class GetAllAgedReceivablesPeriodsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxPeriodFilter { get; set; }
		public DateTime? MinPeriodFilter { get; set; }

		public decimal? MaxCurrentFilter { get; set; }
		public decimal? MinCurrentFilter { get; set; }

		public decimal? MaxOver30Filter { get; set; }
		public decimal? MinOver30Filter { get; set; }

		public decimal? MaxOver60Filter { get; set; }
		public decimal? MinOver60Filter { get; set; }

		public decimal? MaxOver90Filter { get; set; }
		public decimal? MinOver90Filter { get; set; }

		public decimal? MaxOver120Filter { get; set; }
		public decimal? MinOver120Filter { get; set; }



    }
}