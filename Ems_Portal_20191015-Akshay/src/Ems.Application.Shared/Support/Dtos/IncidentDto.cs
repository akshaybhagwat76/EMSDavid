
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class IncidentDto : EntityDto
    {
		public string Description { get; set; }

		public DateTime IncidentDate { get; set; }

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