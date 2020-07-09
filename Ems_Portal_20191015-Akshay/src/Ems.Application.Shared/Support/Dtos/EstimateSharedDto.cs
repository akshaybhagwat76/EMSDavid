using System.Collections.Generic;

namespace Ems.Support.Dtos
{
    public class EstimateWorkOrderFkListDto
    {
        public List<EstimateCustomerLookupTableDto> CustomerList { get; set; }

        public List<EstimateQuotationLookupTableDto> QuotationList { get; set; }
    }

    public class EstimateQuotationFkListDto
    {
        public List<EstimateCustomerLookupTableDto> CustomerList { get; set; }
    }
}
