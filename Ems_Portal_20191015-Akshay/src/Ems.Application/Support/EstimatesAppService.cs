using Ems.Quotations;
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
using Ems.Assets;
using Ems.Metrics;
using Abp.Domain.Uow;
using System;
using Ems.Authorization.Users;
using Ems.Customers;
using Ems.Organizations;

namespace Ems.Support
{
    [AbpAuthorize(AppPermissions.Pages_Main_Estimates)]
    public class EstimatesAppService : EmsAppServiceBase, IEstimatesAppService
    {
        private readonly string _entityType = "Estimate";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Estimate> _estimateRepository;
        private readonly IRepository<EstimateDetail> _estimateDetailRepository;
        private readonly IEstimatesExcelExporter _estimatesExcelExporter;
        private readonly IRepository<Quotation, int> _lookup_quotationRepository;
        private readonly IRepository<EstimateStatus, int> _lookup_estimateStatusRepository;

        private readonly IRepository<QuotationDetail> _quotationDetailRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<SupportType, int> _lookup_supportTypeRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<WorkOrderUpdate, int> _lookup_workOrderUpdateRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;
        private readonly IRepository<WorkOrderPriority, int> _lookup_workOrderPriorityRepository;
        private readonly IRepository<WorkOrderType, int> _lookup_workOrderTypeRepository;
        private readonly IRepository<Vendors.Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<AssetOwnership, int> _lookup_assetOwnershipRepository;
        private readonly IRepository<WorkOrderStatus, int> _lookup_workOrderStatusRepository;
        private readonly IRepository<SupportContract, int> _lookup_supportContractRepository;
        private readonly IRepository<QuotationStatus, int> _lookup_quotationStatusRepository;
        private readonly IRepository<Contact, int> _lookup_contactRepository;
        private readonly IRepository<Address, int> _lookup_addressRepository;
        private readonly IRepository<AssetOwner, int> _lookup_assetOwnerRepository;
        private readonly IRepository<LeaseItem, int> _lookup_leaseItemRepository;

        public EstimatesAppService(IUnitOfWorkManager unitOfWorkManager,
            IRepository<Estimate> estimateRepository,
            IRepository<EstimateDetail> estimateDetailRepository,
            IEstimatesExcelExporter estimatesExcelExporter,
            IRepository<Quotation, int> lookup_quotationRepository,
            IRepository<EstimateStatus, int> lookup_estimateStatusRepository,
            IRepository<QuotationDetail> quotationDetailRepository,
            IRepository<Asset, int> lookup_assetRepository,
            IRepository<AssetClass, int> lookup_assetClassRepository,
            IRepository<ItemType, int> lookup_itemTypeRepository,
            IRepository<SupportType, int> lookup_supportTypeRepository,
            IRepository<Uom, int> lookup_uomRepository,
            IRepository<SupportItem, int> lookup_supportItemRepository,
            IRepository<WorkOrder, int> lookup_workOrderRepository,
            IRepository<WorkOrderUpdate, int> lookup_workOrderUpdateRepository,
            IRepository<WorkOrderAction, int> lookup_workOrderActionRepository,
            IRepository<WorkOrderPriority, int> lookup_workOrderPriorityRepository,
            IRepository<WorkOrderType, int> lookup_workOrderTypeRepository,
            IRepository<Vendors.Vendor, int> lookup_vendorRepository,
            IRepository<Incident, int> lookup_incidentRepository,
            IRepository<User, long> lookup_userRepository,
            IRepository<Customer, int> lookup_customerRepository,
            IRepository<AssetOwnership, int> lookup_assetOwnershipRepository,
            IRepository<WorkOrderStatus, int> lookup_workOrderStatusRepository,
            IRepository<SupportContract, int> lookup_supportContractRepository,
            IRepository<QuotationStatus, int> lookup_quotationStatusRepository,
            IRepository<Contact, int> lookup_contactRepository,
            IRepository<Address, int> lookup_addressRepository,
            IRepository<AssetOwner, int> lookup_assetOwnerRepository,
            IRepository<LeaseItem, int> lookup_leaseItemRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _estimateRepository = estimateRepository;
            _estimateDetailRepository = estimateDetailRepository;
            _estimatesExcelExporter = estimatesExcelExporter;
            _lookup_quotationRepository = lookup_quotationRepository;
            _lookup_estimateStatusRepository = lookup_estimateStatusRepository;

            _quotationDetailRepository = quotationDetailRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_supportTypeRepository = lookup_supportTypeRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_workOrderUpdateRepository = lookup_workOrderUpdateRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
            _lookup_workOrderPriorityRepository = lookup_workOrderPriorityRepository;
            _lookup_workOrderTypeRepository = lookup_workOrderTypeRepository;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_assetOwnershipRepository = lookup_assetOwnershipRepository;
            _lookup_workOrderStatusRepository = lookup_workOrderStatusRepository;
            _lookup_supportContractRepository = lookup_supportContractRepository;
            _lookup_quotationStatusRepository = lookup_quotationStatusRepository;
            _lookup_contactRepository = lookup_contactRepository;
            _lookup_addressRepository = lookup_addressRepository;
            _lookup_assetOwnerRepository = lookup_assetOwnerRepository;
            _lookup_leaseItemRepository = lookup_leaseItemRepository;
        }

