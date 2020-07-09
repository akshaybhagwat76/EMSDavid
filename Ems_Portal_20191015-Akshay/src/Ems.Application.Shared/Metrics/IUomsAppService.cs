using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Metrics.Dtos;
using Ems.Dto;

namespace Ems.Metrics
{
    public interface IUomsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUomForViewDto>> GetAll(GetAllUomsInput input);

        Task<GetUomForViewDto> GetUomForView(int id);

		Task<GetUomForEditOutput> GetUomForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUomDto input);

		Task Delete(EntityDto input);

		
    }
}