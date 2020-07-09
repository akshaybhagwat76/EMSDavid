using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface ISupportTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSupportTypeForViewDto>> GetAll(GetAllSupportTypesInput input);

        Task<GetSupportTypeForViewDto> GetSupportTypeForView(int id);

		Task<GetSupportTypeForEditOutput> GetSupportTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSupportTypeDto input);

		Task Delete(EntityDto input);

		
    }
}