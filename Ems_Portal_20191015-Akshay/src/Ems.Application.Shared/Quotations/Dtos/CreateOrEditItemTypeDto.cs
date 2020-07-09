
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class CreateOrEditItemTypeDto : EntityDto<int?>
    {

		[Required]
		public string Type { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}