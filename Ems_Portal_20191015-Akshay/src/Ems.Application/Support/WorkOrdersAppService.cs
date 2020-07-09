using Ems.Vendors;
using Ems.Authorization.Users;
using Ems.Customers;
using Ems.Assets;
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
using System;
using Abp.Domain.Uow;
using Ems.Organizations;
using Ems.Telematics;
using Abp.UI;
using Abp.Web.Mvc.Models;
using Ems.Storage;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders)]
    public class WorkOrdersAppService : EmsAppServiceBase, IWorkOrdersAppService
    {
        private readonly string _entityType = "WorkOrder";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<WorkOrder> _workOrderRepository;
        private readonly IWorkOrdersExcelExporter _workOrdersExcelExporter;
        private readonly IRepository<WorkOrderPriority, int> _lookup_workOrderPriorityRepository;
        private readonly IRepository<WorkOrderType, int> _lookup_workOrderTypeRepository;
        private readonly IRepository<Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<AssetOwnership, int> _lookup_assetOwnershipRepository;
        private readonly IRepository<WorkOrderStatus, int> _lookup_workOrderStatusRepository;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<Location, int> _lookup_locationRepository;
        private readonly IRepository<UsageMetric, int> _lookup_usageMetricRepository;
        private readonly IRepository<UsageMetricRecord, int> _lookup_usageMetricRecordRepository;
        private readonly IRepository<Attachment, int> _lookup_attachmentRepository;

        public WorkOrdersAppService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<WorkOrder> workOrderRepository,
            IWorkOrdersExcelExporter workOrdersExcelExporter,
            IRepository<WorkOrderPriority, int> lookup_workOrderPriorityRepository,
            IRepository<WorkOrderType, int> lookup_workOrderTypeRepository,
            IRepository<Vendor, int> lookup_vendorRepository,
            IRepository<Incident, int> lookup_incidentRepository,
            IRepository<SupportItem, int> lookup_supportItemRepository,
            IRepository<User, long> lookup_userRepository,
            IRepository<Customer, int> lookup_customerRepository,
            IRepository<AssetOwnership, int> lookup_assetOwnershipRepository,
            IRepository<WorkOrderStatus, int> lookup_workOrderStatusRepository,
            IRepository<LeaseItem> leaseItemtRepository,
            IRepository<Location, int> lookup_locationRepository,
            IRepository<UsageMetric, int> lookup_usageMetricRepository,
            IRepository<UsageMetricRecord, int> lookup_usageMetricRecordRepository,
            IRepository<Attachment, int> lookup_attachmentRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _workOrderRepository = workOrderRepository;
            _workOrdersExcelExporter = workOrdersExcelExporter;
            _lookup_workOrderPriorityRepository = lookup_workOrderPriorityRepository;
            _lookup_workOrderTypeRepository = lookup_workOrderTypeRepository;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_assetOwnershipRepository = lookup_assetOwnershipRepository;
            _lookup_workOrderStatusRepository = lookup_workOrderStatusRepository;
            _leaseItemRepository = leaseItemtRepository;
            _lookup_locationRepository = lookup_locationRepository;
            _lookup_usageMetricRepository = lookup_usageMetricRepository;
            _lookup_usageMetricRecordRepository = lookup_usageMetricRecordRepository;
            _lookup_attachmentRepository = lookup_attachmentRepository;
        }

        public async Task<PagedResultDto<GetWorkOrderForViewDto>> GetAll(GetAllWorkOrdersInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);

                var filteredWorkOrders = _workOrderRepository.GetAll()
                            .Include(e => e.WorkOrderPriorityFk)
                            .Include(e => e.WorkOrderTypeFk)
                            .Include(e => e.VendorFk)
                            .Include(e => e.IncidentFk)
                            .Include(e => e.SupportItemFk)
                            .Include(e => e.UserFk)
                            .Include(e => e.CustomerFk)
                            .Include(e => e.AssetOwnershipFk)
                            .Include(e => e.WorkOrderStatusFk)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Loc8GUID.Contains(input.Filter) || e.Subject.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.Remarks.Contains(input.Filter) || e.Attachments.Contains(input.Filter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Loc8GUIDFilter), e => e.Loc8GUID.ToLower().Contains(input.Loc8GUIDFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.SubjectFilter), e => e.Subject.ToLower().Contains(input.SubjectFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower().Contains(input.DescriptionFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower().Contains(input.LocationFilter.ToLower().Trim()))
                            .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                            .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                            .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                            .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower().Contains(input.RemarksFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderPriorityPriorityFilter), e => e.WorkOrderPriorityFk != null && e.WorkOrderPriorityFk.Priority.ToLower().Contains(input.WorkOrderPriorityPriorityFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderTypeTypeFilter), e => e.WorkOrderTypeFk != null && e.WorkOrderTypeFk.Type.ToLower().Contains(input.WorkOrderTypeTypeFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower().Contains(input.VendorNameFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower().Contains(input.IncidentDescriptionFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower().Contains(input.SupportItemDescriptionFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower().Contains(input.UserNameFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower().Contains(input.CustomerNameFilter.ToLower().Trim()))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnershipAssetFkFilter), e => e.AssetOwnershipFk != null && (e.AssetOwnershipFk.AssetFk.Reference.ToLower().Contains(input.AssetOwnershipAssetFkFilter.ToLower().Trim()) || e.AssetOwnershipFk.AssetFk.Description.ToLower().Contains(input.AssetOwnershipAssetFkFilter.ToLower().Trim())))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderStatusStatusFilter), e => e.WorkOrderStatusFk != null && e.WorkOrderStatusFk.Status.ToLower().Contains(input.WorkOrderStatusStatusFilter.ToLower().Trim()))
                            .WhereIf(!input.IsCompleted, e => e.WorkOrderStatusFk != null && e.WorkOrderStatusFk.Status.ToLower() != "completed")
                            .WhereIf(!input.IsPreventative, e => e.WorkOrderTypeFk != null && !e.WorkOrderTypeFk.Type.ToLower().Contains("preventative"));

                var pagedAndFilteredWorkOrders = filteredWorkOrders
                    .OrderBy(input.Sorting ?? "id desc")
                    .PageBy(input);

                var workOrders = from o in pagedAndFilteredWorkOrders
                                 join o1 in _lookup_workOrderPriorityRepository.GetAll() on o.WorkOrderPriorityId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_workOrderTypeRepository.GetAll() on o.WorkOrderTypeId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 join o3 in _lookup_vendorRepository.GetAll() on o.VendorId equals o3.Id into j3
                                 from s3 in j3.DefaultIfEmpty()

                                 join o4 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o4.Id into j4
                                 from s4 in j4.DefaultIfEmpty()

                                 join o5 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o5.Id into j5
                                 from s5 in j5.DefaultIfEmpty()

                                 join o6 in _lookup_userRepository.GetAll() on o.UserId equals o6.Id into j6
                                 from s6 in j6.DefaultIfEmpty()

                                 join o7 in _lookup_customerRepository.GetAll() on o.CustomerId equals o7.Id into j7
                                 from s7 in j7.DefaultIfEmpty()

                                 join o8 in _lookup_assetOwnershipRepository.GetAll() on o.AssetOwnershipId equals o8.Id into j8
                                 from s8 in j8.DefaultIfEmpty()

                                 join o9 in _lookup_workOrderStatusRepository.GetAll() on o.WorkOrderStatusId equals o9.Id into j9
                                 from s9 in j9.DefaultIfEmpty()

                                 select new GetWorkOrderForViewDto()
                                 {
                                     WorkOrder = new WorkOrderDto
                                     {
                                         Loc8GUID = o.Loc8GUID,
                                         Subject = o.Subject,
                                         Description = o.Description,
                                         Location = o.Location,
                                         StartDate = o.StartDate,
                                         EndDate = o.EndDate,
                                         Remarks = o.Remarks,
                                         Attachments = o.Attachments,
                                         Id = o.Id,
                                         AssetOwnershipId = o.AssetOwnershipId,
                                         CustomerId = o.CustomerId,
                                         IncidentId = o.IncidentId,
                                         SupportItemId = o.SupportItemId,
                                         UserId = o.UserId,
                                         VendorId = o.VendorId,
                                         WorkOrderPriorityId = o.WorkOrderPriorityId,
                                         WorkOrderStatusId = o.WorkOrderStatusId,
                                         WorkOrderTypeId = o.WorkOrderTypeId
                                     },
                                     WorkOrderPriorityPriority = s1 == null ? "" : s1.Priority.ToString(),
                                     WorkOrderTypeType = s2 == null ? "" : s2.Type.ToString(),
                                     VendorName = s3 == null ? "" : s3.Name.ToString(),
                                     IncidentDescription = s4 == null ? "" : s4.Description.ToString(),
                                     SupportItemDescription = s5 == null ? "" : s5.Description.ToString(),
                                     UserName = s6 == null ? "" : s6.Name.ToString(),
                                     CustomerName = s7 == null ? "" : s7.Name.ToString(),
                                     AssetOwnershipAssetDisplayName = s8 == null ? "" : s8.AssetFk.Reference.ToString(),
                                     WorkOrderStatusStatus = s9 == null ? "" : s9.Status.ToString()
                                 };

                var totalCount = await filteredWorkOrders.CountAsync();

                return new PagedResultDto<GetWorkOrderForViewDto>(
                    totalCount,
                    await workOrders.ToListAsync()
                );
            }
        }

        public List<GetWorkorderItemsForViewDto> GetWorkorderItems(GetWorkorderItemsInput input)
        {
            var workOrders = _workOrderRepository.GetAll()
                                .Where(w => w.SupportItemFk != null)
                                .Include(w => w.SupportItemFk.ConsumableTypeFk)
                                .Where(w => !w.IsDeleted)
                                .Where(w => w.StartDate >= DateTime.Now.AddDays(-input.Days))
                                .ToList();

            var supportItemsConsumed =
                    from w in workOrders
                    group w by w.SupportItemFk into s
                    select new GetWorkorderItemsForViewDto()
                    {
                        SupportItem = new SupportItemDto
                        {
                            Description = s.Key.Description,
                            UnitPrice = s.Key.UnitPrice
                        },
                        Consumed = s.Count()
                    };

            var result = supportItemsConsumed.OrderByDescending(a => a.Consumed).Take(10).ToList();

            return result;
        }

        public async Task<GetWorkOrderForViewDto> GetWorkOrderForView(int id)
        {
            var workOrder = await _workOrderRepository.GetAsync(id);

            var output = new GetWorkOrderForViewDto { WorkOrder = ObjectMapper.Map<WorkOrderDto>(workOrder) };

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                if (output.WorkOrder.WorkOrderPriorityId > 0)
                {
                    var _lookupWorkOrderPriority = await _lookup_workOrderPriorityRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderPriorityId);
                    output.WorkOrderPriorityPriority = _lookupWorkOrderPriority?.Priority;
                }

                if (output.WorkOrder.WorkOrderTypeId > 0)
                {
                    var _lookupWorkOrderType = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderTypeId);
                    output.WorkOrderTypeType = _lookupWorkOrderType?.Type;
                }

                if (output.WorkOrder.VendorId > 0)
                {
                    var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.WorkOrder.VendorId);
                    output.VendorName = _lookupVendor?.Name;
                }

                if (output.WorkOrder.IncidentId != null)
                {
                    var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.WorkOrder.IncidentId);
                    output.IncidentDescription = _lookupIncident?.Description;
                }

                if (output.WorkOrder.SupportItemId != null)
                {
                    var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.WorkOrder.SupportItemId);
                    output.SupportItemDescription = _lookupSupportItem?.Description;
                }

                if (output.WorkOrder.UserId > 0)
                {
                    var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.WorkOrder.UserId);
                    output.UserName = _lookupUser?.Name;
                }

                if (output.WorkOrder.CustomerId != null)
                {
                    var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.WorkOrder.CustomerId);
                    output.CustomerName = _lookupCustomer?.Name;
                }

                if (output.WorkOrder.AssetOwnershipId != null)
                {
                    var _lookupAssetOwnership = await _lookup_assetOwnershipRepository.GetAll().Include(a => a.AssetFk).Where(a => a.Id == (int)output.WorkOrder.AssetOwnershipId).FirstOrDefaultAsync();
                    output.AssetOwnershipAssetDisplayName = string.Format("{0} - {1}", _lookupAssetOwnership?.AssetFk?.Reference, _lookupAssetOwnership?.AssetFk?.Description);
                    output.AssetId = _lookupAssetOwnership.AssetFk.Id;
                }

                if (output.WorkOrder.WorkOrderStatusId > 0)
                {
                    var _lookupWorkOrderStatus = await _lookup_workOrderStatusRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderStatusId);
                    output.WorkOrderStatusStatus = _lookupWorkOrderStatus?.Status;
                }
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_Edit)]
        public async Task<GetWorkOrderForEditOutput> GetWorkOrderForEdit(EntityDto input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var workOrder = await _workOrderRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetWorkOrderForEditOutput { WorkOrder = ObjectMapper.Map<CreateOrEditWorkOrderDto>(workOrder) };

            output.TenantType = tenantInfo.Tenant.TenantType;

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                if (output.WorkOrder.WorkOrderPriorityId > 0)
                {
                    var _lookupWorkOrderPriority = await _lookup_workOrderPriorityRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderPriorityId);
                    output.WorkOrderPriorityPriority = _lookupWorkOrderPriority?.Priority;
                }

                if (output.WorkOrder.WorkOrderTypeId > 0)
                {
                    var _lookupWorkOrderType = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderTypeId);
                    output.WorkOrderTypeType = _lookupWorkOrderType?.Type;
                }

                if (output.WorkOrder.VendorId > 0)
                {
                    var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.WorkOrder.VendorId);
                    output.VendorName = _lookupVendor?.Name;
                }

                if (output.WorkOrder.IncidentId != null)
                {
                    var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)output.WorkOrder.IncidentId);
                    output.IncidentDescription = _lookupIncident?.Description;
                }

                if (output.WorkOrder.SupportItemId != null)
                {
                    var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.WorkOrder.SupportItemId);
                    output.SupportItemDescription = _lookupSupportItem.Description;
                }

                if (output.WorkOrder.UserId > 0)
                {
                    var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.WorkOrder.UserId);
                    output.UserName = _lookupUser?.Name;
                }

                if (output.WorkOrder.CustomerId != null)
                {
                    var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.WorkOrder.CustomerId);
                    output.CustomerName = _lookupCustomer?.Name;
                }

                if (output.WorkOrder.AssetOwnershipId != null)
                {
                    var _lookupAssetOwnership = await _lookup_assetOwnershipRepository.GetAll().Include(a => a.AssetFk).Where(a => a.Id == (int)output.WorkOrder.AssetOwnershipId).FirstOrDefaultAsync();
                    output.AssetOwnershipAssetDisplayName = string.Format("{0} - {1}", _lookupAssetOwnership?.AssetFk?.Reference, _lookupAssetOwnership?.AssetFk?.Description);
                }

                if (output.WorkOrder.WorkOrderStatusId > 0)
                {
                    var _lookupWorkOrderStatus = await _lookup_workOrderStatusRepository.FirstOrDefaultAsync((int)output.WorkOrder.WorkOrderStatusId);
                    output.WorkOrderStatusStatus = _lookupWorkOrderStatus?.Status;
                }
            }

            return output;
        }

        public async Task<int> CreateOrEdit(CreateOrEditWorkOrderDto input)
        {
            if (input.Id == null)
            {
                return await Create(input);
            }
            else
            {
                return await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_Create)]
        protected virtual async Task<int> Create(CreateOrEditWorkOrderDto input)
        {
            ErrorViewModel errorInfo = CheckWOValidToSetComplete(input.Id ?? 0, input.WorkOrderStatusId, input.WorkOrderTypeId, input.StartDate, input.AssetOwnershipId);
            if (errorInfo == null)
            {
                var workOrder = ObjectMapper.Map<WorkOrder>(input);

                if (AbpSession.TenantId != null)
                    workOrder.TenantId = (int?)AbpSession.TenantId;

                var tenantInfo = await TenantManager.GetTenantInfo();
                if (tenantInfo.Tenant.TenantType == "C")
                    workOrder.CustomerId = tenantInfo.Customer.Id;
                else if (tenantInfo.Tenant.TenantType == "V")
                    workOrder.VendorId = tenantInfo.Vendor.Id;

                int newWorkOrderId = _workOrderRepository.InsertAndGetId(workOrder);

                if (newWorkOrderId > 0)
                    CreateLocation(input.Location);

                return newWorkOrderId;
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_Edit)]
        protected virtual async Task<int> Update(CreateOrEditWorkOrderDto input)
        {
            ErrorViewModel errorInfo = CheckWOValidToSetComplete(input.Id ?? 0, input.WorkOrderStatusId, input.WorkOrderTypeId, input.StartDate, input.AssetOwnershipId);
            if (errorInfo == null)
            {
                var workOrder = await _workOrderRepository.FirstOrDefaultAsync((int)input.Id);

                ObjectMapper.Map(input, workOrder);
                CreateLocation(input.Location);

                return input.Id ?? 0;
            }
            else
                throw new UserFriendlyException(errorInfo.ErrorInfo.Message, errorInfo.ErrorInfo.Details);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_Edit)]
        public async Task<ErrorViewModel> SetWorkOrderStatusComplete(CreateOrEditWorkOrderDto input)
        {
            var workOrderStatus = _lookup_workOrderStatusRepository.GetAll().Where(w => w.Status == "Completed").FirstOrDefault();
            if (workOrderStatus != null)
            {
                input.WorkOrderStatusId = workOrderStatus.Id;

                ErrorViewModel errorInfo = CheckWOValidToSetComplete(input.Id ?? 0, input.WorkOrderStatusId, input.WorkOrderTypeId, input.StartDate, input.AssetOwnershipId);
                if (errorInfo == null)
                {
                    var workOrder = await _workOrderRepository.FirstOrDefaultAsync((int)input.Id);

                    ObjectMapper.Map(input, workOrder);
                    CreateLocation(input.Location);
                }

                return errorInfo;
            }
            else
                throw new UserFriendlyException("Error", "Work order status not found. Please try again later.");
        }

        [AbpAuthorize(AppPermissions.Pages_Main_WorkOrders_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _workOrderRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetWorkOrdersToExcel(GetAllWorkOrdersForExcelInput input)
        {
            var filteredWorkOrders = _workOrderRepository.GetAll()
                        .Include(e => e.WorkOrderPriorityFk)
                        .Include(e => e.WorkOrderTypeFk)
                        .Include(e => e.VendorFk)
                        .Include(e => e.IncidentFk)
                        .Include(e => e.SupportItemFk)
                        .Include(e => e.UserFk)
                        .Include(e => e.CustomerFk)
                        .Include(e => e.AssetOwnershipFk)
                        .Include(e => e.WorkOrderStatusFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Loc8GUID.Contains(input.Filter) || e.Subject.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Location.Contains(input.Filter) || e.Remarks.Contains(input.Filter) || e.Attachments.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Loc8GUIDFilter), e => e.Loc8GUID.ToLower() == input.Loc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SubjectFilter), e => e.Subject.ToLower() == input.SubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LocationFilter), e => e.Location.ToLower() == input.LocationFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower() == input.RemarksFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderPriorityPriorityFilter), e => e.WorkOrderPriorityFk != null && e.WorkOrderPriorityFk.Priority.ToLower() == input.WorkOrderPriorityPriorityFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderTypeTypeFilter), e => e.WorkOrderTypeFk != null && e.WorkOrderTypeFk.Type.ToLower() == input.WorkOrderTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IncidentDescriptionFilter), e => e.IncidentFk != null && e.IncidentFk.Description.ToLower() == input.IncidentDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower() == input.SupportItemDescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name.ToLower() == input.UserNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnershipAssetFkFilter), e => e.AssetOwnershipFk != null && e.AssetOwnershipFk.AssetFk.Description.ToLower() == input.AssetOwnershipAssetFkFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderStatusStatusFilter), e => e.WorkOrderStatusFk != null && e.WorkOrderStatusFk.Status.ToLower() == input.WorkOrderStatusStatusFilter.ToLower().Trim())
                        .WhereIf(!input.IsCompleted, e => e.WorkOrderStatusFk != null && e.WorkOrderStatusFk.Status.ToLower() != "completed")
                        .WhereIf(!input.IsPreventative, e => e.WorkOrderTypeFk != null && !e.WorkOrderTypeFk.Type.ToLower().Contains("preventative"));

            var query = (from o in filteredWorkOrders
                         join o1 in _lookup_workOrderPriorityRepository.GetAll() on o.WorkOrderPriorityId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_workOrderTypeRepository.GetAll() on o.WorkOrderTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_vendorRepository.GetAll() on o.VendorId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_incidentRepository.GetAll() on o.IncidentId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         join o6 in _lookup_userRepository.GetAll() on o.UserId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                         join o7 in _lookup_customerRepository.GetAll() on o.CustomerId equals o7.Id into j7
                         from s7 in j7.DefaultIfEmpty()

                         join o8 in _lookup_assetOwnershipRepository.GetAll() on o.AssetOwnershipId equals o8.Id into j8
                         from s8 in j8.DefaultIfEmpty()

                         join o9 in _lookup_workOrderStatusRepository.GetAll() on o.WorkOrderStatusId equals o9.Id into j9
                         from s9 in j9.DefaultIfEmpty()

                         select new GetWorkOrderForViewDto()
                         {
                             WorkOrder = new WorkOrderDto
                             {
                                 Loc8GUID = o.Loc8GUID,
                                 Subject = o.Subject,
                                 Description = o.Description,
                                 Location = o.Location,
                                 StartDate = o.StartDate,
                                 EndDate = o.EndDate,
                                 Remarks = o.Remarks,
                                 Attachments = o.Attachments,
                                 Id = o.Id
                             },
                             WorkOrderPriorityPriority = s1 == null ? "" : s1.Priority.ToString(),
                             WorkOrderTypeType = s2 == null ? "" : s2.Type.ToString(),
                             VendorName = s3 == null ? "" : s3.Name.ToString(),
                             IncidentDescription = s4 == null ? "" : s4.Description.ToString(),
                             SupportItemDescription = s5 == null ? "" : s5.Description.ToString(),
                             UserName = s6 == null ? "" : s6.Name.ToString(),
                             CustomerName = s7 == null ? "" : s7.Name.ToString(),
                             AssetOwnershipAssetDisplayName = s8 == null ? "" : s8.AssetFk.Reference.ToString(),
                             WorkOrderStatusStatus = s9 == null ? "" : s9.Status.ToString()
                         });


            var workOrderListDtos = await query.ToListAsync();

            return _workOrdersExcelExporter.ExportToFile(workOrderListDtos);
        }

        protected async void CreateLocation(string location)
        {
            var query = _lookup_locationRepository.GetAll().Where(e => e.LocationName.Trim() == location.Trim()).FirstOrDefault();
            if (query == null)
            {
                Location loc = new Location()
                {
                    LocationName = location,
                    UserId = AbpSession.UserId,
                    TenantId = AbpSession.TenantId != null ? AbpSession.TenantId : 0
                };

                await _lookup_locationRepository.InsertAsync(loc);
            }
        }

        protected ErrorViewModel CheckWOValidToSetComplete(int workOrderId, int woStatusId, int woTypeId, DateTime? woStartDate, int? assetOwnerShipId)
        {
            ErrorViewModel errorInfo = new ErrorViewModel();
            errorInfo.ErrorInfo = new Abp.Web.Models.ErrorInfo();

            var woStatusInfo = _lookup_workOrderStatusRepository.FirstOrDefault(woStatusId);
            if (woStatusInfo != null)
            {
                if (woStatusInfo.Status.ToLower() != "completed")
                    return null;

                var assetOwnershipInfo = _lookup_assetOwnershipRepository.FirstOrDefault(assetOwnerShipId ?? 0);
                if (assetOwnershipInfo != null)
                {
                    int assetId = assetOwnershipInfo.AssetId ?? 0;

                    var usageMetric = _lookup_usageMetricRepository.GetAll()
                         .Where(e => e.AssetId == assetId)
                         .Select(s => new Telematics.Dtos.GetUsageMetricForViewDto()
                         {
                             UsageMetric = new Telematics.Dtos.UsageMetricDto
                             {
                                 Id = s.Id
                             },
                             NeedRecordUpdate = (woStartDate != null) ? ((_lookup_usageMetricRecordRepository.GetAll().Where(w => w.UsageMetricId == s.Id).Count() > 0) ? (_lookup_usageMetricRecordRepository.GetAll().Where(w => w.UsageMetricId == s.Id && (w.CreationTime > woStartDate || w.LastModificationTime > woStartDate)).Count() == 0) : false) : false
                         });

                    if (usageMetric != null && usageMetric.Count() > 0 && usageMetric.Count() == usageMetric.Where(w => w.NeedRecordUpdate == false).Count())
                    {
                        var woTypeInfo = _lookup_workOrderTypeRepository.FirstOrDefault(woTypeId);
                        if (woTypeInfo != null)
                        {
                            if (woTypeInfo.Type.ToLower() != "inspection" || workOrderId == 0)
                                return null;
                            else
                            {
                                int attachmentCount = _lookup_attachmentRepository.GetAll()
                                    //.WhereIf(woStartDate != null, w => w.CreationTime > woStartDate || w.LastModificationTime > woStartDate)
                                    .Where(w => w.WorkOrderId == workOrderId)
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
                            errorInfo.ErrorInfo.Details = "Workorder type not found";
                            return errorInfo;
                        }
                    }
                    else
                    {
                        errorInfo.ErrorInfo.Message = "Need attention";
                        errorInfo.ErrorInfo.Details = "Please update usage records before completing the case";
                        return errorInfo;
                    }
                }
                else
                {
                    errorInfo.ErrorInfo.Message = "Error";
                    errorInfo.ErrorInfo.Details = "Asset ownership info not found";
                    return errorInfo;
                }
            }
            else
            {
                errorInfo.ErrorInfo.Message = "Error";
                errorInfo.ErrorInfo.Details = "Workorder status not found";
                return errorInfo;
            }
        }


        public async Task<PagedResultDto<WorkOrderWorkOrderPriorityLookupTableDto>> GetAllWorkOrderPriorityForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderPriorityRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Priority.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderPriorityList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderWorkOrderPriorityLookupTableDto>();
            foreach (var workOrderPriority in workOrderPriorityList)
            {
                lookupTableDtoList.Add(new WorkOrderWorkOrderPriorityLookupTableDto
                {
                    Id = workOrderPriority.Id,
                    DisplayName = workOrderPriority.Priority?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderWorkOrderPriorityLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderWorkOrderTypeLookupTableDto>> GetAllWorkOrderTypeForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderTypeRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Type.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderWorkOrderTypeLookupTableDto>();
            foreach (var workOrderType in workOrderTypeList)
            {
                lookupTableDtoList.Add(new WorkOrderWorkOrderTypeLookupTableDto
                {
                    Id = workOrderType.Id,
                    DisplayName = workOrderType.Type?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderWorkOrderTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Asset OwnerShip ID

            var tenantInfo = await TenantManager.GetTenantInfo();
            int assetId = _lookup_assetOwnershipRepository.Get(input.FilterId)?.AssetId ?? 0;

            IQueryable<Vendor> query;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A": // Get all vendors related to the Asset (in most cases will 0 or 1)
                case "C": // Same for AssetOwner and Customer

                    query = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Where(e => e.AssetId == assetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.SupportContractFk.VendorFk.Name.Contains(input.Filter))
                        .Select(e => e.SupportContractFk.VendorFk);

                    break;

                case "V": // Just use the Vendor's Id

                    query = _lookup_vendorRepository
                        .GetAll()
                        .Where(e => e.Id == tenantInfo.Vendor.Id);
                    break;

                case "H": // Get Everything

                    query = _lookup_vendorRepository
                        .GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter));
                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderVendorLookupTableDto>();
            foreach (var vendor in vendorList)
            {
                lookupTableDtoList.Add(new WorkOrderVendorLookupTableDto
                {
                    Id = vendor.Id,
                    DisplayName = vendor.Name
                });
            }

            return new PagedResultDto<WorkOrderVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_incidentRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Description.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var incidentList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderIncidentLookupTableDto>();
            foreach (var incident in incidentList)
            {
                lookupTableDtoList.Add(new WorkOrderIncidentLookupTableDto
                {
                    Id = incident.Id,
                    DisplayName = incident.Description?.ToString()
                });
            }

            return new PagedResultDto<WorkOrderIncidentLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Asset OwnerShip ID

            var tenantInfo = await TenantManager.GetTenantInfo();
            int assetId = _lookup_assetOwnershipRepository.Get(input.FilterId)?.AssetId ?? 0;

            IQueryable<SupportItem> query;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A": // Get all Support Items related to the selected Asset
                case "C":
                case "V":
                    query = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter));
                    break;

                case "H": // Get Everything
                    query = _lookup_supportItemRepository
                        .GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter));
                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var totalCount = await query.CountAsync();

            var supportItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderSupportItemLookupTableDto>();
            foreach (var supportItem in supportItemList)
            {
                lookupTableDtoList.Add(new WorkOrderSupportItemLookupTableDto
                {
                    Id = supportItem.Id,
                    DisplayName = supportItem.Description
                });
            }

            return new PagedResultDto<WorkOrderSupportItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "User");

                var query = _lookup_userRepository.GetAll()
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter));

                var totalCount = await query.CountAsync();

                var userList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<WorkOrderUserLookupTableDto>();
                foreach (var user in userList)
                {
                    lookupTableDtoList.Add(new WorkOrderUserLookupTableDto
                    {
                        Id = user.Id,
                        DisplayName = String.Format("{0} [{1}]", user.Name, user.EmailAddress)
                    });
                }

                return new PagedResultDto<WorkOrderUserLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        public async Task<PagedResultDto<WorkOrderCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Asset OwnerShip ID

            var tenantInfo = await TenantManager.GetTenantInfo();
            int assetId = _lookup_assetOwnershipRepository.Get(input.FilterId)?.AssetId ?? 0;

            IQueryable<Customer> query;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A":
                case "V":

                    query = _leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.LeaseAgreementFk.CustomerFk.Name.Contains(input.Filter))
                        .Select(e => e.LeaseAgreementFk.CustomerFk);

                    break;

                case "C": // Just use the Customer's Id
                    query = _lookup_customerRepository
                        .GetAll()
                        .Where(e => e.Id == tenantInfo.Customer.Id);
                    break;

                case "H": // Get Everything
                    query = _lookup_customerRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter)
               );
                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new WorkOrderCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name
                });
            }

            return new PagedResultDto<WorkOrderCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderAssetOwnershipLookupTableDto>> GetAllAssetOwnershipForLookupTable(GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Incident ID

            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Asset");
            int assetId = 0;

            if (input.FilterId > 0)
                assetId = _lookup_incidentRepository.Get(input.FilterId)?.AssetId ?? 0;

            IQueryable<AssetOwnership> query;

            if (assetId > 0)
            {
                query = _lookup_assetOwnershipRepository
                        .GetAll()
                        .Include(a => a.AssetFk).WhereIf(!string.IsNullOrWhiteSpace(input.Filter), (e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter)))
                        .Where(e => e.AssetId == assetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), (e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter)));

            }
            else
            {
                List<int> assetIdList = null;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                        assetIdList = _leaseItemRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .Include(e => e.LeaseAgreementFk)
                            .Where(e => e.LeaseAgreementFk.CustomerId == tenantInfo.Customer.Id)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter))
                            .Select(s => s.AssetFk.Id).ToList();

                        query = _lookup_assetOwnershipRepository
                            .GetAll()
                            .Where(e => assetIdList.Contains((int)e.AssetId));

                        break;

                    case "V":
                        assetIdList = _lookup_supportItemRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .Include(e => e.SupportContractFk)
                            .Where(e => e.SupportContractFk.VendorId == tenantInfo.Vendor.Id)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter))
                            .Select(s => s.AssetFk.Id).ToList();

                        query = _lookup_assetOwnershipRepository
                            .GetAll()
                            .Where(e => assetIdList.Contains((int)e.AssetId));

                        break;

                    case "A":
                        query = _lookup_assetOwnershipRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter));
                        break;

                    case "H":
                        query = _lookup_assetOwnershipRepository
                            .GetAll()
                            .Include(e => e.AssetFk)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter) || e.AssetFk.Description.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }
            }

            var totalCount = await query.CountAsync();

            var assetOwnershipList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderAssetOwnershipLookupTableDto>();
            foreach (var assetOwnership in assetOwnershipList)
            {
                lookupTableDtoList.Add(new WorkOrderAssetOwnershipLookupTableDto
                {
                    Id = assetOwnership.Id,
                    DisplayName = string.Format("{0} - {1}", assetOwnership.AssetFk.Reference, assetOwnership.AssetFk.Description)
                });
            }

            return new PagedResultDto<WorkOrderAssetOwnershipLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<WorkOrderWorkOrderStatusLookupTableDto>> GetAllWorkOrderStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_workOrderStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var workOrderStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<WorkOrderWorkOrderStatusLookupTableDto>();
            foreach (var workOrderStatus in workOrderStatusList)
            {
                lookupTableDtoList.Add(new WorkOrderWorkOrderStatusLookupTableDto
                {
                    Id = workOrderStatus.Id,
                    DisplayName = workOrderStatus.Status
                });
            }

            return new PagedResultDto<WorkOrderWorkOrderStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<WorkOrderAssetFkListDto> GetAssetFkList(int incidentId, int assetOwnerShipId)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            int assetId = 0;

            if (incidentId > 0)
                assetId = _lookup_incidentRepository.Get(incidentId)?.AssetId ?? 0;
            else
                assetId = _lookup_assetOwnershipRepository.Get(assetOwnerShipId)?.AssetId ?? 0;

            IQueryable<SupportItem> supportItemQuery;
            IQueryable<Customer> customerQuery;
            IQueryable<AssetOwnership> assetOwnerQuery;
            IQueryable<Vendor> vendorQuery;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "A":
                case "H":
                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId);

                    customerQuery = _leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.LeaseAgreementFk.CustomerFk);

                    assetOwnerQuery = _lookup_assetOwnershipRepository
                    .GetAll()
                    .Include(a => a.AssetFk)
                    .Where(e => e.AssetId == assetId);

                    vendorQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.SupportContractFk.VendorFk);

                    break;

                case "V":
                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId);

                    customerQuery = _leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.LeaseAgreementFk.CustomerFk);

                    assetOwnerQuery = _lookup_assetOwnershipRepository
                        .GetAll()
                        .Include(a => a.AssetFk)
                        .Where(e => e.AssetId == assetId);

                    vendorQuery = _lookup_vendorRepository
                        .GetAll()
                        .Where(e => e.Id == tenantInfo.Vendor.Id);

                    break;

                case "C":
                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetId);

                    customerQuery = null;

                    assetOwnerQuery = _lookup_assetOwnershipRepository
                        .GetAll()
                        .Include(a => a.AssetFk)
                        .Where(e => e.AssetId == assetId);

                    vendorQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.SupportContractFk.VendorFk);

                    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var customerTableDtoList = new List<WorkOrderCustomerLookupTableDto>();
            var supportItemTableDtoList = new List<WorkOrderSupportItemLookupTableDto>();
            var assetOwnerTableDtoList = new List<WorkOrderAssetOwnershipLookupTableDto>();
            var vendorTableDtoList = new List<WorkOrderVendorLookupTableDto>();

            if (customerQuery?.Count() > 0)
            {
                foreach (var customer in customerQuery)
                {
                    customerTableDtoList.Add(new WorkOrderCustomerLookupTableDto
                    {
                        Id = customer.Id,
                        DisplayName = customer.Name
                    });
                }
            }

            if (supportItemQuery?.Count() > 0)
            {
                foreach (var supportItem in supportItemQuery)
                {
                    supportItemTableDtoList.Add(new WorkOrderSupportItemLookupTableDto
                    {
                        Id = supportItem.Id,
                        DisplayName = supportItem.Description
                    });
                }
            }

            if (assetOwnerQuery?.Count() > 0)
            {
                foreach (var assetOwner in assetOwnerQuery)
                {
                    assetOwnerTableDtoList.Add(new WorkOrderAssetOwnershipLookupTableDto
                    {
                        Id = assetOwner.Id,
                        DisplayName = string.Format("{0} - {1}", assetOwner.AssetFk.Reference, assetOwner.AssetFk.Description)
                    });
                }
            }

            if (vendorQuery?.Count() > 0)
            {
                foreach (var vendor in vendorQuery)
                {
                    vendorTableDtoList.Add(new WorkOrderVendorLookupTableDto
                    {
                        Id = vendor.Id,
                        DisplayName = vendor.Name
                    });
                }
            }

            return new WorkOrderAssetFkListDto { CustomerList = customerTableDtoList, SupportItemList = supportItemTableDtoList, AssetOwnerList = assetOwnerTableDtoList, VendorList = vendorTableDtoList };
        }

        public async Task<WorkOrderWorkOrderPriorityLookupTableDto> GetWorkOrderPriorityByType(int workOrderTypeId)
        {
            string priority = "Medium";
            var woTypeInfo = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync(workOrderTypeId);

            if (woTypeInfo != null && (woTypeInfo.Type == "Corrective Maintenance" || woTypeInfo.Type == "Breakdown"))
                priority = "High";

            return _lookup_workOrderPriorityRepository.GetAll()
                    .Where(w => w.Priority == priority)
                    .Select(s => new WorkOrderWorkOrderPriorityLookupTableDto { Id = s.Id, DisplayName = s.Priority }).FirstOrDefault();
        }

        public async Task<WorkOrderUserLookupTableDto> GetDefaultCreator()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.UserId.ToString());

            if (user != null)
            {
                return new WorkOrderUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = String.Format("{0} [{1}]", user.Name, user.EmailAddress)
                };
            }

            return null;
        }

        public async Task<PagedResultDto<Organizations.Dtos.LocationLookupTableDto>> GetAllLocationForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_locationRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.LocationName.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var locationList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<Organizations.Dtos.LocationLookupTableDto>();
            foreach (var loc in locationList)
            {
                lookupTableDtoList.Add(new Organizations.Dtos.LocationLookupTableDto
                {
                    Id = loc.Id,
                    DisplayName = loc.LocationName
                });
            }

            return new PagedResultDto<Organizations.Dtos.LocationLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}