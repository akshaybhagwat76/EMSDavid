using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetOwnersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetOwnerForViewDto>> GetAll(GetAllAssetOwnersInput input);

        Task<GetAssetOwnerForViewDto> GetAssetOwnerForView(int? id);

		Task<GetAssetOwnerForEditOutput> GetAssetOwnerForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetOwnerDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetOwnersToExcel(GetAllAssetOwnersForExcelInput input);

		
		Task<PagedResultDto<AssetOwnerCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AssetOwnerSsicCodeLookupTableDto>> GetAllSsicCodeForLookupTable(GetAllForLookupTableInput input);
		
    }
}