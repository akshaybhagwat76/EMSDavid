using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Vendors.Dtos;
using Ems.Dto;

namespace Ems.Vendors
{
    public interface IVendorChargesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVendorChargeForViewDto>> GetAll(GetAllVendorChargesInput input);

        Task<GetVendorChargeForViewDto> GetVendorChargeForView(int id);

		Task<GetVendorChargeForEditOutput> GetVendorChargeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditVendorChargeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetVendorChargesToExcel(GetAllVendorChargesForExcelInput input);

		
		Task<PagedResultDto<VendorChargeVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<VendorChargeSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<VendorChargeWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<VendorChargeVendorChargeStatusLookupTableDto>> GetAllVendorChargeStatusForLookupTable(GetAllForLookupTableInput input);
		
    }
}