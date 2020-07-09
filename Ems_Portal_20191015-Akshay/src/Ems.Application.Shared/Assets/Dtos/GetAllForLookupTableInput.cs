using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}