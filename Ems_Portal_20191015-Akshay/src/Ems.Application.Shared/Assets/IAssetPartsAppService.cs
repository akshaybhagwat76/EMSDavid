using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;


namespace Ems.Assets
{
    public interface IAssetPartsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetPartForViewDto>> GetAll(GetAllAssetPartsInput input);

        Task<GetAssetPartForViewDto> GetAssetPartForView(int id);

		Task<GetAssetPartForEditOutput> GetAssetPartForEdit(EntityDto input);

		Task<CreateOrEditAssetPartDto> CreateOrEdit(CreateOrEditAssetPartDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetPartsToExcel(GetAllAssetPartsForExcelInput input);

		
		Task<PagedResultDto<AssetPartAssetPartTypeLookupTableDto>> GetAllAssetPartTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartAssetPartLookupTableDto>> GetAllAssetPartForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartAssetPartStatusLookupTableDto>> GetAllAssetPartStatusForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartAttachmentLookupTableDto>> GetAllAttachmentForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetPartItemTypeLookupTableDto>> GetAllItemTypeForLookupTable(GetAllForLookupTableInput input);
		
    }
}