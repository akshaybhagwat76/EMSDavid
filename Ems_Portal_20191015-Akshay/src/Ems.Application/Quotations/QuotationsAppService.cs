using Ems.Support;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Quotations.Exporting;
using Ems.Quotations.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Ems.Assets;
using Ems.Organizations;

namespace Ems.Quotations
{
    [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
    public class QuotationsAppService : EmsAppServiceBase, IQuotationsAppService
    {
        private readonly string _entityType = "Quotation";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Quotation> _quotationRepository;
        private readonly IRepository<QuotationDetail> _quotationDetailRepository;
        private readonly IQuotationsExcelExporter _quotationsExcelExporter;
        private readonly IRepository<SupportContract, int> _lookup_supportContractRepository;
        private readonly IRepository<QuotationStatus, int> _lookup_quotationStatusRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<SupportType, int> _lookup_supportTypeRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
        private readonly IRepository<Address> _addressRepository;

        public QuotationsAppService
            (
                IUnitOfWorkManager unitOfWorkManager, 
                IRepository<QuotationDetail> quotationDetailRepository, 
                IRepository<Quotation> quotationRepository, 
                IQuotationsExcelExporter quotationsExcelExporter, 
                IRepository<SupportContract, int> lookup_supportContractRepository, 
                IRepository<QuotationStatus, int> lookup_quotationStatusRepository, 
                IRepository<WorkOrder, int> lookup_workOrderRepository, 
                IRepository<Asset, int> lookup_assetRepository, 
                IRepository<AssetClass, int> lookup_assetClassRepository, 
                IRepository<SupportType, int> lookup_supportTypeRepository, 
                IRepository<SupportItem, int> lookup_supportItemRepository, 
                IRepository<AssetOwnership> assetOwnershipRepository, 
                IRepository<Address> addressRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _quotationRepository = quotationRepository;
            _quotationDetailRepository = quotationDetailRepository;
            _quotationsExcelExporter = quotationsExcelExporter;
            _lookup_supportContractRepository = lookup_supportContractRepository;
            _lookup_quotationStatusRepository = lookup_quotationStatusRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_supportTypeRepository = lookup_supportTypeRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _assetOwnershipRepository = assetOwnershipRepository;
            _addressRepository = addressRepository;
        }

        public async Task<PagedResultDto<GetQuotationForViewDto>> GetAll(GetAllQuotationsInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);
                var tenantType = tenantInfo.Tenant.TenantType;

                // NOTE: In this case, we need to apply cross-tenant-permissions AND also
                //       filter on the AssetOwner and Vendor.  If we don't, Vendor A will be able
                //       to see Vendor B's Quotations...
                // 
                //       We don't really need to apply cross-tenant-permissions check here because
                //       the "A" and "V" filters provide adequate filtering, but I've it included 
                //       it here to apply a standard pattern... caching should ensure the extra method call
                //       doesn't impact performance.
                //

                var filteredQuotations = _quotationRepository.GetAll()
                            .Include(e => e.SupportContractFk)
                            .Include(e => e.QuotationStatusFk)
                            .Include(e => e.WorkOrderFk)
                            .Include(e => e.AssetFk)
                            .Include(e => e.AssetClassFk)
                            .Include(e => e.SupportTypeFk)
                            .Include(e => e.SupportItemFk)

                            // Auth Filters
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                                                                                                                                         //.WhereIf(tenantType == "A", e => e.SupportContractFk.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any Quotations that are not relevant to the AssetOwener
                                                                                                                                                         //.WhereIf(tenantType == "V", e => e.SupportContractFk.VendorId == tenantInfo.Vendor.Id) // Filter out any Quotations that are not relevant to the Vendor

                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.QuotationLoc8GUID.Contains(input.Filter) || e.AcknowledgedBy.Contains(input.Filter))
                            .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                            .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                            .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                            .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                            .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                            .WhereIf(input.IsFinalFilter > -1, e => Convert.ToInt32(e.IsFinal) == input.IsFinalFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.RemarkFilter), e => e.Remark.ToLower() == input.RemarkFilter.ToLower().Trim())
                            .WhereIf(input.MinRequoteRefIdFilter != null, e => e.RequoteRefId >= input.MinRequoteRefIdFilter)
                            .WhereIf(input.MaxRequoteRefIdFilter != null, e => e.RequoteRefId <= input.MaxRequoteRefIdFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationLoc8GUIDFilter), e => e.QuotationLoc8GUID.ToLower() == input.QuotationLoc8GUIDFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.AcknowledgedByFilter), e => e.AcknowledgedBy.ToLower() == input.AcknowledgedByFilter.ToLower().Trim())
                            .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                            .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.SupportTypeTypeFilter), e => e.SupportTypeFk != null && e.SupportTypeFk.Type.ToLower() == input.SupportTypeTypeFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower() == input.SupportItemDescriptionFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationStatusStatusFilter), e => e.QuotationStatusFk != null && e.QuotationStatusFk.Status.ToLower() == input.QuotationStatusStatusFilter.ToLower().Trim())
                            .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim());

                var pagedAndFilteredQuotations = filteredQuotations
                    .OrderBy(input.Sorting ?? "id desc")
                    .PageBy(input);

                var quotations = from o in pagedAndFilteredQuotations
                                 join o1 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o1.Id into j1
                                 from s1 in j1.DefaultIfEmpty()

                                 join o2 in _lookup_quotationStatusRepository.GetAll() on o.QuotationStatusId equals o2.Id into j2
                                 from s2 in j2.DefaultIfEmpty()

                                 join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                                 from s3 in j3.DefaultIfEmpty()

                                 join o4 in _lookup_assetRepository.GetAll() on o.AssetId equals o4.Id into j4
                                 from s4 in j4.DefaultIfEmpty()

                                 join o5 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o5.Id into j5
                                 from s5 in j5.DefaultIfEmpty()

                                 join o6 in _lookup_supportTypeRepository.GetAll() on o.SupportTypeId equals o6.Id into j6
                                 from s6 in j6.DefaultIfEmpty()

                                 join o7 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o7.Id into j7
                                 from s7 in j7.DefaultIfEmpty()

                                 select new GetQuotationForViewDto()
                                 {
                                     Quotation = new QuotationDto
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
                                         IsFinal = o.IsFinal,
                                         Remark = o.Remark,
                                         RequoteRefId = o.RequoteRefId,
                                         QuotationLoc8GUID = o.QuotationLoc8GUID,
                                         AcknowledgedBy = o.AcknowledgedBy,
                                         AcknowledgedAt = o.AcknowledgedAt,
                                         Id = o.Id
                                     },
                                     SupportContractTitle = s1 == null ? "" : s1.Title.ToString(),
                                     QuotationStatusStatus = s2 == null ? "" : s2.Status.ToString(),
                                     WorkOrderSubject = s3 == null ? "" : s3.Subject.ToString(),
                                     AssetReference = s4 == null ? "" : s4.Reference.ToString(),
                                     AssetClassClass = s5 == null ? "" : s5.Class.ToString(),
                                     SupportTypeType = s6 == null ? "" : s6.Type.ToString(),
                                     SupportItemDescription = s7 == null ? "" : s7.Description.ToString()
                                 };

                var totalCount = await filteredQuotations.CountAsync();

                return new PagedResultDto<GetQuotationForViewDto>(
                    totalCount,
                    await quotations.ToListAsync()
                );
            }
        }

        public async Task<GetQuotationForViewDto> GetQuotationForView(int id)
        {
            var quotation = await _quotationRepository.GetAsync(id);

            var output = new GetQuotationForViewDto { Quotation = ObjectMapper.Map<QuotationDto>(quotation) };

            if (output.Quotation.SupportContractId > 0)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.Quotation.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.Quotation.QuotationStatusId != null)
            {
                var _lookupQuotationStatus = await _lookup_quotationStatusRepository.FirstOrDefaultAsync((int)output.Quotation.QuotationStatusId);
                output.QuotationStatusStatus = _lookupQuotationStatus.Status.ToString();
            }

            if (output.Quotation.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Quotation.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }

            if (output.Quotation.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Quotation.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

            if (output.Quotation.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Quotation.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.Quotation.SupportTypeId != null)
            {
                var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)output.Quotation.SupportTypeId);
                output.SupportTypeType = _lookupSupportType.Type.ToString();
            }

            if (output.Quotation.SupportItemId != null)
            {
                var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.Quotation.SupportItemId);
                output.SupportItemDescription = _lookupSupportItem.Description.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_Edit)]
        public async Task<GetQuotationForEditOutput> GetQuotationForEdit(EntityDto input)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetQuotationForEditOutput { Quotation = ObjectMapper.Map<CreateOrEditQuotationDto>(quotation) };

            if (output.Quotation.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.Quotation.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();

                DateTime curDate = DateTime.UtcNow;

                var assetOwnerId = _assetOwnershipRepository
                            .GetAll()
                            .Where(e => e.AssetId == output.Quotation.AssetId && (curDate >= e.StartDate && curDate <= e.FinishDate))
                            .Select(e => e.AssetOwnerId).FirstOrDefault();

                var addressData = _addressRepository
                        .GetAll()
                        .Where(e => e.AssetOwnerId == assetOwnerId)
                        .FirstOrDefault();

                if (addressData != null)
                {
                    output.AO_Address = new Organizations.Dtos.AddressDto();
                    output.AO_Address.AddressEntryName = addressData.AddressEntryName;
                    output.AO_Address.AddressLine1 = addressData.AddressLine1;
                    output.AO_Address.AddressLine2 = addressData.AddressLine2;
                    output.AO_Address.AddressLoc8GUID = addressData.AddressLoc8GUID;
                    output.AO_Address.AssetOwnerId = addressData.AssetOwnerId;
                    output.AO_Address.City = addressData.City;
                    output.AO_Address.Country = addressData.Country;
                    output.AO_Address.Id = addressData.Id;
                    output.AO_Address.PostalCode = addressData.PostalCode;
                    output.AO_Address.State = addressData.State;
                }
            }

            if (output.Quotation.AssetClassId != null)
            {
                var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.Quotation.AssetClassId);
                output.AssetClassClass = _lookupAssetClass.Class.ToString();
            }

            if (output.Quotation.SupportTypeId != null)
            {
                var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)output.Quotation.SupportTypeId);
                output.SupportTypeType = _lookupSupportType.Type.ToString();
            }

            if (output.Quotation.SupportItemId != null)
            {
                var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.Quotation.SupportItemId);
                output.SupportItemDescription = _lookupSupportItem.Description.ToString();
            }

            if (output.Quotation.WorkOrderId != null)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.Quotation.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            }


            if (output.Quotation.SupportContractId > 0)
            {
                var _lookupSupportContract = await _lookup_supportContractRepository.FirstOrDefaultAsync((int)output.Quotation.SupportContractId);
                output.SupportContractTitle = _lookupSupportContract.Title.ToString();
            }

            if (output.Quotation.QuotationStatusId != null)
            {
                var _lookupQuotationStatus = await _lookup_quotationStatusRepository.FirstOrDefaultAsync((int)output.Quotation.QuotationStatusId);
                output.QuotationStatusStatus = _lookupQuotationStatus.Status.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditQuotationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_Create)]
        protected virtual async Task Create(CreateOrEditQuotationDto input)
        {
            var quotation = ObjectMapper.Map<Quotation>(input);


            if (AbpSession.TenantId != null)
            {
                quotation.TenantId = (int?)AbpSession.TenantId;
            }


            await _quotationRepository.InsertAsync(quotation);
        }


        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_Create)]
        public async Task<GetQuotationForViewDto> CreateWithDetail(CreateQuotationWithDetailDto input)
        {
            decimal discountPrice = 0, taxPrice = 0;

            decimal markUp = input.MarkUp;
            decimal unitPrice = input.UnitPrice;
            decimal quantity = input.Quantity;
            decimal tax = input.Tax;
            decimal discount = input.Discount;
            decimal costPrice = unitPrice * quantity;

            if (markUp > 0)
                costPrice += costPrice * (markUp / 100);

            if (discount > 0)
                discountPrice = costPrice * (discount / 100);

            if (tax > 0)
                taxPrice = (costPrice - discountPrice) * (tax / 100);

            Quotation quotation = new Quotation()
            {
                AcknowledgedAt = null,
                AcknowledgedBy = null,
                CreationTime = DateTime.UtcNow,
                CreatorUserId = AbpSession.UserId,
                DeleterUserId = null,
                DeletionTime = null,
                Description = input.QuotationDescription,
                EndDate = input.EndDate,
                Id = 0,
                IsDeleted = false,
                IsFinal = false,
                LastModificationTime = null,
                LastModifierUserId = null,
                QuotationLoc8GUID = input.QuotationLoc8GUID,
                QuotationStatusId = input.QuotationStatusId,
                Reference = input.Reference,
                Remark = input.QuotationRemark,
                RequoteRefId = null,
                StartDate = input.StartDate,
                SupportContractId = input.SupportContractId,
                Title = input.Title,
                TotalCharge = (costPrice - discountPrice) + taxPrice,
                TotalDiscount = discountPrice,
                TotalPrice = costPrice,
                TotalTax = taxPrice,
                AssetClassId = input.AssetClassId,
                AssetId = input.AssetId,
                SupportItemId = input.SupportItemId,
                SupportTypeId = input.SupportTypeId,
                WorkOrderId = input.WorkOrderId
            };

            if (AbpSession.TenantId != null)
            {
                quotation.TenantId = (int?)AbpSession.TenantId;
            }

            var newQuotation = await _quotationRepository.InsertAsync(quotation);

            QuotationDetail quotationDetail = new QuotationDetail()
            {
                Charge = (costPrice - discountPrice) + taxPrice,
                Cost = costPrice,
                CreationTime = DateTime.UtcNow,
                CreatorUserId = AbpSession.UserId,
                DeletionTime = null,
                DeleterUserId = null,
                Description = input.DetailDescription,
                Discount = input.Discount,
                Id = 0,
                IsAdHoc = input.IsAdHoc,
                IsChargeable = input.IsChargeable,
                IsDeleted = false,
                IsOptionalItem = input.IsOptionalItem,
                IsStandbyReplacementUnit = input.IsStandbyReplacementUnit,
                ItemTypeId = input.ItemTypeId,
                LastModificationTime = null,
                LastModifierUserId = null,
                Loc8GUID = input.DetailLoc8GUID,
                MarkUp = input.MarkUp,
                Quantity = input.Quantity,
                QuotationId = newQuotation.Id,
                Remark = input.DetailRemark,
                Tax = input.Tax,
                UnitPrice = input.UnitPrice,
                UomId = input.UomId,
            };

            if (AbpSession.TenantId != null)
            {
                quotationDetail.TenantId = (int?)AbpSession.TenantId;
            }

            var newQuotationDetail = await _quotationDetailRepository.InsertAsync(quotationDetail);

            var output = new GetQuotationForViewDto { Quotation = ObjectMapper.Map<QuotationDto>(newQuotation) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_Edit)]
        protected virtual async Task Update(CreateOrEditQuotationDto input)
        {
            var quotation = await _quotationRepository.FirstOrDefaultAsync((int)input.Id);
            SupportContract supportCntrt = null;

            if (quotation != null)
                supportCntrt = await _lookup_supportContractRepository.GetAsync(quotation.SupportContractId);

            var tenantInfo = await TenantManager.GetTenantInfo();
            var tenantType = tenantInfo.Tenant.TenantType;
            bool okayToUpdate = false;

            switch (tenantType)
            {
                case "A":
                    okayToUpdate = (supportCntrt?.AssetOwnerId == tenantInfo.AssetOwner.Id) ? true : false;
                    break;
                case "V":
                    okayToUpdate = (supportCntrt?.VendorId == tenantInfo.Vendor.Id) ? true : false;
                    break;
                case "H":
                    okayToUpdate = true;
                    break;
                default:
                    okayToUpdate = false;
                    break;
            }

            if (okayToUpdate)
            {
                ObjectMapper.Map(input, quotation);
            }
            else
            {
                throw new Exception($"User is not entitled to update this data.  UserId: {AbpSession.UserId}.  WorkOrderId: {input.Id}");
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _quotationRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetQuotationsToExcel(GetAllQuotationsForExcelInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, _entityType);
            var tenantType = tenantInfo.Tenant.TenantType;

            // NOTE: In this case, we need to apply cross-tenant-permissions AND also
            //       filter on the AssetOwner and Vendor.  If we don't, Vendor A will be able
            //       to see Vendor B's Quotations...
            // 
            //       We don't really need to apply cross-tenant-permissions check here because
            //       the "A" and "V" filters provide adequate filtering, but I've it included 
            //       it here to apply a standard pattern... caching should ensure the extra method call
            //       doesn't impact performance.
            //

            var filteredQuotations = _quotationRepository.GetAll()
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.QuotationStatusFk)
                        .Include(e => e.WorkOrderFk)

                        // Auth Filters
                        .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                        .WhereIf(tenantType == "A", e => e.SupportContractFk.AssetOwnerId == tenantInfo.AssetOwner.Id)  // Filter out any Quotations that are not relevant to the AssetOwener
                        .WhereIf(tenantType == "V", e => e.SupportContractFk.VendorId == tenantInfo.Vendor.Id) // Filter out any Quotations that are not relevant to the Vendor

                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Reference.Contains(input.Filter) || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.QuotationLoc8GUID.Contains(input.Filter) || e.AcknowledgedBy.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter), e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title.ToLower() == input.TitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
                        .WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
                        .WhereIf(input.MinEndDateFilter != null, e => e.EndDate >= input.MinEndDateFilter)
                        .WhereIf(input.MaxEndDateFilter != null, e => e.EndDate <= input.MaxEndDateFilter)
                        .WhereIf(input.IsFinalFilter > -1, e => Convert.ToInt32(e.IsFinal) == input.IsFinalFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarkFilter), e => e.Remark.ToLower() == input.RemarkFilter.ToLower().Trim())
                        .WhereIf(input.MinRequoteRefIdFilter != null, e => e.RequoteRefId >= input.MinRequoteRefIdFilter)
                        .WhereIf(input.MaxRequoteRefIdFilter != null, e => e.RequoteRefId <= input.MaxRequoteRefIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationLoc8GUIDFilter), e => e.QuotationLoc8GUID.ToLower() == input.QuotationLoc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AcknowledgedByFilter), e => e.AcknowledgedBy.ToLower() == input.AcknowledgedByFilter.ToLower().Trim())
                        .WhereIf(input.MinAcknowledgedAtFilter != null, e => e.AcknowledgedAt >= input.MinAcknowledgedAtFilter)
                        .WhereIf(input.MaxAcknowledgedAtFilter != null, e => e.AcknowledgedAt <= input.MaxAcknowledgedAtFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.SupportContractTitleFilter), e => e.SupportContractFk != null && e.SupportContractFk.Title.ToLower() == input.SupportContractTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationStatusStatusFilter), e => e.QuotationStatusFk != null && e.QuotationStatusFk.Status.ToLower() == input.QuotationStatusStatusFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim());

            var query = (from o in filteredQuotations
                         join o1 in _lookup_supportContractRepository.GetAll() on o.SupportContractId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_quotationStatusRepository.GetAll() on o.QuotationStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         select new GetQuotationForViewDto()
                         {
                             Quotation = new QuotationDto
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
                                 IsFinal = o.IsFinal,
                                 Remark = o.Remark,
                                 RequoteRefId = o.RequoteRefId,
                                 QuotationLoc8GUID = o.QuotationLoc8GUID,
                                 AcknowledgedBy = o.AcknowledgedBy,
                                 AcknowledgedAt = o.AcknowledgedAt,
                                 Id = o.Id
                             },
                             SupportContractTitle = s1 == null ? "" : s1.Title.ToString(),
                             QuotationStatusStatus = s2 == null ? "" : s2.Status.ToString(),
                             WorkOrderSubject = s3 == null ? "" : s3.Subject.ToString()
                         });


            var quotationListDtos = await query.ToListAsync();

            return _quotationsExcelExporter.ExportToFile(quotationListDtos);
        }



        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Asset ID

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "SupportContract");

                IQueryable<SupportContract> query;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                        query = _lookup_supportItemRepository
                                .GetAll()
                                .Include(e => e.SupportContractFk)
                                .Where(e => e.AssetId == input.FilterId)
                                //.WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.SupportContractFk.Title.Contains(input.Filter))
                                .Select(e => e.SupportContractFk);
                        break;

                    case "V":
                        query = _lookup_supportContractRepository
                                .GetAll()
                                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.ToString().Contains(input.Filter));

                        break;

                    case "H":
                        query = _lookup_supportContractRepository
                                .GetAll()
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.ToString().Contains(input.Filter));

                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }


                var totalCount = await query.CountAsync();

                var supportContractList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<QuotationSupportContractLookupTableDto>();
                foreach (var supportContract in supportContractList)
                {
                    if (supportContract != null)
                    {
                        lookupTableDtoList.Add(new QuotationSupportContractLookupTableDto
                        {
                            Id = supportContract.Id,
                            DisplayName = supportContract.Title?.ToString()
                        });
                    }
                }

                return new PagedResultDto<QuotationSupportContractLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationQuotationStatusLookupTableDto>> GetAllQuotationStatusForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var query = _lookup_quotationStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.ToString().Contains(input.Filter)
               );

                var totalCount = await query.CountAsync();

                var quotationStatusList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<QuotationQuotationStatusLookupTableDto>();
                foreach (var quotationStatus in quotationStatusList)
                {
                    lookupTableDtoList.Add(new QuotationQuotationStatusLookupTableDto
                    {
                        Id = quotationStatus.Id,
                        DisplayName = quotationStatus.Status?.ToString()
                    });
                }

                return new PagedResultDto<QuotationQuotationStatusLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");

                var query = _lookup_workOrderRepository
                    .GetAll()
                    .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Subject.Contains(input.Filter)
                );

                var totalCount = await query.CountAsync();

                var workOrderList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<QuotationWorkOrderLookupTableDto>();
                foreach (var workOrder in workOrderList)
                {
                    lookupTableDtoList.Add(new QuotationWorkOrderLookupTableDto
                    {
                        Id = workOrder.Id,
                        DisplayName = workOrder.Subject?.ToString()
                    });
                }

                return new PagedResultDto<QuotationWorkOrderLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationAssetLookupTableDto>> GetAllAssetForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                //input.FilterId => WorkOrder ID

                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Asset");

                IQueryable<Asset> query;
                int assetId = 0;

                if (input.FilterId > 0) // has WorkOrder ID
                {
                    assetId = _lookup_workOrderRepository
                        .GetAll()
                        .Include(e => e.IncidentFk)
                        .Where(e => e.Id == input.FilterId)
                        .Select(e => e.IncidentFk.AssetId).FirstOrDefault() ?? 0;
                }


                switch (tenantInfo.Tenant.TenantType)
                {
                    case "A":
                        if (input.FilterId > 0) // has WorkOrder ID
                        {
                            query = _lookup_assetRepository
                                .GetAll()
                                .Where(e => e.Id == assetId)
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Reference.Contains(input.Filter));
                        }
                        else
                        {
                            var myAssetIds = _assetOwnershipRepository.GetAll()
                                .Where(e => e.AssetOwnerId == tenantInfo.AssetOwner.Id)
                                .Select(e => e.AssetId)
                                .ToList();

                            query = _lookup_assetRepository
                                .GetAll()
                                .Where(e => myAssetIds.Contains(e.Id)) // Get all my Assets
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Reference.Contains(input.Filter));
                        }
                        break;

                    case "V":

                        if (input.FilterId > 0) // has WorkOrder ID
                        {
                            query = _lookup_assetRepository
                                .GetAll()
                                .Where(e => e.Id == assetId)
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Reference.Contains(input.Filter));
                        }
                        else
                        {
                            query = _lookup_supportItemRepository
                                .GetAll()
                                .Include(e => e.AssetFk)
                                .Include(e => e.SupportContractFk)
                                .Where(e => e.SupportContractFk.VendorId == tenantInfo.Vendor.Id)
                                .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetFk.Reference.Contains(input.Filter))
                                .Select(s => s.AssetFk);
                        }
                        break;

                    case "H": // Get Everything
                        query = _lookup_assetRepository
                            .GetAll()
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var totalCount = await query.CountAsync();

                var assetList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<QuotationAssetLookupTableDto>();
                foreach (var asset in assetList)
                {
                    lookupTableDtoList.Add(new QuotationAssetLookupTableDto
                    {
                        Id = asset.Id,
                        DisplayName = asset.Reference?.ToString()
                    });
                }

                return new PagedResultDto<QuotationAssetLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            var query = _lookup_assetRepository
                    .GetAll()
                    .Include(e => e.AssetClassFk)
                    .Where(e => e.Id == input.FilterId)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AssetClassFk.Class.Contains(input.Filter))
                    .Select(e => e.AssetClassFk);

            var totalCount = await query.CountAsync();

            var assetClassList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<QuotationAssetClassLookupTableDto>();
            foreach (var assetClass in assetClassList)
            {
                lookupTableDtoList.Add(new QuotationAssetClassLookupTableDto
                {
                    Id = assetClass.Id,
                    DisplayName = assetClass.Class?.ToString()
                });
            }

            return new PagedResultDto<QuotationAssetClassLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationSupportTypeLookupTableDto>> GetAllSupportTypeForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            var query = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.SupportTypeFk)
                        .Where(e => e.AssetId == input.FilterId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.SupportTypeFk.Type.Contains(input.Filter))
                        .Select(e => e.SupportTypeFk);

            var totalCount = await query.CountAsync();

            var supportTypeList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<QuotationSupportTypeLookupTableDto>();
            foreach (var supportType in supportTypeList)
            {
                lookupTableDtoList.Add(new QuotationSupportTypeLookupTableDto
                {
                    Id = supportType.Id,
                    DisplayName = supportType.Type?.ToString()
                });
            }

            return new PagedResultDto<QuotationSupportTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => Support Contract ID

            var query = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Description.Contains(input.Filter))
                        .Where(e => e.SupportContractId == input.FilterId);

            var totalCount = await query.CountAsync();

            var supportItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<QuotationSupportItemLookupTableDto>();
            foreach (var supportItem in supportItemList)
            {
                lookupTableDtoList.Add(new QuotationSupportItemLookupTableDto
                {
                    Id = supportItem.Id,
                    DisplayName = supportItem.Description?.ToString()
                });
            }

            return new PagedResultDto<QuotationSupportItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<QuotationAssetAndSupportItemListDto> GetQuotationAssetAndSupportItemList(int workOrderId, int assetId)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            Asset assetInfo;

            if (workOrderId > 0)
            {
                assetInfo = _lookup_workOrderRepository
                    .GetAll()
                    .Include(e => e.AssetOwnershipFk)
                    .Where(e => e.Id == workOrderId)
                    .Select(e => e.AssetOwnershipFk.AssetFk).FirstOrDefault();
            }
            else
            {
                assetInfo = _lookup_assetRepository.Get(assetId);
            }

            DateTime curDate = DateTime.UtcNow;

            var assetOwnerId = _assetOwnershipRepository
                        .GetAll()
                        .Where(e => e.AssetId == assetInfo.Id && (curDate >= e.StartDate && curDate <= e.FinishDate))
                        .Select(e => e.AssetOwnerId).FirstOrDefault();

            IQueryable<SupportContract> supportContractQuery;
            IQueryable<SupportItem> supportItemQuery;
            IQueryable<Asset> assetQuery = null;
            IQueryable<AssetClass> assetClassQuery;
            IQueryable<SupportType> supportTypeQuery;
            Address addressData;

            switch (tenantInfo.Tenant.TenantType)
            {
                case "H":
                case "A":
                case "C":
                case "V":

                    if (workOrderId > 0)
                    {
                        assetQuery = _lookup_assetRepository
                            .GetAll()
                            .Where(e => e.Id == assetInfo.Id);
                    }

                    assetClassQuery = _lookup_assetClassRepository
                        .GetAll()
                        .Where(e => e.Id == assetInfo.AssetClassId);

                    supportContractQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Where(e => e.AssetId == assetInfo.Id)
                        .Select(e => e.SupportContractFk);

                    supportItemQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Where(e => e.AssetId == assetInfo.Id);

                    supportTypeQuery = _lookup_supportItemRepository
                        .GetAll()
                        .Include(e => e.SupportContractFk)
                        .Include(e => e.SupportTypeFk)
                        .Where(e => e.AssetId == assetInfo.Id)
                        .Select(e => e.SupportTypeFk);

                    addressData = _addressRepository
                        .GetAll()
                        .Where(e => e.AssetOwnerId == assetOwnerId)
                        .FirstOrDefault();

                    break;

                //case "H": // Get Everything
                //    if (workOrderId > 0)
                //    {
                //        assetQuery = _lookup_assetRepository
                //        .GetAll();
                //    }

                //    assetClassQuery = _lookup_assetClassRepository
                //        .GetAll();

                //    supportContractQuery = _lookup_supportContractRepository
                //        .GetAll();

                //    supportItemQuery = _lookup_supportItemRepository
                //        .GetAll();

                //    supportTypeQuery = _lookup_supportTypeRepository
                //        .GetAll();

                //    addressData = _addressRepository
                //        .GetAll()
                //        .Where(e => e.AssetOwnerId == assetOwnerId)
                //        .FirstOrDefault();

                //    break;

                default:
                    throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
            }

            var assetTableDtoList = new List<QuotationAssetLookupTableDto>();
            var assetClassTableDtoList = new List<QuotationAssetClassLookupTableDto>();
            var supportContractTableDtoList = new List<QuotationSupportContractLookupTableDto>();
            var supportItemTableDtoList = new List<QuotationSupportItemLookupTableDto>();
            var supportTypeTableDtoList = new List<QuotationSupportTypeLookupTableDto>();
            var addressTableDtoList = new Organizations.Dtos.AddressDto();

            if (assetQuery?.Count() > 0)
            {
                foreach (var asset in assetQuery)
                {
                    if (asset != null)
                    {
                        assetTableDtoList.Add(new QuotationAssetLookupTableDto
                        {
                            Id = asset.Id,
                            DisplayName = asset.Reference
                        });
                    }
                }
            }

            if (assetClassQuery?.Count() > 0)
            {
                foreach (var assetCls in assetClassQuery)
                {
                    if (assetCls != null)
                    {
                        assetClassTableDtoList.Add(new QuotationAssetClassLookupTableDto
                        {
                            Id = assetCls.Id,
                            DisplayName = assetCls.Class
                        });
                    }
                }
            }

            if (supportContractQuery?.Count() > 0)
            {
                foreach (var supportContract in supportContractQuery)
                {
                    if (supportContract != null)
                    {
                        supportContractTableDtoList.Add(new QuotationSupportContractLookupTableDto
                        {
                            Id = supportContract.Id,
                            DisplayName = supportContract.Title
                        });
                    }
                }
            }

            if (supportItemQuery?.Count() > 0)
            {
                foreach (var supportItem in supportItemQuery)
                {
                    if (supportItem != null)
                    {
                        supportItemTableDtoList.Add(new QuotationSupportItemLookupTableDto
                        {
                            Id = supportItem.Id,
                            DisplayName = supportItem.Description
                        });
                    }
                }
            }

            if (supportTypeQuery?.Count() > 0)
            {
                foreach (var supportType in supportTypeQuery)
                {
                    if (supportType != null)
                    {
                        supportTypeTableDtoList.Add(new QuotationSupportTypeLookupTableDto
                        {
                            Id = supportType.Id,
                            DisplayName = supportType.Type
                        });
                    }
                }
            }

            if (addressData != null)
            {
                addressTableDtoList.AddressEntryName = addressData.AddressEntryName;
                addressTableDtoList.AddressLine1 = addressData.AddressLine1;
                addressTableDtoList.AddressLine2 = addressData.AddressLine2;
                addressTableDtoList.AddressLoc8GUID = addressData.AddressLoc8GUID;
                addressTableDtoList.AssetOwnerId = addressData.AssetOwnerId;
                addressTableDtoList.City = addressData.City;
                addressTableDtoList.Country = addressData.Country;
                addressTableDtoList.Id = addressData.Id;
                addressTableDtoList.PostalCode = addressData.PostalCode;
                addressTableDtoList.State = addressData.State;
            }

            return new QuotationAssetAndSupportItemListDto { AssetList = assetTableDtoList, SupportContractList = supportContractTableDtoList, SupportItemList = supportItemTableDtoList, AssetClassList = assetClassTableDtoList, SupportTypeList = supportTypeTableDtoList, AO_Address = addressTableDtoList };
        }
    }
}