using Abp.Application.Services.Dto;

namespace Ems.Telematics.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}