using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IBillingRuleTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBillingRuleTypeForViewDto>> GetAll(GetAllBillingRuleTypesInput input);

        Task<GetBillingRuleTypeForViewDto> GetBillingRuleTypeForView(int id);

		Task<GetBillingRuleTypeForEditOutput> GetBillingRuleTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBillingRuleTypeDto input);

		Task Delete(EntityDto input);

		
    }
}