using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetTypeForViewDto>> GetAll(GetAllAssetTypesInput input);

        Task<GetAssetTypeForViewDto> GetAssetTypeForView(int id);

		Task<GetAssetTypeForEditOutput> GetAssetTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetTypeDto input);

		Task Delete(EntityDto input);

		
    }
}