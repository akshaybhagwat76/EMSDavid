
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditConsumableTypeDto : EntityDto<int?>
    {

		[Required]
		public string Type { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}