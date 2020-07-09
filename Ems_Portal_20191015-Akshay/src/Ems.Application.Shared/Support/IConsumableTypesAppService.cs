using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IConsumableTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetConsumableTypeForViewDto>> GetAll(GetAllConsumableTypesInput input);

        Task<GetConsumableTypeForViewDto> GetConsumableTypeForView(int id);

		Task<GetConsumableTypeForEditOutput> GetConsumableTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditConsumableTypeDto input);

		Task Delete(EntityDto input);

		
    }
}