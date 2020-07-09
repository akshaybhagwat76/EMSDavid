using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Support.Exporting;
using Ems.Support.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderActions)]
    public class WorkOrderActionsAppService : EmsAppServiceBase, IWorkOrderActionsAppService
    {
        private readonly IRepository<WorkOrderAction> _workOrderActionRepository;
        private readonly IWorkOrderActionsExcelExporter _workOrderActionsExcelExporter;


        public WorkOrderActionsAppService(IRepository<WorkOrderAction> workOrderActionRepository, IWorkOrderActionsExcelExporter workOrderActionsExcelExporter)
        {
            _workOrderActionRepository = workOrderActionRepository;
            _workOrderActionsExcelExporter = workOrderActionsExcelExporter;
        }

        public async Task<PagedResultDto<GetWorkOrderActionForViewDto>> GetAll(GetAllWorkOrderActionsInput input)
        {

            var filteredWorkOrderActions = _workOrderActionRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Action.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ActionFilter), e => e.Action.ToLower() == input.ActionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

            var pagedAndFilteredWorkOrderActions = filteredWorkOrderActions
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var workOrderActions = from o in pagedAndFilteredWorkOrderActions
                                   select new GetWorkOrderActionForViewDto()
                                   {
                                       WorkOrderAction = new WorkOrderActionDto
                                       {
                                           Action = o.Action,
                                           Description = o.Description,
                                           Id = o.Id
                                       }
                                   };

            var totalCount = await filteredWorkOrderActions.CountAsync();

            return new PagedResultDto<GetWorkOrderActionForViewDto>(
                totalCount,
                await workOrderActions.ToListAsync()
            );
        }

        public async Task<GetWorkOrderActionForViewDto> GetWorkOrderActionForView(int id)
        {
            var workOrderAction = await _workOrderActionRepository.GetAsync(id);

            var output = new GetWorkOrderActionForViewDto { WorkOrderAction = ObjectMapper.Map<WorkOrderActionDto>(workOrderAction) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderActions_Edit)]
        public async Task<GetWorkOrderActionForEditOutput> GetWorkOrderActionForEdit(EntityDto input)
        {
            var workOrderAction = await _workOrderActionRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetWorkOrderActionForEditOutput { WorkOrderAction = ObjectMapper.Map<CreateOrEditWorkOrderActionDto>(workOrderAction) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditWorkOrderActionDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderActions_Create)]
        protected virtual async Task Create(CreateOrEditWorkOrderActionDto input)
        {
            var workOrderAction = ObjectMapper.Map<WorkOrderAction>(input);

            if (AbpSession.TenantId != null)
                workOrderAction.TenantId = (int?)AbpSession.TenantId;

            await _workOrderActionRepository.InsertAsync(workOrderAction);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderActions_Edit)]
        protected virtual async Task Update(CreateOrEditWorkOrderActionDto input)
        {
            var workOrderAction = await _workOrderActionRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, workOrderAction);
        }

        [AbpAuthorize(AppPermissions.Pages_Configuration_WorkOrderActions_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _workOrderActionRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetWorkOrderActionsToExcel(GetAllWorkOrderActionsForExcelInput input)
        {

            var filteredWorkOrderActions = _workOrderActionRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Action.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ActionFilter), e => e.Action.ToLower() == input.ActionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim());

            var query = (from o in filteredWorkOrderActions
                         select new GetWorkOrderActionForViewDto()
                         {
                             WorkOrderAction = new WorkOrderActionDto
                             {
                                 Action = o.Action,
                                 Description = o.Description,
                                 Id = o.Id
                             }
                         });


            var workOrderActionListDtos = await query.ToListAsync();

            return _workOrderActionsExcelExporter.ExportToFile(workOrderActionListDtos);
        }
    }
}