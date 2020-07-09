
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class CreateOrEditCustomerGroupDto : EntityDto<int?>
    {

		[Required]
		public string Name { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}