
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditWorkOrderPriorityDto : EntityDto<int?>
    {

		[Required]
		public string Priority { get; set; }
		
		
		public int PriorityLevel { get; set; }
		
		

    }
}