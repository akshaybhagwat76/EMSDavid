
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class CreateOrEditCustomerTypeDto : EntityDto<int?>
    {

		[Required]
		public string Type { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}