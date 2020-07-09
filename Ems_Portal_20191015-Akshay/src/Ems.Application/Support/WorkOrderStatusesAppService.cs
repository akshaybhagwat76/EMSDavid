

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderStatuses)]
    public class WorkOrderStatusesAppService : EmsAppServiceBase, IWorkOrderStatusesAppService
    {
		 private readonly IRepository<WorkOrderStatus> _workOrderStatusRepository;
		 private readonly IWorkOrderStatusesExcelExporter _workOrderStatusesExcelExporter;
		 

		  public WorkOrderStatusesAppService(IRepository<WorkOrderStatus> workOrderStatusRepository, IWorkOrderStatusesExcelExporter workOrderStatusesExcelExporter ) 
		  {
			_workOrderStatusRepository = workOrderStatusRepository;
			_workOrderStatusesExcelExporter = workOrderStatusesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetWorkOrderStatusForViewDto>> GetAll(GetAllWorkOrderStatusesInput input)
         {
			
			var filteredWorkOrderStatuses = _workOrderStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var pagedAndFilteredWorkOrderStatuses = filteredWorkOrderStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var workOrderStatuses = from o in pagedAndFilteredWorkOrderStatuses
                         select new GetWorkOrderStatusForViewDto() {
							WorkOrderStatus = new WorkOrderStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredWorkOrderStatuses.CountAsync();

            return new PagedResultDto<GetWorkOrderStatusForViewDto>(
                totalCount,
                await workOrderStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetWorkOrderStatusForViewDto> GetWorkOrderStatusForView(int id)
         {
            var workOrderStatus = await _workOrderStatusRepository.GetAsync(id);

            var output = new GetWorkOrderStatusForViewDto { WorkOrderStatus = ObjectMapper.Map<WorkOrderStatusDto>(workOrderStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderStatuses_Edit)]
		 public async Task<GetWorkOrderStatusForEditOutput> GetWorkOrderStatusForEdit(EntityDto input)
         {
            var workOrderStatus = await _workOrderStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetWorkOrderStatusForEditOutput {WorkOrderStatus = ObjectMapper.Map<CreateOrEditWorkOrderStatusDto>(workOrderStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditWorkOrderStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditWorkOrderStatusDto input)
         {
            var workOrderStatus = ObjectMapper.Map<WorkOrderStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				workOrderStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _workOrderStatusRepository.InsertAsync(workOrderStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditWorkOrderStatusDto input)
         {
            var workOrderStatus = await _workOrderStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, workOrderStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _workOrderStatusRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetWorkOrderStatusesToExcel(GetAllWorkOrderStatusesForExcelInput input)
         {
			
			var filteredWorkOrderStatuses = _workOrderStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.StatusFilter),  e => e.Status.ToLower() == input.StatusFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

			var query = (from o in filteredWorkOrderStatuses
                         select new GetWorkOrderStatusForViewDto() { 
							WorkOrderStatus = new WorkOrderStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var workOrderStatusListDtos = await query.ToListAsync();

            return _workOrderStatusesExcelExporter.ExportToFile(workOrderStatusListDtos);
         }


    }
}