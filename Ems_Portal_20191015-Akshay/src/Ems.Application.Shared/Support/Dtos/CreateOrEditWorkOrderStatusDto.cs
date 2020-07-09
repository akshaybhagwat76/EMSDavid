
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderStatusDto : EntityDto<int?>
    {

		[Required]
		public string Status { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		

    }
}