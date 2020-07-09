using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Organizations.Dtos;
using Ems.Dto;

namespace Ems.Organizations
{
    public interface IAddressesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAddressForViewDto>> GetAll(GetAllAddressesInput input);

        Task<GetAddressForViewDto> GetAddressForView(int id);

		Task<GetAddressForEditOutput> GetAddressForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAddressDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<AddressCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AddressAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AddressVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input);
		
    }
}