using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IQuotationsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetQuotationForViewDto>> GetAll(GetAllQuotationsInput input);

        Task<GetQuotationForViewDto> GetQuotationForView(int id);

		Task<GetQuotationForEditOutput> GetQuotationForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditQuotationDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetQuotationsToExcel(GetAllQuotationsForExcelInput input);

		
		Task<PagedResultDto<QuotationSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<QuotationQuotationStatusLookupTableDto>> GetAllQuotationStatusForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<QuotationWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<QuotationAssetLookupTableDto>> GetAllAssetForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        Task<PagedResultDto<QuotationAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        Task<PagedResultDto<QuotationSupportTypeLookupTableDto>> GetAllSupportTypeForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        Task<PagedResultDto<QuotationSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        Task<QuotationAssetAndSupportItemListDto> GetQuotationAssetAndSupportItemList(int workOrderId, int assetId);
    }
}