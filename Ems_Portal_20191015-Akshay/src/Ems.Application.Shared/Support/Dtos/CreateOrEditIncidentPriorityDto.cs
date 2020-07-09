
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditIncidentPriorityDto : EntityDto<int?>
    {

		[Required]
		public string Priority { get; set; }
		
		
		[Required]
		public string Description { get; set; }
		
		
		public int PriorityLevel { get; set; }
		
		

    }
}