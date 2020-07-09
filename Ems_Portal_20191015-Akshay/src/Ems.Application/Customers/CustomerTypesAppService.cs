

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Customers.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Customers
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_CustomerTypes)]
    public class CustomerTypesAppService : EmsAppServiceBase, ICustomerTypesAppService
    {
		 private readonly IRepository<CustomerType> _customerTypeRepository;
		 

		  public CustomerTypesAppService(IRepository<CustomerType> customerTypeRepository ) 
		  {
			_customerTypeRepository = customerTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetCustomerTypeForViewDto>> GetAll(GetAllCustomerTypesInput input)
         {
			
			var filteredCustomerTypes = _customerTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredCustomerTypes = filteredCustomerTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customerTypes = from o in pagedAndFilteredCustomerTypes
                         select new GetCustomerTypeForViewDto() {
							CustomerType = new CustomerTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCustomerTypes.CountAsync();

            return new PagedResultDto<GetCustomerTypeForViewDto>(
                totalCount,
                await customerTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerTypeForViewDto> GetCustomerTypeForView(int id)
         {
            var customerType = await _customerTypeRepository.GetAsync(id);

            var output = new GetCustomerTypeForViewDto { CustomerType = ObjectMapper.Map<CustomerTypeDto>(customerType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerTypes_Edit)]
		 public async Task<GetCustomerTypeForEditOutput> GetCustomerTypeForEdit(EntityDto input)
         {
            var customerType = await _customerTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerTypeForEditOutput {CustomerType = ObjectMapper.Map<CreateOrEditCustomerTypeDto>(customerType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerTypes_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerTypeDto input)
         {
            var customerType = ObjectMapper.Map<CustomerType>(input);

			
			if (AbpSession.TenantId != null)
			{
				customerType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerTypeRepository.InsertAsync(customerType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerTypeDto input)
         {
            var customerType = await _customerTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, customerType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _customerTypeRepository.DeleteAsync(input.Id);
         } 
    }
}