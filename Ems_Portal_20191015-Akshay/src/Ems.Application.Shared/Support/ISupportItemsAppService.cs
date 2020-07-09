using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface ISupportItemsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSupportItemForViewDto>> GetAll(GetAllSupportItemsInput input);

        Task<GetSupportItemForViewDto> GetSupportItemForView(int id);

		Task<GetSupportItemForEditOutput> GetSupportItemForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSupportItemDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetSupportItemsToExcel(GetAllSupportItemsForExcelInput input);

		
		Task<PagedResultDto<SupportItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SupportItemAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SupportItemSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SupportItemConsumableTypeLookupTableDto>> GetAllConsumableTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SupportItemSupportTypeLookupTableDto>> GetAllSupportTypeForLookupTable(GetAllForLookupTableInput input);
		
    }
}