using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Organizations.Dtos;
using Ems.Dto;

namespace Ems.Organizations
{
    public interface IContactsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetContactForViewDto>> GetAll(GetAllContactsInput input);

        Task<GetContactForViewDto> GetContactForView(int id);

		Task<GetContactForEditOutput> GetContactForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditContactDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetContactsToExcel(GetAllContactsForExcelInput input);

		
		Task<PagedResultDto<ContactUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<ContactVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<ContactAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<ContactCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input);
		
    }
}