        public async Task<PagedResultDto<GetEstimateForViewDto>> GetAll(GetAllEstimatesInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);
                var tenantType = tenantInfo.Tenant.TenantType;


                var filteredEstimates = _estimateRepository.GetAll()
                            .Include(e => e.CustomerFk)
                            .Include(e => e.QuotationFk)
                            .Include(e => e.WorkOrderFk)
                            .Include(e => e.EstimateStatusFk)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.QuotationLoc8GUID.Contains(input.Filter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                            .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                            .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                            .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                            .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                            .WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
                            .WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
                            .WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
                            .WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
                            .WhereIf(input.MinTotalDiscountFilter != null, e => e.TotalDiscount >= input.MinTotalDiscountFilter)
                            .WhereIf(input.MaxTotalDiscountFilter != null, e => e.TotalDiscount <= input.MaxTotalDiscountFilter)
                            .WhereIf(input.MinTotalChargeFilter != null, e => e.TotalCharge >= input.MinTotalChargeFilter)
                            .WhereIf(input.MaxTotalChargeFilter != null, e => e.TotalCharge <= input.MaxTotalChargeFilter)
                            .WhereIf(input.MinVersionFilter != null, e => e.Version >= input.MinVersionFilter)
                            .WhereIf(input.MaxVersionFilter != null, e => e.Version <= input.MaxVersionFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.RemarkFilter), e => e.Remark.ToLower() == input.RemarkFilter.ToLower().Trim())
                            .WhereIf(input.MinRequoteRefIdFilter != null, e => e.RequoteRefId >= input.MinRequoteRefIdFilter)
                            .WhereIf(input.MaxRequoteRefIdFilter != null, e => e.RequoteRefId <= input.MaxRequoteRefIdFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationLoc8GUIDFilter), e => e.QuotationLoc8GUID.ToLower() == input.QuotationLoc8GUIDFilter.ToLower().Trim())
                            .WhereIf(input.MinAcknowledgedByFilter != null, e => e.AcknowledgedBy >= input.MinAcknowledgedByFilter)
                            .WhereIf(input.MaxAcknowledgedByFilter != null, e => e.AcknowledgedBy <= input.MaxAcknowledgedByFilter)
                            .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                            .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.Contains(input.WorkOrderSubjectFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.Contains(input.QuotationTitleFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.EstimateStatusStatusFilter), e => e.EstimateStatusFk != null && e.EstimateStatusFk.Status.Contains(input.EstimateStatusStatusFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.Contains(input.CustomerNameFilter));

                var pagedAndFilteredEstimates = filteredEstimates
                    .OrderBy(input.Sorting ?? "id desc")
                    .PageBy(input);

                var estimates = from o in pagedAndFilteredEstimates
                                join o1 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()

                                join o2 in _lookup_estimateStatusRepository.GetAll() on o.EstimateStatusId equals o2.Id into j2
                                from s2 in j2.DefaultIfEmpty()

                                join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                                from s3 in j3.DefaultIfEmpty()

                                join o4 in _lookup_customerRepository.GetAll() on o.CustomerId equals o4.Id into j4
                                from s4 in j4.DefaultIfEmpty()

                                select new GetEstimateForViewDto()
                                {
                                    Estimate = new EstimateDto
                                    {
                                        Reference = o.Reference,
                                        Title = o.Title,
                                        Description = o.Description,
                                        StartDate = o.StartDate,
                                        EndDate = o.EndDate,
                                        TotalTax = o.TotalTax,
                                        TotalPrice = o.TotalPrice,
                                        TotalDiscount = o.TotalDiscount,
                                        TotalCharge = o.TotalCharge,
                                        Version = o.Version,
                                        Remark = o.Remark,
                                        RequoteRefId = o.RequoteRefId,
                                        QuotationLoc8GUID = o.QuotationLoc8GUID,
                                        AcknowledgedBy = o.AcknowledgedBy,
                                        AcknowledgedAt = o.AcknowledgedAt,
                                        Id = o.Id,
                                        CustomerId = o.CustomerId
                                    },
                                    QuotationTitle = s1 == null ? "" : s1.Title,
                                    EstimateStatusStatus = s2 == null ? "" : s2.Status,
                                    WorkOrderSubject = s3 == null ? "" : s3.Subject,
                                    CustomerName = s4 == null ? "" : s4.Name
                                };

                var totalCount = await filteredEstimates.CountAsync();

                return new PagedResultDto<GetEstimateForViewDto>(
                    totalCount,
                    await estimates.ToListAsync()
                );
            }
        }

        public async Task<GetEstimateForViewDto> GetEstimateForView(int id)
        {
            var estimate = await _estimateRepository.GetAsync(id);

            var output = new GetEstimateForViewDto { Estimate = ObjectMapper.Map<EstimateDto>(estimate) };

            if (output?.Estimate != null)
            {
                if (output.Estimate.QuotationId > 0)
                {
                    var quotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.Estimate.QuotationId);
                    output.QuotationTitle = quotation?.Title;
                }

                if (output.Estimate.EstimateStatusId > 0)
                {
                    var _lookupEstimateStatus = await _lookup_estimateStatusRepository.FirstOrDefaultAsync((int)output.Estimate.EstimateStatusId);
                    output.EstimateStatusStatus = _lookupEstimateStatus.Status;
                }

                if (output.Estimate.WorkOrderId > 0)
                {
                    var workOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Estimate.WorkOrderId);
                    output.WorkOrderSubject = workOrder.Subject;
                }

                if (output.Estimate.CustomerId > 0)
                {
                    var customer = await _lookup_customerRepository.FirstOrDefaultAsync(output.Estimate.CustomerId);
                    output.CustomerName = customer.Name;
                }
            }
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_Edit)]
        public async Task<GetEstimateForEditOutput> GetEstimateForEdit(EntityDto input)
        {
            var estimate = await _estimateRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetEstimateForEditOutput { Estimate = ObjectMapper.Map<CreateOrEditEstimateDto>(estimate) };

            if (output.Estimate.QuotationId != null)
            {
                var _lookupQuotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.Estimate.QuotationId);
                output.QuotationTitle = _lookupQuotation.Title.ToString();
            }

            if (output.Estimate.WorkOrderId > 0)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Estimate.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject;
            }

            if (output.Estimate.EstimateStatusId > 0)
            {
                var _lookupEstimateStatus = await _lookup_estimateStatusRepository.FirstOrDefaultAsync((int)output.Estimate.EstimateStatusId);
                output.EstimateStatusStatus = _lookupEstimateStatus.Status;
            }

            if (output.Estimate.CustomerId > 0)
            {
                var customer = await _lookup_customerRepository.FirstOrDefaultAsync(output.Estimate.CustomerId);
                output.CustomerName = customer.Name;
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditEstimateDto input)
        {
            if (input.Id == null)
            {
                //int quotationId = input?.QuotationId ?? 0;
                //if (quotationId > 0)
                //{
                //    var quotationInfo = _lookup_quotationRepository.Get(quotationId);

                //    if (quotationInfo != null && quotationInfo.Id > 0)
                //    {
                //        input.TotalCharge = quotationInfo.TotalCharge;
                //        input.TotalDiscount = quotationInfo.TotalDiscount;
                //        input.TotalPrice = quotationInfo.TotalPrice;
                //        input.TotalTax = quotationInfo.TotalTax;
                //    }
                //}

                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_Create)]
        protected virtual async Task Create(CreateOrEditEstimateDto input)
        {
            var estimate = ObjectMapper.Map<Estimate>(input);

            if (AbpSession.TenantId != null)
                estimate.TenantId = (int?)AbpSession.TenantId;

            await _estimateRepository.InsertAsync(estimate);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_Edit)]
        protected virtual async Task Update(CreateOrEditEstimateDto input)
        {
            var estimate = await _estimateRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, estimate);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Estimates_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _estimateRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetEstimatesToExcel(GetAllEstimatesForExcelInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);
                var tenantType = tenantInfo.Tenant.TenantType;

                var filteredEstimates = _estimateRepository.GetAll()
                            .Include(e => e.CustomerFk)
                            .Include(e => e.QuotationFk)
                            .Include(e => e.WorkOrderFk)
                            .Include(e => e.EstimateStatusFk)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.QuotationLoc8GUID.Contains(input.Filter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                            .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                            .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                            .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                            .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                            .WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
                            .WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
                            .WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
                            .WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
                            .WhereIf(input.MinTotalDiscountFilter != null, e => e.TotalDiscount >= input.MinTotalDiscountFilter)
                            .WhereIf(input.MaxTotalDiscountFilter != null, e => e.TotalDiscount <= input.MaxTotalDiscountFilter)
                            .WhereIf(input.MinTotalChargeFilter != null, e => e.TotalCharge >= input.MinTotalChargeFilter)
                            .WhereIf(input.MaxTotalChargeFilter != null, e => e.TotalCharge <= input.MaxTotalChargeFilter)
                            .WhereIf(input.MinVersionFilter != null, e => e.Version >= input.MinVersionFilter)
                            .WhereIf(input.MaxVersionFilter != null, e => e.Version <= input.MaxVersionFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.RemarkFilter), e => e.Remark.ToLower() == input.RemarkFilter.ToLower().Trim())
                            .WhereIf(input.MinRequoteRefIdFilter != null, e => e.RequoteRefId >= input.MinRequoteRefIdFilter)
                            .WhereIf(input.MaxRequoteRefIdFilter != null, e => e.RequoteRefId <= input.MaxRequoteRefIdFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationLoc8GUIDFilter), e => e.QuotationLoc8GUID.ToLower() == input.QuotationLoc8GUIDFilter.ToLower().Trim())
                            .WhereIf(input.MinAcknowledgedByFilter != null, e => e.AcknowledgedBy >= input.MinAcknowledgedByFilter)
                            .WhereIf(input.MaxAcknowledgedByFilter != null, e => e.AcknowledgedBy <= input.MaxAcknowledgedByFilter)
                            .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                            .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.Contains(input.WorkOrderSubjectFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.Contains(input.QuotationTitleFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.EstimateStatusStatusFilter), e => e.EstimateStatusFk != null && e.EstimateStatusFk.Status.Contains(input.EstimateStatusStatusFilter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.Contains(input.CustomerNameFilter));

                var query = (from o in filteredEstimates
                             join o1 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o1.Id into j1
                             from s1 in j1.DefaultIfEmpty()

                             join o2 in _lookup_estimateStatusRepository.GetAll() on o.EstimateStatusId equals o2.Id into j2
                             from s2 in j2.DefaultIfEmpty()

                             join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                             from s3 in j3.DefaultIfEmpty()

                             join o4 in _lookup_customerRepository.GetAll() on o.CustomerId equals o4.Id into j4
                             from s4 in j4.DefaultIfEmpty()

                             select new GetEstimateForViewDto()
                             {
                                 Estimate = new EstimateDto
                                 {
                                     Reference = o.Reference,
                                     Title = o.Title,
                                     Description = o.Description,
                                     StartDate = o.StartDate,
                                     EndDate = o.EndDate,
                                     TotalTax = o.TotalTax,
                                     TotalPrice = o.TotalPrice,
                                     TotalDiscount = o.TotalDiscount,
                                     TotalCharge = o.TotalCharge,
                                     Version = o.Version,
                                     Remark = o.Remark,
                                     RequoteRefId = o.RequoteRefId,
                                     QuotationLoc8GUID = o.QuotationLoc8GUID,
                                     AcknowledgedBy = o.AcknowledgedBy,
                                     AcknowledgedAt = o.AcknowledgedAt,
                                     Id = o.Id,
                                     CustomerId = o.CustomerId
                                 },
                                 QuotationTitle = s1 == null ? "" : s1.Title,
                                 EstimateStatusStatus = s2 == null ? "" : s2.Status,
                                 WorkOrderSubject = s3 == null ? "" : s3.Subject,
                                 CustomerName = s4 == null ? "" : s4.Name
                             });


                var estimateListDtos = await query.ToListAsync();

                return _estimatesExcelExporter.ExportToFile(estimateListDtos);
            }
        }

        public void UpdateEstimatePrices(int estimateId)
        {
            var estimate = _estimateRepository.Get(estimateId);

            if (estimate != null)
            {
                var estimateDetailList = _estimateDetailRepository.GetAll()
                    .Where(e => e.EstimateId == estimate.Id && !e.IsDeleted);

                if (estimateDetailList != null && estimateDetailList.Count() > 0)
                {
                    decimal totalPrice = 0, totalTax = 0, totalDiscount = 0, totalCharge = 0;

                    foreach (var item in estimateDetailList)
                    {
                        decimal discountPrice = 0, taxPrice = 0, costPrice = 0;

                        costPrice = item.UnitPrice * item.Quantity;

                        if (item.MarkUp > 0)
                            costPrice += costPrice * (item.MarkUp / 100);

                        if (item.Discount > 0)
                            discountPrice = costPrice * (item.Discount / 100);

                        if (item.Tax > 0)
                            taxPrice = (costPrice - discountPrice) * (item.Tax / 100);

                        totalDiscount += discountPrice;
                        totalTax += taxPrice;
                        totalPrice += costPrice;
                        totalCharge += (costPrice - discountPrice) + taxPrice;
                    }

                    estimate.TotalPrice = totalPrice;
                    estimate.TotalTax = totalTax;
                    estimate.TotalDiscount = totalDiscount;
                    estimate.TotalCharge = totalCharge;
                }
                else
                {
                    estimate.TotalPrice = 0;
                    estimate.TotalTax = 0;
                    estimate.TotalDiscount = 0;
                    estimate.TotalCharge = 0;
                }

                _estimateRepository.Update(estimate);
            }
        }


        public async Task<PagedResultDto<EstimateWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");

                IQueryable<WorkOrder> query;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                    case "V":
                        query = _lookup_workOrderRepository
                            .GetAll()
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Subject.Contains(input.Filter));
                        break;

                    case "H":
                        query = _lookup_workOrderRepository
                            .GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Subject.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var totalCount = await query.CountAsync();

                var workOrderList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<EstimateWorkOrderLookupTableDto>();
                foreach (var workOrder in workOrderList)
                {
                    lookupTableDtoList.Add(new EstimateWorkOrderLookupTableDto
                    {
                        Id = workOrder.Id,
                        DisplayName = workOrder.Subject?.ToString()
                    });
                }

                return new PagedResultDto<EstimateWorkOrderLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        public async Task<PagedResultDto<EstimateQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => WorkOrder ID

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Quotation");

                IQueryable<Quotation> query;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                    case "V":
                        query = _lookup_quotationRepository
                            .GetAll()
                            .WhereIf(input.FilterId > 0, e => e.WorkOrderId == input.FilterId)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.Contains(input.Filter));
                        break;

                    case "H":
                        query = _lookup_quotationRepository
                            .GetAll()
                            .WhereIf(input.FilterId > 0, e => e.WorkOrderId == input.FilterId)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var totalCount = await query.CountAsync();

                var quotationList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<EstimateQuotationLookupTableDto>();
                foreach (var quotation in quotationList)
                {
                    lookupTableDtoList.Add(new EstimateQuotationLookupTableDto
                    {
                        Id = quotation.Id,
                        DisplayName = quotation.Title?.ToString()
                    });
                }

                return new PagedResultDto<EstimateQuotationLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        public async Task<PagedResultDto<EstimateEstimateStatusLookupTableDto>> GetAllEstimateStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_estimateStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var estimateStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EstimateEstimateStatusLookupTableDto>();
            foreach (var estimateStatus in estimateStatusList)
            {
                lookupTableDtoList.Add(new EstimateEstimateStatusLookupTableDto
                {
                    Id = estimateStatus.Id,
                    DisplayName = estimateStatus.Status?.ToString()
                });
            }

            return new PagedResultDto<EstimateEstimateStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<PagedResultDto<EstimateCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForEstimateLookupTableInput input)
        {
            IQueryable<Customer> query;
            var tenantInfo = await TenantManager.GetTenantInfo();
            int assetId = 0;

            if (input.QuotationId > 0)
            {
                assetId = _lookup_quotationRepository.Get(input.QuotationId)?.AssetId ?? 0;
            }
            else if (input.WorkOrderId > 0)
            {
                var assetOwnershipId = _lookup_workOrderRepository.Get(input.WorkOrderId)?.AssetOwnershipId ?? 0;

                if (assetOwnershipId > 0)
                    assetId = _lookup_assetOwnershipRepository.Get(assetOwnershipId)?.AssetId ?? 0;
            }

            if (assetId > 0)
            {
                query = _lookup_leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.LeaseAgreementFk.CustomerFk.Name.Contains(input.Filter))
                        .Select(e => e.LeaseAgreementFk.CustomerFk);
            }
            else
            {
                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                        query = _lookup_customerRepository
                            .GetAll()
                            .Where(e => e.Id == tenantInfo.Customer.Id);
                        break;

                    case "A":
                    case "H": // Get Everything
                        query = _lookup_customerRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }
            }

            var totalCount = await query.CountAsync();

            var customerList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<EstimateCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new EstimateCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name
                });
            }

            return new PagedResultDto<EstimateCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        public async Task<EstimateWorkOrderFkListDto> GetWorkOrderFkData(int workOrderId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Quotation");
                int assetId = 0;

                IQueryable<Quotation> quotationQuery;
                IQueryable<Customer> customerQuery = null;

                var assetOwnershipId = _lookup_workOrderRepository.Get(workOrderId)?.AssetOwnershipId ?? 0;
                if (assetOwnershipId > 0)
                    assetId = _lookup_assetOwnershipRepository.Get(assetOwnershipId)?.AssetId ?? 0;

                if (assetId > 0)
                {
                    customerQuery = _lookup_leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.LeaseAgreementFk.CustomerFk);
                }

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                        quotationQuery = _lookup_quotationRepository
                            .GetAll()
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .Where(w => w.WorkOrderId == workOrderId);
                        break;

                    case "V":
                        quotationQuery = null;
                        break;

                    case "H":
                        quotationQuery = _lookup_quotationRepository
                            .GetAll()
                            .Where(w => w.WorkOrderId == workOrderId);
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var quotationTableDtoList = new List<EstimateQuotationLookupTableDto>();
                var customerTableDtoList = new List<EstimateCustomerLookupTableDto>();

                if (quotationQuery?.Count() > 0)
                {
                    foreach (var quotation in quotationQuery)
                    {
                        quotationTableDtoList.Add(new EstimateQuotationLookupTableDto
                        {
                            Id = quotation.Id,
                            DisplayName = quotation.Title?.ToString()
                        });
                    }
                }

                if (customerQuery?.Count() > 0)
                {
                    foreach (var customer in customerQuery)
                    {
                        customerTableDtoList.Add(new EstimateCustomerLookupTableDto
                        {
                            Id = customer.Id,
                            DisplayName = customer.Name
                        });
                    }
                }

                return new EstimateWorkOrderFkListDto { QuotationList = quotationTableDtoList, CustomerList = customerTableDtoList };
            }
        }

        public async Task<EstimateQuotationFkListDto> GetQuotationFkData(int quotationId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                int assetId = _lookup_quotationRepository.Get(quotationId)?.AssetId ?? 0;

                IQueryable<Customer> customerQuery = null;

                if (assetId > 0)
                {
                    customerQuery = _lookup_leaseItemRepository // <------------- get the customer via the leaseItemRepository ----------------<
                        .GetAll()
                        .Include(e => e.LeaseAgreementFk)
                        .Where(e => e.AssetId == assetId)
                        .Select(e => e.LeaseAgreementFk.CustomerFk);
                }

                var customerTableDtoList = new List<EstimateCustomerLookupTableDto>();

                if (customerQuery?.Count() > 0)
                {
                    foreach (var customer in customerQuery)
                    {
                        customerTableDtoList.Add(new EstimateCustomerLookupTableDto
                        {
                            Id = customer.Id,
                            DisplayName = customer.Name
                        });
                    }
                }

                return new EstimateQuotationFkListDto { CustomerList = customerTableDtoList };
            }
        }

        public async Task<GetEstimateQuotationForViewDto> GetEstimateQuotationForView(int estimateId, PagedAndSortedResultRequestDto input)
        {
            var output = new GetEstimateQuotationForViewDto();

            if (estimateId > 0)
            {
                var estimate = await _estimateRepository.FirstOrDefaultAsync(estimateId);
                if (estimate != null && estimate.QuotationId > 0)
                {
                    var quotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)estimate.QuotationId);
                    var quoteOutput = new Quotations.Dtos.GetQuotationForViewDto { Quotation = ObjectMapper.Map<Quotations.Dtos.QuotationDto>(quotation) };

                    if (quoteOutput.Quotation != null && quoteOutput.Quotation.Id > 0)
                    {
                        if (quoteOutput.Quotation.SupportContractId > 0)
                        {
                            var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.SupportContractId);
                            quoteOutput.SupportContractTitle = _lookupSupportContract.Title.ToString();
                        }

                        if (quoteOutput.Quotation.QuotationStatusId != null)
                        {
                            var _lookupQuotationStatus = await _lookup_quotationStatusRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.QuotationStatusId);
                            quoteOutput.QuotationStatusStatus = _lookupQuotationStatus.Status.ToString();
                        }

                        if (quoteOutput.Quotation.WorkOrderId != null)
                        {
                            var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.WorkOrderId);
                            quoteOutput.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
                        }

                        if (quoteOutput.Quotation.AssetId != null)
                        {
                            var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.AssetId);
                            quoteOutput.AssetReference = _lookupAsset.Reference.ToString();
                        }

                        if (quoteOutput.Quotation.AssetClassId != null)
                        {
                            var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.AssetClassId);
                            quoteOutput.AssetClassClass = _lookupAssetClass.Class.ToString();
                        }

                        if (quoteOutput.Quotation.SupportTypeId != null)
                        {
                            var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.SupportTypeId);
                            quoteOutput.SupportTypeType = _lookupSupportType.Type.ToString();
                        }

                        if (quoteOutput.Quotation.SupportItemId != null)
                        {
                            var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)quoteOutput.Quotation.SupportItemId);
                            quoteOutput.SupportItemDescription = _lookupSupportItem.Description.ToString();
                        }


                        //Quotation Details Block
                        var filteredQuotationDetails = _quotationDetailRepository.GetAll()
                            .Include(e => e.ItemTypeFk)
                            .Include(e => e.QuotationFk)
                            .Include(e => e.UomFk)
                            .Where(e => e.QuotationId == quoteOutput.Quotation.Id);

                        var pagedAndFilteredQuotationDetails = filteredQuotationDetails
                            .OrderBy(input.Sorting ?? "id asc")
                            .PageBy(input);

                        var quotationDetails = from o in pagedAndFilteredQuotationDetails

                                               join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                                               from s3 in j3.DefaultIfEmpty()

                                               join o5 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o5.Id into j5
                                               from s5 in j5.DefaultIfEmpty()

                                               join o6 in _lookup_uomRepository.GetAll() on o.UomId equals o6.Id into j6
                                               from s6 in j6.DefaultIfEmpty()

                                               select new Quotations.Dtos.GetQuotationDetailForViewDto()
                                               {
                                                   QuotationDetail = new Quotations.Dtos.QuotationDetailDto
                                                   {
                                                       Description = o.Description,
                                                       Quantity = o.Quantity,
                                                       UnitPrice = o.UnitPrice,
                                                       Cost = o.Cost,
                                                       Tax = o.Tax,
                                                       Charge = o.Charge,
                                                       Discount = o.Discount,
                                                       MarkUp = o.MarkUp,
                                                       IsChargeable = o.IsChargeable,
                                                       IsAdHoc = o.IsAdHoc,
                                                       IsStandbyReplacementUnit = o.IsStandbyReplacementUnit,
                                                       IsOptionalItem = o.IsOptionalItem,
                                                       Remark = o.Remark,
                                                       Loc8GUID = o.Loc8GUID,
                                                       Id = o.Id
                                                   },
                                                   ItemTypeType = s3 == null ? "" : s3.Type.ToString(),
                                                   QuotationTitle = s5 == null ? "" : s5.Title.ToString(),
                                                   UomUnitOfMeasurement = s6 == null ? "" : s6.UnitOfMeasurement.ToString(),
                                               };

                        var totalCount = await filteredQuotationDetails.CountAsync();

                        var quoteDetailsOutput = new PagedResultDto<Quotations.Dtos.GetQuotationDetailForViewDto>(
                            totalCount,
                            await quotationDetails.ToListAsync()
                        );

                        output.Quotation = quoteOutput;
                        output.QuotationDetails = quoteDetailsOutput;
                    }
                }
            }

            return output;
        }

        public async Task<GetEstimateWorkOrderForViewDto> GetEstimateWorkOrderForView(int estimateId, PagedAndSortedResultRequestDto input)
        {
            var output = new GetEstimateWorkOrderForViewDto();

            if (estimateId > 0)
            {
                var estimate = await _estimateRepository.FirstOrDefaultAsync(estimateId);
                if (estimate != null && estimate.WorkOrderId > 0)
                {
                    var workOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)estimate.WorkOrderId);
                    var woOutput = new GetWorkOrderForViewDto { WorkOrder = ObjectMapper.Map<WorkOrderDto>(workOrder) };

                    if (woOutput.WorkOrder != null && woOutput.WorkOrder.Id > 0)
                    {
                        if (woOutput.WorkOrder.WorkOrderPriorityId > 0)
                        {
                            var _lookupWorkOrderPriority = await _lookup_workOrderPriorityRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.WorkOrderPriorityId);
                            woOutput.WorkOrderPriorityPriority = _lookupWorkOrderPriority.Priority.ToString();
                        }

                        if (woOutput.WorkOrder.WorkOrderTypeId > 0)
                        {
                            var _lookupWorkOrderType = await _lookup_workOrderTypeRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.WorkOrderTypeId);
                            woOutput.WorkOrderTypeType = _lookupWorkOrderType.Type.ToString();
                        }

                        if (woOutput.WorkOrder.VendorId > 0)
                        {
                            var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.VendorId);
                            woOutput.VendorName = _lookupVendor.Name.ToString();
                        }

                        if (woOutput.WorkOrder.IncidentId != null)
                        {
                            var _lookupIncident = await _lookup_incidentRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.IncidentId);
                            woOutput.IncidentDescription = _lookupIncident.Description.ToString();
                        }

                        if (woOutput.WorkOrder.SupportItemId != null)
                        {
                            var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.SupportItemId);
                            woOutput.SupportItemDescription = _lookupSupportItem.Description.ToString();
                        }

                        if (woOutput.WorkOrder.UserId > 0)
                        {
                            var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)woOutput.WorkOrder.UserId);
                            woOutput.UserName = _lookupUser?.Name.ToString();
                        }

                        if (woOutput.WorkOrder.CustomerId != null)
                        {
                            var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.CustomerId);
                            woOutput.CustomerName = _lookupCustomer.Name.ToString();
                        }

                        if (woOutput.WorkOrder.AssetOwnershipId != null)
                        {
                            var _lookupAssetOwnership = await _lookup_assetOwnershipRepository.GetAll().Include(a => a.AssetFk).Where(a => a.Id == (int)woOutput.WorkOrder.AssetOwnershipId).FirstOrDefaultAsync();
                            woOutput.AssetOwnershipAssetDisplayName = string.Format("{0} - {1}", _lookupAssetOwnership.AssetFk.Reference.ToString(), _lookupAssetOwnership.AssetFk.Description.ToString());
                            woOutput.AssetId = _lookupAssetOwnership.AssetFk.Id;
                        }

                        if (woOutput.WorkOrder.WorkOrderStatusId > 0)
                        {
                            var _lookupWorkOrderStatus = await _lookup_workOrderStatusRepository.FirstOrDefaultAsync((int)woOutput.WorkOrder.WorkOrderStatusId);
                            woOutput.WorkOrderStatusStatus = _lookupWorkOrderStatus.Status.ToString();
                        }


                        var filteredWorkOrderUpdates = _lookup_workOrderUpdateRepository.GetAll()
                            .Include(e => e.WorkOrderFk)
                            .Where(e => e.WorkOrderId == woOutput.WorkOrder.Id)
                            .Include(e => e.ItemTypeFk)
                            .Include(e => e.WorkOrderActionFk);

                        var pagedAndFilteredWorkOrderUpdates = filteredWorkOrderUpdates
                            .OrderBy(input.Sorting ?? "id asc")
                            .PageBy(input);

                        //Workorder updates block
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

                        var woUpdatesOutput = new PagedResultDto<GetWorkOrderUpdateForViewDto>(
                            totalCount,
                            await workOrderUpdates.ToListAsync()
                        );

                        output.WorkOrder = woOutput;
                        output.WorkOrderUpdates = woUpdatesOutput;
                    }
                }
            }

            return output;
        }


        public async Task<EstimatePdfDto> GetEstimatePDFInfo(int estimateId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                EstimatePdfDto ePdf = new EstimatePdfDto();

                ePdf.AuthenticationKey = await SettingManager.GetSettingValueAsync(Configuration.AppSettings.WebApiManagement.AuthorizationKey);
                ePdf.EstimateInfo = ObjectMapper.Map<EstimateDto>(await _estimateRepository.GetAsync(estimateId));

                if (ePdf.EstimateInfo != null)
                {
                    PdfEstimateDetailListDto eDetailInfo = new PdfEstimateDetailListDto();

                    var filteredEstimateDetails = _estimateDetailRepository.GetAll().Where(e => e.EstimateId == estimateId);

                    eDetailInfo.DetailList = (from o in filteredEstimateDetails

                                              join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                                              from s1 in j1.DefaultIfEmpty()

                                              join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                                              from s3 in j3.DefaultIfEmpty()

                                              join o4 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o4.Id into j4
                                              from s4 in j4.DefaultIfEmpty()

                                              select new PdfEstimateDetailForViewDto()
                                              {
                                                  EstimateDetail = new PdfEstimateDetailDto
                                                  {
                                                      Description = o.Description,
                                                      Quantity = o.Quantity,
                                                      UnitPrice = o.UnitPrice,
                                                      Cost = o.Cost,
                                                      Tax = o.Tax,
                                                      Charge = o.Charge,
                                                      Discount = o.Discount,
                                                      MarkUp = o.MarkUp,
                                                      Remark = o.Remark,
                                                      EstimateId = o.EstimateId,
                                                      ItemTypeId = o.ItemTypeId,
                                                      UomId = o.UomId,
                                                      WorkOrderActionId = o.WorkOrderActionId,
                                                      Id = o.Id
                                                  },
                                                  ItemTypeType = s1 == null ? "" : s1.Type,
                                                  UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement,
                                                  ActionWorkOrderAction = s4 == null ? "" : s4.Action
                                              }).ToList();


                    if (eDetailInfo.DetailList != null && eDetailInfo.DetailList.Count > 0)
                    {
                        decimal totalTax = 0;

                        foreach (var dItem in eDetailInfo.DetailList)
                        {
                            decimal markupPrice = 0;
                            decimal discountPrice = 0;

                            if (dItem.EstimateDetail.MarkUp > 0)
                                markupPrice = dItem.EstimateDetail.UnitPrice * (dItem.EstimateDetail.MarkUp / 100);

                            if (dItem.EstimateDetail.Discount > 0)
                                discountPrice = (dItem.EstimateDetail.UnitPrice + markupPrice) * (dItem.EstimateDetail.Discount / 100);

                            dItem.EstimateDetail.CalculatedUnitPrice = dItem.EstimateDetail.UnitPrice + markupPrice - discountPrice;
                            dItem.EstimateDetail.CalculatedAmount = (dItem.EstimateDetail.UnitPrice + markupPrice - discountPrice) * dItem.EstimateDetail.Quantity;

                            totalTax += dItem.EstimateDetail.CalculatedAmount * (dItem.EstimateDetail.Tax / 100);
                        }

                        eDetailInfo.TotalTax = totalTax;
                        eDetailInfo.TotalAmount = totalTax + eDetailInfo.DetailList.Sum(s => s.EstimateDetail.CalculatedAmount);
                        ePdf.EstimateDetailList = eDetailInfo;
                    }

                    if (ePdf.EstimateInfo.QuotationId > 0 || ePdf.EstimateInfo.WorkOrderId > 0)
                    {
                        int assetOwnerId = 0;

                        if (ePdf.EstimateInfo.QuotationId > 0)
                        {
                            var quotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)ePdf.EstimateInfo.QuotationId);
                            if (quotation != null)
                            {
                                var supportContractInfo = await _lookup_supportContractRepository.GetAsync(quotation.SupportContractId);
                                if (supportContractInfo != null)
                                    assetOwnerId = (int)supportContractInfo.AssetOwnerId;
                            }
                        }
                        else if (ePdf.EstimateInfo.WorkOrderId > 0)
                        {
                            var workOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)ePdf.EstimateInfo.WorkOrderId);
                            if (workOrder != null)
                            {
                                var _lookupAssetOwnership = await _lookup_assetOwnershipRepository.GetAll().Include(a => a.AssetOwnerFk).Where(a => a.Id == (int)workOrder.AssetOwnershipId).FirstOrDefaultAsync();
                                assetOwnerId = (_lookupAssetOwnership != null && _lookupAssetOwnership.AssetOwnerFk != null) ? _lookupAssetOwnership.AssetOwnerFk.Id : 0;
                            }
                        }

                        var aoAddress = _lookup_addressRepository
                            .GetAll()
                            .Where(e => e.AssetOwnerId == assetOwnerId)
                            .FirstOrDefault();

                        var aoCnt = _lookup_contactRepository
                            .GetAll()
                            .Where(e => e.AssetOwnerId == assetOwnerId)
                            .FirstOrDefault();

                        var custAddress = _lookup_addressRepository
                            .GetAll()
                            .Where(e => e.CustomerId == ePdf.EstimateInfo.CustomerId)
                            .FirstOrDefault();

                        var custCnt = _lookup_contactRepository
                            .GetAll()
                            .Where(e => e.CustomerId == ePdf.EstimateInfo.CustomerId)
                            .FirstOrDefault();

                        ePdf.AssetOwnerInfo = ObjectMapper.Map<Assets.Dtos.AssetOwnerDto>(await _lookup_assetOwnerRepository.GetAsync(assetOwnerId));
                        ePdf.AssetOwnerAddress = ObjectMapper.Map<Organizations.Dtos.AddressDto>(aoAddress);
                        ePdf.AssetOwnerContact = ObjectMapper.Map<Organizations.Dtos.ContactDto>(aoCnt);
                        ePdf.CustomerInfo = ObjectMapper.Map<Customers.Dtos.CustomerDto>(await _lookup_customerRepository.GetAsync(ePdf.EstimateInfo.CustomerId));
                        ePdf.CustomerAddress = ObjectMapper.Map<Organizations.Dtos.AddressDto>(custAddress);
                        ePdf.CustomerContact = ObjectMapper.Map<Organizations.Dtos.ContactDto>(custCnt);
                    }
                }

                return ePdf;
            }
        }
    }
}