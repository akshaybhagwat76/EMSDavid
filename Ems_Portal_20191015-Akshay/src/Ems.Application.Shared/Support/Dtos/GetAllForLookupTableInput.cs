using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }
    }

    public class GetAllSupportItemsForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int AssetId { get; set; }
    }

    public class GetAllCustomersForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int AssetId { get; set; }
    }

    public class GetAllCustomersForEstimateLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int WorkOrderId { get; set; }

        public int QuotationId { get; set; }
    }

    public class GetAllUsingIdForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int FilterId { get; set; }
    }

    

}