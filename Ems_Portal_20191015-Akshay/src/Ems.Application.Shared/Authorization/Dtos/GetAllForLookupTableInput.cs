using Abp.Application.Services.Dto;

namespace Ems.Authorization.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}