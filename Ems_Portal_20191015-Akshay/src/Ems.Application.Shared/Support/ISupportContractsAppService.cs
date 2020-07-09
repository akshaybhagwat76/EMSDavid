using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface ISupportContractsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetSupportContractForViewDto>> GetAll(GetAllSupportContractsInput input);

        Task<GetSupportContractForViewDto> GetSupportContractForView(int id);

		Task<GetSupportContractForEditOutput> GetSupportContractForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditSupportContractDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetSupportContractsToExcel(GetAllSupportContractsForExcelInput input);

		
		Task<PagedResultDto<SupportContractVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<SupportContractAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input);
		
    }
}