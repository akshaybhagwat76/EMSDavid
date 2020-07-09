

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_BillingEventTypes)]
    public class BillingEventTypesAppService : EmsAppServiceBase, IBillingEventTypesAppService
    {
		 private readonly IRepository<BillingEventType> _billingEventTypeRepository;
		 

		  public BillingEventTypesAppService(IRepository<BillingEventType> billingEventTypeRepository ) 
		  {
			_billingEventTypeRepository = billingEventTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetBillingEventTypeForViewDto>> GetAll(GetAllBillingEventTypesInput input)
         {
			
			var filteredBillingEventTypes = _billingEventTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredBillingEventTypes = filteredBillingEventTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var billingEventTypes = from o in pagedAndFilteredBillingEventTypes
                         select new GetBillingEventTypeForViewDto() {
							BillingEventType = new BillingEventTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredBillingEventTypes.CountAsync();

            return new PagedResultDto<GetBillingEventTypeForViewDto>(
                totalCount,
                await billingEventTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetBillingEventTypeForViewDto> GetBillingEventTypeForView(int id)
         {
            var billingEventType = await _billingEventTypeRepository.GetAsync(id);

            var output = new GetBillingEventTypeForViewDto { BillingEventType = ObjectMapper.Map<BillingEventTypeDto>(billingEventType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingEventTypes_Edit)]
		 public async Task<GetBillingEventTypeForEditOutput> GetBillingEventTypeForEdit(EntityDto input)
         {
            var billingEventType = await _billingEventTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBillingEventTypeForEditOutput {BillingEventType = ObjectMapper.Map<CreateOrEditBillingEventTypeDto>(billingEventType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBillingEventTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingEventTypes_Create)]
		 protected virtual async Task Create(CreateOrEditBillingEventTypeDto input)
         {
            var billingEventType = ObjectMapper.Map<BillingEventType>(input);

			
			if (AbpSession.TenantId != null)
			{
				billingEventType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _billingEventTypeRepository.InsertAsync(billingEventType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingEventTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditBillingEventTypeDto input)
         {
            var billingEventType = await _billingEventTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, billingEventType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingEventTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _billingEventTypeRepository.DeleteAsync(input.Id);
         } 
    }
}