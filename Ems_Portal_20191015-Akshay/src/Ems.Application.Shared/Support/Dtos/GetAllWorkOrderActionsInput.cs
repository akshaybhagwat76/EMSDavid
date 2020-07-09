using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderActionsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ActionFilter { get; set; }

        public string DescriptionFilter { get; set; }
    }
}
