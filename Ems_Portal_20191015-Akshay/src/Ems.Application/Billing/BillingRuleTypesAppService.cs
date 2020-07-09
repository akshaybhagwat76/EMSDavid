

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_BillingRuleTypes)]
    public class BillingRuleTypesAppService : EmsAppServiceBase, IBillingRuleTypesAppService
    {
		 private readonly IRepository<BillingRuleType> _billingRuleTypeRepository;
		 

		  public BillingRuleTypesAppService(IRepository<BillingRuleType> billingRuleTypeRepository ) 
		  {
			_billingRuleTypeRepository = billingRuleTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetBillingRuleTypeForViewDto>> GetAll(GetAllBillingRuleTypesInput input)
         {
			
			var filteredBillingRuleTypes = _billingRuleTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredBillingRuleTypes = filteredBillingRuleTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var billingRuleTypes = from o in pagedAndFilteredBillingRuleTypes
                         select new GetBillingRuleTypeForViewDto() {
							BillingRuleType = new BillingRuleTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredBillingRuleTypes.CountAsync();

            return new PagedResultDto<GetBillingRuleTypeForViewDto>(
                totalCount,
                await billingRuleTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetBillingRuleTypeForViewDto> GetBillingRuleTypeForView(int id)
         {
            var billingRuleType = await _billingRuleTypeRepository.GetAsync(id);

            var output = new GetBillingRuleTypeForViewDto { BillingRuleType = ObjectMapper.Map<BillingRuleTypeDto>(billingRuleType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingRuleTypes_Edit)]
		 public async Task<GetBillingRuleTypeForEditOutput> GetBillingRuleTypeForEdit(EntityDto input)
         {
            var billingRuleType = await _billingRuleTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBillingRuleTypeForEditOutput {BillingRuleType = ObjectMapper.Map<CreateOrEditBillingRuleTypeDto>(billingRuleType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBillingRuleTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingRuleTypes_Create)]
		 protected virtual async Task Create(CreateOrEditBillingRuleTypeDto input)
         {
            var billingRuleType = ObjectMapper.Map<BillingRuleType>(input);

			
			if (AbpSession.TenantId != null)
			{
				billingRuleType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _billingRuleTypeRepository.InsertAsync(billingRuleType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingRuleTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditBillingRuleTypeDto input)
         {
            var billingRuleType = await _billingRuleTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, billingRuleType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_BillingRuleTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _billingRuleTypeRepository.DeleteAsync(input.Id);
         } 
    }
}