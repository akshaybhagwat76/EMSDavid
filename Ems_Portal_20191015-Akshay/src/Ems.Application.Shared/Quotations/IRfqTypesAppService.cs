using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IRfqTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetRfqTypeForViewDto>> GetAll(GetAllRfqTypesInput input);

        Task<GetRfqTypeForViewDto> GetRfqTypeForView(int id);

		Task<GetRfqTypeForEditOutput> GetRfqTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditRfqTypeDto input);

		Task Delete(EntityDto input);

		
    }
}