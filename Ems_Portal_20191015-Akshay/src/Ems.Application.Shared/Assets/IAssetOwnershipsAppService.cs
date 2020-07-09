using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetOwnershipsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetOwnershipForViewDto>> GetAll(GetAllAssetOwnershipsInput input);

        Task<GetAssetOwnershipForViewDto> GetAssetOwnershipForView(int id);

		Task<GetAssetOwnershipForEditOutput> GetAssetOwnershipForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetOwnershipDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetOwnershipsToExcel(GetAllAssetOwnershipsForExcelInput input);

		
		Task<PagedResultDto<AssetOwnershipAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetOwnershipAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input);
		
    }
}