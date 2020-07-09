

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderTypes)]
    public class WorkOrderTypesAppService : EmsAppServiceBase, IWorkOrderTypesAppService
    {
		 private readonly IRepository<WorkOrderType> _workOrderTypeRepository;
		 

		  public WorkOrderTypesAppService(IRepository<WorkOrderType> workOrderTypeRepository ) 
		  {
			_workOrderTypeRepository = workOrderTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetWorkOrderTypeForViewDto>> GetAll(GetAllWorkOrderTypesInput input)
         {
			
			var filteredWorkOrderTypes = _workOrderTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter),  e => e.Type.ToLower() == input.TypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var pagedAndFilteredWorkOrderTypes = filteredWorkOrderTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var workOrderTypes = from o in pagedAndFilteredWorkOrderTypes
                         select new GetWorkOrderTypeForViewDto() {
							WorkOrderType = new WorkOrderTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredWorkOrderTypes.CountAsync();

            return new PagedResultDto<GetWorkOrderTypeForViewDto>(
                totalCount,
                await workOrderTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetWorkOrderTypeForViewDto> GetWorkOrderTypeForView(int id)
         {
            var workOrderType = await _workOrderTypeRepository.GetAsync(id);

            var output = new GetWorkOrderTypeForViewDto { WorkOrderType = ObjectMapper.Map<WorkOrderTypeDto>(workOrderType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderTypes_Edit)]
		 public async Task<GetWorkOrderTypeForEditOutput> GetWorkOrderTypeForEdit(EntityDto input)
         {
            var workOrderType = await _workOrderTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetWorkOrderTypeForEditOutput {WorkOrderType = ObjectMapper.Map<CreateOrEditWorkOrderTypeDto>(workOrderType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditWorkOrderTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderTypes_Create)]
		 protected virtual async Task Create(CreateOrEditWorkOrderTypeDto input)
         {
            var workOrderType = ObjectMapper.Map<WorkOrderType>(input);

			
			if (AbpSession.TenantId != null)
			{
				workOrderType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _workOrderTypeRepository.InsertAsync(workOrderType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditWorkOrderTypeDto input)
         {
            var workOrderType = await _workOrderTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, workOrderType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _workOrderTypeRepository.DeleteAsync(input.Id);
         } 
    }
}