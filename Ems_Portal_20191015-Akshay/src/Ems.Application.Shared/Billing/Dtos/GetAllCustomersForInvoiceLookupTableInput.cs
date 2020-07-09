using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class GetAllCustomersForInvoiceLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int EstimateId { get; set; }

        public int WorkOrderId { get; set; }
        
        //public string DescriptionFilter { get; set; }

    }
}
