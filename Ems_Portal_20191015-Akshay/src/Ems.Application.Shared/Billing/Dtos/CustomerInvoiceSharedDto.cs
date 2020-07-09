using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using System.Collections.Generic;

namespace Ems.Billing.Dtos
{
    public class CustomerInvoiceWorkOrderFkListDto
    {
        public List<CustomerInvoiceCustomerLookupTableDto> CustomerList { get; set; }

        //public List<CustomerInvoiceLeaseItemLookupTableDto> LeaseItemList { get; set; }

        public List<CustomerInvoiceEstimateLookupTableDto> EstimateList { get; set; }
    }

    public class CustomerInvoiceEstimateFkListDto
    {
        public List<CustomerInvoiceCustomerLookupTableDto> CustomerList { get; set; }

        //public List<CustomerInvoiceLeaseItemLookupTableDto> LeaseItemList { get; set; }
    }

    public class GetCustomerInvoiceEstimateForViewDto
    {
        public GetEstimateForViewDto Estimate { get; set; }

        public PagedResultDto<GetEstimateDetailForViewDto> EstimateDetails { get; set; }
    }

    public class GetCustomerInvoiceWorkOrderForViewDto
    {
        public GetWorkOrderForViewDto WorkOrder { get; set; }

        public PagedResultDto<GetWorkOrderUpdateForViewDto> WorkOrderUpdates { get; set; }
    }
}
