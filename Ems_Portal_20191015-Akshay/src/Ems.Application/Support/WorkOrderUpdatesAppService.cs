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
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Quotations;
using Abp.Web.Mvc.Models;
using Ems.Storage;
using Abp.UI;
using Ems.Metrics;
using System;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders)]
    public class WorkOrderUpdatesAppService : EmsAppServiceBase, IWorkOrderUpdatesAppService
    {
        private readonly IRepository<WorkOrderUpdate> _workOrderUpdateRepository;
        private readonly IWorkOrderUpdatesExcelExporter _workOrderUpdatesExcelExporter;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;
        private readonly IRepository<Attachment, int> _lookup_attachmentRepository;

        public WorkOrderUpdatesAppService(IRepository<WorkOrderUpdate> workOrderUpdateRepository, IWorkOrderUpdatesExcelExporter workOrderUpdatesExcelExporter, IRepository<WorkOrder, int> lookup_workOrderRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<WorkOrderAction, int> lookup_workOrderActionRepository, IRepository<Attachment, int> lookup_attachmentRepository)
        {
            _workOrderUpdateRepository = workOrderUpdateRepository;
            _workOrderUpdatesExcelExporter = workOrderUpdatesExcelExporter;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
            _lookup_attachmentRepository = lookup_attachmentRepository;
        }

        public async Task<PagedResultDto<GetWorkOrderUpdateForViewDto>> GetAll(GetAllWorkOrderUpdatesInput input)
        {

            var filteredWorkOrderUpdates = _workOrderUpdateRepository.GetAll()
                        .Include(e => e.WorkOrderFk)
                        .Where(e => e.WorkOrderId == input.WorkOrderId)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.WorkOrderActionFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type.ToLower() == input.ItemTypeTypeFilter.ToLower().Trim());
            //.WhereIf(input.MinUpdatedAtFilter != null, e => e.UpdatedAt >= input.MinUpdatedAtFilter)
            //.WhereIf(input.MaxUpdatedAtFilter != null, e => e.UpdatedAt <= input.MaxUpdatedAtFilter)
            //.WhereIf(input.MinUpdatedByUserIdFilter != null, e => e.UpdatedByUserId >= input.MinUpdatedByUserIdFilter)
            //.WhereIf(input.MaxUpdatedByUserIdFilter != null, e => e.UpdatedByUserId <= input.MaxUpdatedByUserIdFilter)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim());

            var pagedAndFilteredWorkOrderUpdates = filteredWorkOrderUpdates
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var workOrderUpdates = from o in pagedAndFilteredWorkOrderUpdates
                                   join o1 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   join o6 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o6.Id into j6
                                   from s6 in j6.DefaultIfEmpty()

                                   select new GetWorkOrderUpdateForViewDto()
                                   {
                                       WorkOrderUpdate = new WorkOrderUpdateDto
                                       {
                                           Comments = o.Comments,
                                           Id = o.Id,
                                           Number = o.Number,
                                           ItemTypeId = o.ItemTypeId,
                                           WorkOrderActionId = o.WorkOrderActionId,
                                           WorkOrderId = o.WorkOrderId
                                       },
                                       WorkOrderSubject = s1 == null ? "" : s1.Subject.ToString(),
                                       ItemTypeType = s3 == null ? "" : s3.Type.ToString(),
                                       WorkOrderActionAction = s6 == null ? "" : s6.Action.ToString()
                                   };

            var totalCount = await filteredWorkOrderUpdates.CountAsync();

            return new PagedResultDto<GetWorkOrderUpdateForViewDto>(
                totalCount,
                await workOrderUpdates.ToListAsync()
            );
        }

        public async Task<List<GetWorkorderUpdateItemsForViewDto>> GetWorkorderItems(GetWorkorderUpdateItemsInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");

            var workOrders = _workOrderUpdateRepository.GetAll()
                                .Where(w => w.ItemTypeFk != null)
                                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .Include(w => w.ItemTypeFk)
                                .Where(w => !w.IsDeleted)
                                .Where(w => w.CreationTime >= DateTime.Now.AddDays(-input.Days))
                                .ToList();

            var itemsConsumed =
                    from w in workOrders
                    group w by w.ItemTypeFk into s
                    select new GetWorkorderUpdateItemsForViewDto()
                    {
                        Item = new Ems.Quotations.Dtos.ItemTypeDto
                        {
                            Type = s.Key.Type,
                            Description = s.Key.Description
                        },
                        Consumed = s.Count()
                    };

            var result = itemsConsumed.OrderByDescending(a => a.Consumed).Take(10).ToList();

            return result;
        }

        public async Task<GetWorkOrderUpdateForViewDto> GetWorkOrderUpdateForView(int id)
        {
            var workOrderUpdate = await _workOrderUpdateRepository.GetAsync(id);

            var output = new GetWorkOrderUpdateForViewDto { WorkOrderUpdate = ObjectMapper.Map<WorkOrderUpdateDto>(workOrderUpdate) };

            if (output.WorkOrderUpdate != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

            if (output.WorkOrderUpdate.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.WorkOrderUpdate.WorkOrderActionId > 0)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderActionId);
                output.WorkOrderActionAction = _lookupWorkOrderAction.Action.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_EditWorkOrderUpdate)]
        public async Task<GetWorkOrderUpdateForEditOutput> GetWorkOrderUpdateForEdit(EntityDto input)
        {
            var workOrderUpdate = await _workOrderUpdateRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetWorkOrderUpdateForEditOutput { WorkOrderUpdate = ObjectMapper.Map<CreateOrEditWorkOrderUpdateDto>(workOrderUpdate) };

            if (output.WorkOrderUpdate != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

            if (output.WorkOrderUpdate.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.WorkOrderUpdate.WorkOrderActionId > 0)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.WorkOrderUpdate.WorkOrderActionId);
                output.WorkOrderActionAction = _lookupWorkOrderAction.Action.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditWorkOrderUpdateDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_CreateWorkOrderUpdate)]
        protected virtual async Task Create(CreateOrEditWorkOrderUpdateDto input)
        {
            ErrorViewModel errorInfo = CheckWOUpdatesValidToSetComplete(input.WorkOrderId, input.Id ?? 0, input.ItemTypeId ?? 0, input.WorkOrderActionId);
            if (errorInfo == null)
            {
                var workOrderUpdate = ObjectMapper.Map<WorkOrderUpdate>(input);

                if (AbpSession.TenantId != null)
                    workOrderUpdate.TenantId = (int?)AbpSession.TenantId;

                await _workOrderUpdateRepository.InsertAsync(workOrderUpdate);
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_EditWorkOrderUpdate)]
        protected virtual async Task Update(CreateOrEditWorkOrderUpdateDto input)
        {
            ErrorViewModel errorInfo = CheckWOUpdatesValidToSetComplete(input.WorkOrderId, input.Id ?? 0, input.ItemTypeId ?? 0, input.WorkOrderActionId);
            if (errorInfo == null)
            {
                var workOrderUpdate = await _workOrderUpdateRepository.FirstOrDefaultAsync((int)input.Id);
                ObjectMapper.Map(input, workOrderUpdate);
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_DeleteWorkOrderUpdate)]
        public async Task Delete(EntityDto input)
        {
            await _workOrderUpdateRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetWorkOrderUpdatesToExcel(GetAllWorkOrderUpdatesForExcelInput input)
        {

            var filteredWorkOrderUpdates = _workOrderUpdateRepository.GetAll()
                        .Include(e => e.WorkOrderFk)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.WorkOrderActionFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Comments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type.ToLower() == input.ItemTypeTypeFilter.ToLower().Trim());
            //.WhereIf(input.MinUpdatedAtFilter != null, e => e.UpdatedAt >= input.MinUpdatedAtFilter)
            //.WhereIf(input.MaxUpdatedAtFilter != null, e => e.UpdatedAt <= input.MaxUpdatedAtFilter)
            //.WhereIf(input.MinUpdatedByUserIdFilter != null, e => e.UpdatedByUserId >= input.MinUpdatedByUserIdFilter)
            //.WhereIf(input.MaxUpdatedByUserIdFilter != null, e => e.UpdatedByUserId <= input.MaxUpdatedByUserIdFilter)
            //.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim());

            var query = (from o in filteredWorkOrderUpdates
                         join o1 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o6 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                         select new GetWorkOrderUpdateForViewDto()
                         {
                             WorkOrderUpdate = new WorkOrderUpdateDto
                             {
                                 Comments = o.Comments,
                                 Id = o.Id,
                                 Number = o.Number,
                                 ItemTypeId = o.ItemTypeId,
                                 WorkOrderActionId = o.WorkOrderActionId,
                                 WorkOrderId = o.WorkOrderId
                             },
                             WorkOrderSubject = s1 == null ? "" : s1.Subject.ToString(),
                             ItemTypeType = s3 == null ? "" : s3.Type.ToString(),
                             WorkOrderActionAction = s6 == null ? "" : s6.Action.ToString()
                         });


            var workOrderUpdateListDtos = await query.ToListAsync();

            return _workOrderUpdatesExcelExporter.ExportToFile(workOrderUpdateListDtos);
        }

        protected ErrorViewModel CheckWOUpdatesValidToSetComplete(int woId, int woUpdateId, int woItmTypeId, int woActionId)
        {
            if (woItmTypeId == 0)
                return null;

            ErrorViewModel errorInfo = new ErrorViewModel();
            errorInfo.ErrorInfo = new Abp.Web.Models.ErrorInfo();

            var woItemTypeInfo = _lookup_itemTypeRepository.FirstOrDefault(woItmTypeId);
            if (woItemTypeInfo != null)
            {
                if (woItemTypeInfo.Type.ToLower() != "gpu load test")
                    return null;

                var woActionInfo = _lookup_workOrderActionRepository.FirstOrDefault(woActionId);
                if (woActionInfo != null)
                {
                    if (woActionInfo.Action.ToLower() != "completed")
                        return null;
                    else
                    {
                        int attachmentCount = _lookup_attachmentRepository.GetAll()
                            //.WhereIf(woStartDate != null, w => w.CreationTime > woStartDate || w.LastModificationTime > woStartDate)
                            .Where(w => w.WorkOrderId == woId)
                            .Count();

                        if (attachmentCount > 0)
                            return null;
                        else
                        {
                            errorInfo.ErrorInfo.Message = "Need attention";
                            errorInfo.ErrorInfo.Details = "Please add attachment before completing the case";
                            return errorInfo;
                        }
                    }
                }
                else
                {
                    errorInfo.ErrorInfo.Message = "Error";
                    errorInfo.ErrorInfo.Details = "Workorder Action not found";
                    return errorInfo;
                }
            }
            else
            {
                errorInfo.ErrorInfo.Message = "Error";
                errorInfo.ErrorInfo.Details = "Workorder ItemType not found";
                return errorInfo;
            }
        }



        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders)]
        public async Task<PagedResultDto<WorkOrderUpdateWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Subject.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderUpdateWorkOrderLookupTableDto>();
            foreach (var workOrder in workOrderList)
            {
                lookupTableDtoList.Add(new WorkOrderUpdateWorkOrderLookupTableDto
                {
                    Id = workOrder.Id,
                    DisplayName = workOrder.Subject?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderUpdateWorkOrderLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderUpdateWorkOrderActionLookupTableDto>> GetAllWorkOrderActionForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderActionRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Action.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderActionList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderUpdateWorkOrderActionLookupTableDto>();
            foreach (var workOrderAction in workOrderActionList)
            {
                lookupTableDtoList.Add(new WorkOrderUpdateWorkOrderActionLookupTableDto
                {
                    Id = workOrderAction.Id,
                    DisplayName = workOrderAction.Action?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderUpdateWorkOrderActionLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}