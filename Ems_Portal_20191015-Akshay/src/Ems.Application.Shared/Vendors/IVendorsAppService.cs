using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Vendors.Dtos;
using Ems.Dto;

namespace Ems.Vendors
{
    public interface IVendorsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetVendorForViewDto>> GetAll(GetAllVendorsInput input);

        Task<GetVendorForViewDto> GetVendorForView(int? id);

		Task<GetVendorForEditOutput> GetVendorForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditVendorDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetVendorsToExcel(GetAllVendorsForExcelInput input);

		
		Task<PagedResultDto<VendorSsicCodeLookupTableDto>> GetAllSsicCodeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<VendorCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
		
    }
}