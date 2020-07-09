
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditCurrencyDto : EntityDto<int?>
    {

		[Required]
		public string Code { get; set; }
		
		
		[Required]
		public string Name { get; set; }
		
		
		public string Symbol { get; set; }
		
		
		[Required]
		public string Country { get; set; }
		
		
		[Required]
		public string BaseCountry { get; set; }
		
		

    }
}