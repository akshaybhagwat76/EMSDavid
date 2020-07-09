using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IQuotationDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetQuotationDetailForViewDto>> GetAll(GetAllQuotationDetailsInput input);

        Task<GetQuotationDetailForViewDto> GetQuotationDetailForView(int id);

		Task<GetQuotationDetailForEditOutput> GetQuotationDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditQuotationDetailDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetQuotationDetailsToExcel(GetAllQuotationDetailsForExcelInput input);

		
		Task<PagedResultDto<QuotationDetailQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<QuotationDetailWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);

        void UpdateQuotationPrices(int quotationId);
    }
}