

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_ConsumableTypes)]
    public class ConsumableTypesAppService : EmsAppServiceBase, IConsumableTypesAppService
    {
		 private readonly IRepository<ConsumableType> _consumableTypeRepository;
		 

		  public ConsumableTypesAppService(IRepository<ConsumableType> consumableTypeRepository ) 
		  {
			_consumableTypeRepository = consumableTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetConsumableTypeForViewDto>> GetAll(GetAllConsumableTypesInput input)
         {
			
			var filteredConsumableTypes = _consumableTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredConsumableTypes = filteredConsumableTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var consumableTypes = from o in pagedAndFilteredConsumableTypes
                         select new GetConsumableTypeForViewDto() {
							ConsumableType = new ConsumableTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredConsumableTypes.CountAsync();

            return new PagedResultDto<GetConsumableTypeForViewDto>(
                totalCount,
                await consumableTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetConsumableTypeForViewDto> GetConsumableTypeForView(int id)
         {
            var consumableType = await _consumableTypeRepository.GetAsync(id);

            var output = new GetConsumableTypeForViewDto { ConsumableType = ObjectMapper.Map<ConsumableTypeDto>(consumableType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_ConsumableTypes_Edit)]
		 public async Task<GetConsumableTypeForEditOutput> GetConsumableTypeForEdit(EntityDto input)
         {
            var consumableType = await _consumableTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetConsumableTypeForEditOutput {ConsumableType = ObjectMapper.Map<CreateOrEditConsumableTypeDto>(consumableType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditConsumableTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_ConsumableTypes_Create)]
		 protected virtual async Task Create(CreateOrEditConsumableTypeDto input)
         {
            var consumableType = ObjectMapper.Map<ConsumableType>(input);

			
			if (AbpSession.TenantId != null)
			{
				consumableType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _consumableTypeRepository.InsertAsync(consumableType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_ConsumableTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditConsumableTypeDto input)
         {
            var consumableType = await _consumableTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, consumableType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_ConsumableTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _consumableTypeRepository.DeleteAsync(input.Id);
         } 
    }
}