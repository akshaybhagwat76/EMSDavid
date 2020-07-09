
using System;
using Abp.Application.Services.Dto;

namespace Ems.Finance.Dtos
{
    public class AgedReceivablesPeriodDto : EntityDto
    {
		public DateTime Period { get; set; }

		public decimal Current { get; set; }

		public decimal Over30 { get; set; }

		public decimal Over60 { get; set; }

		public decimal Over90 { get; set; }

		public decimal Over120 { get; set; }



    }
}