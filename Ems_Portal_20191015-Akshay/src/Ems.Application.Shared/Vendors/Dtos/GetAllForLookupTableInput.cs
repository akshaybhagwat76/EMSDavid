using Abp.Application.Services.Dto;

namespace Ems.Vendors.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}