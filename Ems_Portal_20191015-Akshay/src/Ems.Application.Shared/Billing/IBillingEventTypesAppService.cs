using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IBillingEventTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBillingEventTypeForViewDto>> GetAll(GetAllBillingEventTypesInput input);

        Task<GetBillingEventTypeForViewDto> GetBillingEventTypeForView(int id);

		Task<GetBillingEventTypeForEditOutput> GetBillingEventTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBillingEventTypeDto input);

		Task Delete(EntityDto input);

		
    }
}