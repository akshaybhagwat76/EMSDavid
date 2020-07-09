using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Quotations.Dtos;
using Ems.Dto;

namespace Ems.Quotations
{
    public interface IRfqsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetRfqForViewDto>> GetAll(GetAllRfqsInput input);

        Task<GetRfqForViewDto> GetRfqForView(int id);

		Task<GetRfqForEditOutput> GetRfqForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditRfqDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetRfqsToExcel(GetAllRfqsForExcelInput input);

		
		Task<PagedResultDto<RfqRfqTypeLookupTableDto>> GetAllRfqTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<RfqUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}