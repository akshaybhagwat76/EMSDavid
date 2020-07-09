using Abp.Application.Services.Dto;

namespace Ems.Storage.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}