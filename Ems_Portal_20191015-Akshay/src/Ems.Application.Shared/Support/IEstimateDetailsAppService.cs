using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using System.Threading.Tasks;

namespace Ems.Support
{
    public interface IEstimateDetailsAppService : IApplicationService
    {
        Task<PagedResultDto<GetEstimateDetailForViewDto>> GetAll(GetAllEstimateDetailsInput input);

        Task<GetEstimateDetailForViewDto> GetEstimateDetailForView(int id);

        Task<GetEstimateDetailForEditOutput> GetEstimateDetailForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditEstimateDetailDto input);

        Task Delete(EntityDto input);
    }
}
