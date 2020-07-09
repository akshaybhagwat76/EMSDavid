using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllIncidentUpdatesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxUpdatedFilter { get; set; }
		public DateTime? MinUpdatedFilter { get; set; }

		public string UpdateFilter { get; set; }


		 public string UserNameFilter { get; set; }

		 		 public string IncidentDescriptionFilter { get; set; }

		 
    }
}