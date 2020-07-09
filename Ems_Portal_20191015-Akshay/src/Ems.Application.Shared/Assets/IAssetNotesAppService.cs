using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using System.Threading.Tasks;

namespace Ems.Assets
{
    public interface IAssetNotesAppService : IApplicationService
    {
        Task<PagedResultDto<GetAssetNotesForViewDto>> GetAll(int assetId, PagedAndSortedResultRequestDto input);

        Task<GetAssetNotesForViewDto> GetAssetNotesForView(int id);

        Task<GetAssetNotesForEditOutput> GetAssetNotesForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditAssetNotesDto input);

        Task Delete(EntityDto input);
    }
}
