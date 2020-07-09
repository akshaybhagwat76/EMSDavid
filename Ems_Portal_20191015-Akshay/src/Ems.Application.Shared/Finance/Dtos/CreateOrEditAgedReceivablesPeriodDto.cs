
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Finance.Dtos
{
    public class CreateOrEditAgedReceivablesPeriodDto : EntityDto<int?>
    {

		public DateTime Period { get; set; }
		
		
		public decimal Current { get; set; }
		
		
		public decimal Over30 { get; set; }
		
		
		public decimal Over60 { get; set; }
		
		
		public decimal Over90 { get; set; }
		
		
		public decimal Over120 { get; set; }
		
		

    }
}