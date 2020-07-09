using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetAllEstimateDetailsInput : PagedAndSortedResultRequestDto
    {
        public int EstimateId { get; set; }

        public string Filter { get; set; }

        public string ItemTypeTypeFilter { get; set; }

        public string EstimateTitleFilter { get; set; }

        public string UomUnitOfMeasurementFilter { get; set; }

        public string ActionWorkOrderActionFilter { get; set; }
    }
}
