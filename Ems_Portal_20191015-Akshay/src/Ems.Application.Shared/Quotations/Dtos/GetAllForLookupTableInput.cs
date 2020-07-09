using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }
}