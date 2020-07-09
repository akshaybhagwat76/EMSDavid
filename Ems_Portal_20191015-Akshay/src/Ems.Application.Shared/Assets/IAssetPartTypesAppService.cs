using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;


namespace Ems.Assets
{
    public interface IAssetPartTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetPartTypeForViewDto>> GetAll(GetAllAssetPartTypesInput input);

        Task<GetAssetPartTypeForViewDto> GetAssetPartTypeForView(int id);

		Task<GetAssetPartTypeForEditOutput> GetAssetPartTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetPartTypeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAssetPartTypesToExcel(GetAllAssetPartTypesForExcelInput input);

		
    }
}