using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Customers.Dtos;
using Ems.Dto;

namespace Ems.Customers
{
    public interface ICustomersAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerForViewDto>> GetAll(GetAllCustomersInput input);

        Task<GetCustomerForViewDto> GetCustomerForView(int? id);

		Task<GetCustomerForEditOutput> GetCustomerForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetCustomersToExcel(GetAllCustomersForExcelInput input);

		
		Task<PagedResultDto<CustomerCustomerTypeLookupTableDto>> GetAllCustomerTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<CustomerCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
		
    }
}