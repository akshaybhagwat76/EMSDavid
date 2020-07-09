using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IWorkOrderTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderTypeForViewDto>> GetAll(GetAllWorkOrderTypesInput input);

        Task<GetWorkOrderTypeForViewDto> GetWorkOrderTypeForView(int id);

		Task<GetWorkOrderTypeForEditOutput> GetWorkOrderTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWorkOrderTypeDto input);

		Task Delete(EntityDto input);

		
    }
}