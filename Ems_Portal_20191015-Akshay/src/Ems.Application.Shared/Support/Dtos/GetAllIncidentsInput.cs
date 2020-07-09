using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllIncidentsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DescriptionFilter { get; set; }

		public DateTime? MaxIncidentDateFilter { get; set; }
		public DateTime? MinIncidentDateFilter { get; set; }

		public string LocationFilter { get; set; }

		public string RemarksFilter { get; set; }

		public DateTime? MaxResolvedAtFilter { get; set; }
		public DateTime? MinResolvedAtFilter { get; set; }


		 public string IncidentPriorityPriorityFilter { get; set; }

		 		 public string IncidentStatusStatusFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 		 public string AssetReferenceFilter { get; set; }

		 		 public string SupportItemDescriptionFilter { get; set; }

		 		 public string IncidentTypeTypeFilter { get; set; }

		 		 public string UserNameFilter { get; set; }

		 
    }
}