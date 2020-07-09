

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
	[AbpAuthorize(AppPermissions.Pages_Main_CustomerGroups)]
    public class CustomerGroupsAppService : EmsAppServiceBase, ICustomerGroupsAppService
    {
		 private readonly IRepository<CustomerGroup> _customerGroupRepository;
		 

		  public CustomerGroupsAppService(IRepository<CustomerGroup> customerGroupRepository ) 
		  {
			_customerGroupRepository = customerGroupRepository;
			
		  }

		 public async Task<PagedResultDto<GetCustomerGroupForViewDto>> GetAll(GetAllCustomerGroupsInput input)
         {
			
			var filteredCustomerGroups = _customerGroupRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredCustomerGroups = filteredCustomerGroups
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customerGroups = from o in pagedAndFilteredCustomerGroups
                         select new GetCustomerGroupForViewDto() {
							CustomerGroup = new CustomerGroupDto
							{
                                Name = o.Name,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCustomerGroups.CountAsync();

            return new PagedResultDto<GetCustomerGroupForViewDto>(
                totalCount,
                await customerGroups.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerGroupForViewDto> GetCustomerGroupForView(int id)
         {
            var customerGroup = await _customerGroupRepository.GetAsync(id);

            var output = new GetCustomerGroupForViewDto { CustomerGroup = ObjectMapper.Map<CustomerGroupDto>(customerGroup) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroups_Edit)]
		 public async Task<GetCustomerGroupForEditOutput> GetCustomerGroupForEdit(EntityDto input)
         {
            var customerGroup = await _customerGroupRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerGroupForEditOutput {CustomerGroup = ObjectMapper.Map<CreateOrEditCustomerGroupDto>(customerGroup)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerGroupDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroups_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerGroupDto input)
         {
            var customerGroup = ObjectMapper.Map<CustomerGroup>(input);

			
			if (AbpSession.TenantId != null)
			{
				customerGroup.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerGroupRepository.InsertAsync(customerGroup);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroups_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerGroupDto input)
         {
            var customerGroup = await _customerGroupRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, customerGroup);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_CustomerGroups_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _customerGroupRepository.DeleteAsync(input.Id);
         } 
    }
}