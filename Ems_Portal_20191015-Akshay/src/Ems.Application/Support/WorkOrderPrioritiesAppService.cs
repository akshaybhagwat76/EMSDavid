

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Exporting;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderPriorities)]
    public class WorkOrderPrioritiesAppService : EmsAppServiceBase, IWorkOrderPrioritiesAppService
    {
		 private readonly IRepository<WorkOrderPriority> _workOrderPriorityRepository;
		 private readonly IWorkOrderPrioritiesExcelExporter _workOrderPrioritiesExcelExporter;
		 

		  public WorkOrderPrioritiesAppService(IRepository<WorkOrderPriority> workOrderPriorityRepository, IWorkOrderPrioritiesExcelExporter workOrderPrioritiesExcelExporter ) 
		  {
			_workOrderPriorityRepository = workOrderPriorityRepository;
			_workOrderPrioritiesExcelExporter = workOrderPrioritiesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetWorkOrderPriorityForViewDto>> GetAll(GetAllWorkOrderPrioritiesInput input)
         {
			
			var filteredWorkOrderPriorities = _workOrderPriorityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Priority.Contains(input.Filter));

			var pagedAndFilteredWorkOrderPriorities = filteredWorkOrderPriorities
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var workOrderPriorities = from o in pagedAndFilteredWorkOrderPriorities
                         select new GetWorkOrderPriorityForViewDto() {
							WorkOrderPriority = new WorkOrderPriorityDto
							{
                                Priority = o.Priority,
                                PriorityLevel = o.PriorityLevel,
                                Id = o.Id
							}
						};

            var totalCount = await filteredWorkOrderPriorities.CountAsync();

            return new PagedResultDto<GetWorkOrderPriorityForViewDto>(
                totalCount,
                await workOrderPriorities.ToListAsync()
            );
         }
		 
		 public async Task<GetWorkOrderPriorityForViewDto> GetWorkOrderPriorityForView(int id)
         {
            var workOrderPriority = await _workOrderPriorityRepository.GetAsync(id);

            var output = new GetWorkOrderPriorityForViewDto { WorkOrderPriority = ObjectMapper.Map<WorkOrderPriorityDto>(workOrderPriority) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderPriorities_Edit)]
		 public async Task<GetWorkOrderPriorityForEditOutput> GetWorkOrderPriorityForEdit(EntityDto input)
         {
            var workOrderPriority = await _workOrderPriorityRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetWorkOrderPriorityForEditOutput {WorkOrderPriority = ObjectMapper.Map<CreateOrEditWorkOrderPriorityDto>(workOrderPriority)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditWorkOrderPriorityDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderPriorities_Create)]
		 protected virtual async Task Create(CreateOrEditWorkOrderPriorityDto input)
         {
            var workOrderPriority = ObjectMapper.Map<WorkOrderPriority>(input);

			
			if (AbpSession.TenantId != null)
			{
				workOrderPriority.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _workOrderPriorityRepository.InsertAsync(workOrderPriority);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderPriorities_Edit)]
		 protected virtual async Task Update(CreateOrEditWorkOrderPriorityDto input)
         {
            var workOrderPriority = await _workOrderPriorityRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, workOrderPriority);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderPriorities_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _workOrderPriorityRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetWorkOrderPrioritiesToExcel(GetAllWorkOrderPrioritiesForExcelInput input)
         {
			
			var filteredWorkOrderPriorities = _workOrderPriorityRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Priority.Contains(input.Filter));

			var query = (from o in filteredWorkOrderPriorities
                         select new GetWorkOrderPriorityForViewDto() { 
							WorkOrderPriority = new WorkOrderPriorityDto
							{
                                Priority = o.Priority,
                                PriorityLevel = o.PriorityLevel,
                                Id = o.Id
							}
						 });


            var workOrderPriorityListDtos = await query.ToListAsync();

            return _workOrderPrioritiesExcelExporter.ExportToFile(workOrderPriorityListDtos);
         }


    }
}