
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class CreateOrEditBillingEventTypeDto : EntityDto<int?>
    {

		[Required]
		public string Type { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}