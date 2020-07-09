using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;


namespace Ems.Assets
{
    public interface IAssetPartStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAssetPartStatusForViewDto>> GetAll(GetAllAssetPartStatusesInput input);

        Task<GetAssetPartStatusForViewDto> GetAssetPartStatusForView(int id);

		Task<GetAssetPartStatusForEditOutput> GetAssetPartStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAssetPartStatusDto input);

		Task Delete(EntityDto input);

		
    }
}