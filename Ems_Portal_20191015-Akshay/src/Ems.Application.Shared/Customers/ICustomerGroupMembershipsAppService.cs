using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Customers.Dtos;
using Ems.Dto;

namespace Ems.Customers
{
    public interface ICustomerGroupMembershipsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerGroupMembershipForViewDto>> GetAll(GetAllCustomerGroupMembershipsInput input);

        Task<GetCustomerGroupMembershipForViewDto> GetCustomerGroupMembershipForView(int id);

		Task<GetCustomerGroupMembershipForEditOutput> GetCustomerGroupMembershipForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerGroupMembershipDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetCustomerGroupMembershipsToExcel(GetAllCustomerGroupMembershipsForExcelInput input);

		
		Task<PagedResultDto<CustomerGroupMembershipCustomerGroupLookupTableDto>> GetAllCustomerGroupForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<CustomerGroupMembershipCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllForLookupTableInput input);
		
    }
}