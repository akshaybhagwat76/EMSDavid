using Abp.Application.Services.Dto;

namespace Ems.Finance.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}