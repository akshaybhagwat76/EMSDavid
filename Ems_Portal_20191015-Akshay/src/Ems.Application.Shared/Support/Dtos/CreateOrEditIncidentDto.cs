
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Support.Dtos
{
    public class CreateOrEditIncidentDto : EntityDto<int?>
    {

		[Required]
		public string Description { get; set; }
		
		
		public DateTime IncidentDate { get; set; }
		
		
		[Required]
		public string Location { get; set; }
		
		
		public string Remarks { get; set; }
		
		
		
		
		
		public DateTime? ResolvedAt { get; set; }
		
		
		 public int? IncidentPriorityId { get; set; }
		 
		 		 public int? IncidentStatusId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 		 public int? AssetId { get; set; }
		 
		 		 public int? SupportItemId { get; set; }
		 
		 		 public int IncidentTypeId { get; set; }
		 
		 		 public long? UserId { get; set; }
		 
		 
    }
}