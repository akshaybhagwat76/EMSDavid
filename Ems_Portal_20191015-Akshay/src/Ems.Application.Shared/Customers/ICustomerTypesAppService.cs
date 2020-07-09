using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Customers.Dtos;
using Ems.Dto;

namespace Ems.Customers
{
    public interface ICustomerTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerTypeForViewDto>> GetAll(GetAllCustomerTypesInput input);

        Task<GetCustomerTypeForViewDto> GetCustomerTypeForView(int id);

		Task<GetCustomerTypeForEditOutput> GetCustomerTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerTypeDto input);

		Task Delete(EntityDto input);

		
    }
}