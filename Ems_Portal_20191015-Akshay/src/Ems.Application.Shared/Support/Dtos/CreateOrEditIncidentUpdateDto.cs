
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditIncidentUpdateDto : EntityDto<int?>
    {

		public DateTime Updated { get; set; }
		
		
		public string Update { get; set; }
		
		
		 public long UserId { get; set; }
		 
		 		 public int IncidentId { get; set; }
		 
		 
    }
}