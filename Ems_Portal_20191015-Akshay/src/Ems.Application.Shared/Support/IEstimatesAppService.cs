using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;
using System.Collections.Generic;

namespace Ems.Support
{
    public interface IEstimatesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetEstimateForViewDto>> GetAll(GetAllEstimatesInput input);

        Task<GetEstimateForViewDto> GetEstimateForView(int id);

		Task<GetEstimateForEditOutput> GetEstimateForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditEstimateDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetEstimatesToExcel(GetAllEstimatesForExcelInput input);


        Task<PagedResultDto<EstimateWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<EstimateQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<EstimateEstimateStatusLookupTableDto>> GetAllEstimateStatusForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<EstimateCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForEstimateLookupTableInput input);


        Task<EstimateWorkOrderFkListDto> GetWorkOrderFkData(int workOrderId);

        Task<EstimateQuotationFkListDto> GetQuotationFkData(int quotationId);

        Task<GetEstimateQuotationForViewDto> GetEstimateQuotationForView(int estimateId, PagedAndSortedResultRequestDto input);

        Task<GetEstimateWorkOrderForViewDto> GetEstimateWorkOrderForView(int estimateId, PagedAndSortedResultRequestDto input);


        Task<EstimatePdfDto> GetEstimatePDFInfo(int estimateId);
    }
}