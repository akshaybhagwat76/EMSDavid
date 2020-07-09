using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IBillingRulesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBillingRuleForViewDto>> GetAll(GetAllBillingRulesInput input);

        Task<GetBillingRuleForViewDto> GetBillingRuleForView(int id);

		Task<GetBillingRuleForEditOutput> GetBillingRuleForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBillingRuleDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetBillingRulesToExcel(GetAllBillingRulesForExcelInput input);

		
		Task<PagedResultDto<BillingRuleBillingRuleTypeLookupTableDto>> GetAllBillingRuleTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingRuleUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingRuleLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingRuleVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingRuleLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingRuleCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
		
    }
}