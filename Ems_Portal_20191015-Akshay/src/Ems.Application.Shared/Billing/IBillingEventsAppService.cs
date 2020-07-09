using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface IBillingEventsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetBillingEventForViewDto>> GetAll(GetAllBillingEventsInput input);

        Task<GetBillingEventForViewDto> GetBillingEventForView(int id);

		Task<GetBillingEventForEditOutput> GetBillingEventForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditBillingEventDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetBillingEventsToExcel(GetAllBillingEventsForExcelInput input);

		
		Task<PagedResultDto<BillingEventLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingEventVendorChargeLookupTableDto>> GetAllVendorChargeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<BillingEventBillingEventTypeLookupTableDto>> GetAllBillingEventTypeForLookupTable(GetAllForLookupTableInput input);
		
    }
}