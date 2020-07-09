using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IBillingEventDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBillingEventDetailForViewDto>> GetAll(GetAllBillingEventDetailsInput input);

        Task<GetBillingEventDetailForViewDto> GetBillingEventDetailForView(int id);

		Task<GetBillingEventDetailForEditOutput> GetBillingEventDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBillingEventDetailDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetBillingEventDetailsToExcel(GetAllBillingEventDetailsForExcelInput input);

		
		Task<PagedResultDto<BillingEventDetailBillingRuleLookupTableDto>> GetAllBillingRuleForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingEventDetailLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingEventDetailBillingEventLookupTableDto>> GetAllBillingEventForLookupTable(GetAllForLookupTableInput input);
		
    }
}