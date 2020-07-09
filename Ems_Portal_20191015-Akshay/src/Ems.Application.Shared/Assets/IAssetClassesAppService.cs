using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetClassesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetClassForViewDto>> GetAll(GetAllAssetClassesInput input);

        Task<GetAssetClassForViewDto> GetAssetClassForView(int id);

		Task<GetAssetClassForEditOutput> GetAssetClassForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetClassDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetClassesToExcel(GetAllAssetClassesForExcelInput input);

		
		Task<PagedResultDto<AssetClassAssetTypeLookupTableDto>> GetAllAssetTypeForLookupTable(GetAllForLookupTableInput input);
		
    }
}