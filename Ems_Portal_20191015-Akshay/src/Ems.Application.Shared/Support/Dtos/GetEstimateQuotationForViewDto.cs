using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;

namespace Ems.Support.Dtos
{
    public class GetEstimateQuotationForViewDto
    {
        public GetQuotationForViewDto Quotation { get; set; }

        public PagedResultDto<GetQuotationDetailForViewDto> QuotationDetails { get; set; }
    }
}
