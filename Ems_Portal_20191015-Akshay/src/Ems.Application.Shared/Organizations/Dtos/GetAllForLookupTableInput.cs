using Abp.Application.Services.Dto;

namespace Ems.Organizations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}