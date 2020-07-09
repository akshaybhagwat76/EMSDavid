using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetIncidentForViewDto
    {
		public IncidentDto Incident { get; set; }

		public string IncidentPriorityPriority { get; set;}

		public string IncidentStatusStatus { get; set;}

		public string CustomerName { get; set;}

		public string AssetReference { get; set;}

		public string SupportItemDescription { get; set;}

		public string IncidentTypeType { get; set;}

		public string UserName { get; set;}

        public PagedResultDto<GetIncidentUpdateForViewDto> IncidentUpdates { get; set; }
    }
}