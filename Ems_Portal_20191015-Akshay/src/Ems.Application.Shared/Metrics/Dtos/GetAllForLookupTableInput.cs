using Abp.Application.Services.Dto;

namespace Ems.Metrics.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}