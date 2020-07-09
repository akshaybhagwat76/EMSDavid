using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface IAssetStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetStatusForViewDto>> GetAll(GetAllAssetStatusesInput input);

        Task<GetAssetStatusForViewDto> GetAssetStatusForView(int id);

		Task<GetAssetStatusForEditOutput> GetAssetStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetStatusDto input);

		Task Delete(EntityDto input);

		
    }
}