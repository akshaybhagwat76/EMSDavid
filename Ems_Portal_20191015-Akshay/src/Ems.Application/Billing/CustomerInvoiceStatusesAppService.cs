

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Billing.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Billing
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses)]
    public class CustomerInvoiceStatusesAppService : EmsAppServiceBase, ICustomerInvoiceStatusesAppService
    {
		 private readonly IRepository<CustomerInvoiceStatus> _customerInvoiceStatusRepository;
		 

		  public CustomerInvoiceStatusesAppService(IRepository<CustomerInvoiceStatus> customerInvoiceStatusRepository ) 
		  {
			_customerInvoiceStatusRepository = customerInvoiceStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetCustomerInvoiceStatusForViewDto>> GetAll(GetAllCustomerInvoiceStatusesInput input)
         {
			
			var filteredCustomerInvoiceStatuses = _customerInvoiceStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var pagedAndFilteredCustomerInvoiceStatuses = filteredCustomerInvoiceStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var customerInvoiceStatuses = from o in pagedAndFilteredCustomerInvoiceStatuses
                         select new GetCustomerInvoiceStatusForViewDto() {
							CustomerInvoiceStatus = new CustomerInvoiceStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCustomerInvoiceStatuses.CountAsync();

            return new PagedResultDto<GetCustomerInvoiceStatusForViewDto>(
                totalCount,
                await customerInvoiceStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetCustomerInvoiceStatusForViewDto> GetCustomerInvoiceStatusForView(int id)
         {
            var customerInvoiceStatus = await _customerInvoiceStatusRepository.GetAsync(id);

            var output = new GetCustomerInvoiceStatusForViewDto { CustomerInvoiceStatus = ObjectMapper.Map<CustomerInvoiceStatusDto>(customerInvoiceStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Edit)]
		 public async Task<GetCustomerInvoiceStatusForEditOutput> GetCustomerInvoiceStatusForEdit(EntityDto input)
         {
            var customerInvoiceStatus = await _customerInvoiceStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCustomerInvoiceStatusForEditOutput {CustomerInvoiceStatus = ObjectMapper.Map<CreateOrEditCustomerInvoiceStatusDto>(customerInvoiceStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCustomerInvoiceStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditCustomerInvoiceStatusDto input)
         {
            var customerInvoiceStatus = ObjectMapper.Map<CustomerInvoiceStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				customerInvoiceStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _customerInvoiceStatusRepository.InsertAsync(customerInvoiceStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditCustomerInvoiceStatusDto input)
         {
            var customerInvoiceStatus = await _customerInvoiceStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, customerInvoiceStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_CustomerInvoiceStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _customerInvoiceStatusRepository.DeleteAsync(input.Id);
         } 
    }
}