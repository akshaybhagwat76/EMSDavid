using Abp.Application.Services.Dto;

namespace Ems.Telematics.Dtos
{
    public class GetSomeUsageMetricsInput : PagedAndSortedResultRequestDto
    {
        public string RelatedEntity { get; set; }

        public int ReferenceId { get; set; }
    }
}
