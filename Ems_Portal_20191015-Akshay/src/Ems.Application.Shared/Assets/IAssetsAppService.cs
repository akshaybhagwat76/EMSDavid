using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetForViewDto>> GetAll(GetAllAssetsInput input);

        Task<GetAssetForViewDto> GetAssetForView(int id);

		Task<GetAssetForEditOutput> GetAssetForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetsToExcel(GetAllAssetsForExcelInput input);

		
		Task<PagedResultDto<AssetAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetAssetStatusLookupTableDto>> GetAllAssetStatusForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<Organizations.Dtos.LocationLookupTableDto>> GetAllLocationForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<GetLeaseItemForViewDto>> GetAllLeaseItems(int assetId, PagedAndSortedResultRequestDto input);

        Task<PagedResultDto<Support.Dtos.GetSupportItemForViewDto>> GetAllSupportItems(int assetId, PagedAndSortedResultRequestDto input);

        Task<PagedResultDto<Support.Dtos.GetWorkOrderForViewDto>> GetAllWorkOrders(int assetId, PagedAndSortedResultRequestDto input);
    }
}