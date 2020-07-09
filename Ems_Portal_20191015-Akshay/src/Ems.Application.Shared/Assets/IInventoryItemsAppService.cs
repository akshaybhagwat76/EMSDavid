using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;


namespace Ems.Assets
{
    public interface IInventoryItemsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetInventoryItemForViewDto>> GetAll(GetAllInventoryItemsInput input);

        Task<GetInventoryItemForViewDto> GetInventoryItemForView(int id);

		Task<GetInventoryItemForEditOutput> GetInventoryItemForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditInventoryItemDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetInventoryItemsToExcel(GetAllInventoryItemsForExcelInput input);

		
		Task<PagedResultDto<InventoryItemItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<InventoryItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
    }
}