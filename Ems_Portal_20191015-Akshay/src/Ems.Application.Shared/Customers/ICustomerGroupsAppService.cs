using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Customers.Dtos;
using Ems.Dto;

namespace Ems.Customers
{
    public interface ICustomerGroupsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerGroupForViewDto>> GetAll(GetAllCustomerGroupsInput input);

        Task<GetCustomerGroupForViewDto> GetCustomerGroupForView(int id);

		Task<GetCustomerGroupForEditOutput> GetCustomerGroupForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerGroupDto input);

		Task Delete(EntityDto input);

		
    }
}