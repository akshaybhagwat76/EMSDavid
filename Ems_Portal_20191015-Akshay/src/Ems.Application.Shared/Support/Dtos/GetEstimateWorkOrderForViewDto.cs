using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetEstimateWorkOrderForViewDto
    {
        public GetWorkOrderForViewDto WorkOrder { get; set; }

        public PagedResultDto<GetWorkOrderUpdateForViewDto> WorkOrderUpdates { get; set; }
    }
}
