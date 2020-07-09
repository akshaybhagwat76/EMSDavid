using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class WorkOrderActionDto : EntityDto
    {
        public string Action { get; set; }

        public string Description { get; set; }
    }
}
