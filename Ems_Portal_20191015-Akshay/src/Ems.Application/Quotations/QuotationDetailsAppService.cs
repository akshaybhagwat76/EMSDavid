using Ems.Assets;
using Ems.Support;
using Ems.Metrics;
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
using Ems.Organizations;
using Ems.Vendors;

namespace Ems.Quotations
{
    [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
    public class QuotationDetailsAppService : EmsAppServiceBase, IQuotationDetailsAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<LeaseItem> _leaseItemRepository;
        private readonly IRepository<QuotationDetail> _quotationDetailRepository;
        private readonly IQuotationDetailsExcelExporter _quotationDetailsExcelExporter;
        private readonly IRepository<Asset, int> _lookup_assetRepository;
        private readonly IRepository<AssetClass, int> _lookup_assetClassRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<SupportType, int> _lookup_supportTypeRepository;
        private readonly IRepository<Quotation, int> _lookup_quotationRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<Asset> _assetRepository;
        private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<SupportContract> _supportContractRepository;
        private readonly IRepository<AssetOwner> _assetOwnerRepository;
        private readonly IRepository<Vendor> _vendorRepository;
        private readonly IRepository<Contact> _contactRepository;


        public QuotationDetailsAppService(IUnitOfWorkManager unitOfWorkManager, IRepository<LeaseItem> leaseItemtRepository, IRepository<QuotationDetail> quotationDetailRepository, IQuotationDetailsExcelExporter quotationDetailsExcelExporter, IRepository<Asset, int> lookup_assetRepository, IRepository<AssetClass, int> lookup_assetClassRepository, IRepository<ItemType, int> lookup_itemTypeRepository, IRepository<SupportType, int> lookup_supportTypeRepository, IRepository<Quotation, int> lookup_quotationRepository, IRepository<Uom, int> lookup_uomRepository, IRepository<SupportItem, int> lookup_supportItemRepository, IRepository<WorkOrder, int> lookup_workOrderRepository, IRepository<Asset> assetRepository, IRepository<AssetOwnership> assetOwnershipRepository, IRepository<Address> addressRepository, IRepository<SupportContract> supportContractRepository, IRepository<AssetOwner> assetOwnerRepository, IRepository<Vendor> vendorRepository, IRepository<Contact> contactRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _leaseItemRepository = leaseItemtRepository;
            _quotationDetailRepository = quotationDetailRepository;
            _quotationDetailsExcelExporter = quotationDetailsExcelExporter;
            _lookup_assetRepository = lookup_assetRepository;
            _lookup_assetClassRepository = lookup_assetClassRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_supportTypeRepository = lookup_supportTypeRepository;
            _lookup_quotationRepository = lookup_quotationRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _assetRepository = assetRepository;
            _assetOwnershipRepository = assetOwnershipRepository;
            _addressRepository = addressRepository;
            _supportContractRepository = supportContractRepository;
            _assetOwnerRepository = assetOwnerRepository;
            _vendorRepository = vendorRepository;
            _contactRepository = contactRepository;
        }

        public async Task<PagedResultDto<GetQuotationDetailForViewDto>> GetAll(GetAllQuotationDetailsInput input)
        {
            var filteredQuotationDetails = _quotationDetailRepository.GetAll()
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.QuotationFk)
                        .Include(e => e.UomFk)
                        .WhereIf(input.QuotationId > 0, e => e.QuotationId == input.QuotationId)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.Loc8GUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Loc8GUIDFilter), e => e.Loc8GUID.ToLower() == input.Loc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type.ToLower() == input.ItemTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.ToLower() == input.QuotationTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim());

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

                                   select new GetQuotationDetailForViewDto()
                                   {
                                       QuotationDetail = new QuotationDetailDto
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
                                           QuotationId = o.QuotationId,
                                           Id = o.Id
                                       },
                                       ItemTypeType = s3 == null ? "" : s3.Type.ToString(),
                                       QuotationTitle = s5 == null ? "" : s5.Title.ToString(),
                                       UomUnitOfMeasurement = s6 == null ? "" : s6.UnitOfMeasurement.ToString(),
                                   };

            var totalCount = await filteredQuotationDetails.CountAsync();

            return new PagedResultDto<GetQuotationDetailForViewDto>(
                totalCount,
                await quotationDetails.ToListAsync()
            );
        }

        public async Task<GetQuotationDetailForViewDto> GetQuotationDetailForView(int id)
        {
            var quotationDetail = await _quotationDetailRepository.GetAsync(id);
            var output = new GetQuotationDetailForViewDto { QuotationDetail = ObjectMapper.Map<QuotationDetailDto>(quotationDetail) };

            if (output.QuotationDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.QuotationDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.QuotationDetail != null)
            {
                var _lookupQuotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.QuotationDetail.QuotationId);
                output.QuotationTitle = _lookupQuotation.Title.ToString();
            }

            if (output.QuotationDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.QuotationDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }


            //if (output.QuotationDetail.AssetId != null)
            //{
            //    var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.QuotationDetail.AssetId);
            //    output.AssetReference = _lookupAsset.Reference.ToString();
            //}

            //if (output.QuotationDetail.AssetClassId != null)
            //{
            //    var _lookupAssetClass = await _lookup_assetClassRepository.FirstOrDefaultAsync((int)output.QuotationDetail.AssetClassId);
            //    output.AssetClassClass = _lookupAssetClass.Class.ToString();
            //}

            //if (output.QuotationDetail.SupportTypeId != null)
            //{
            //    var _lookupSupportType = await _lookup_supportTypeRepository.FirstOrDefaultAsync((int)output.QuotationDetail.SupportTypeId);
            //    output.SupportTypeType = _lookupSupportType.Type.ToString();
            //}

            //if (output.QuotationDetail.SupportItemId != null)
            //{
            //    var _lookupSupportItem = await _lookup_supportItemRepository.FirstOrDefaultAsync((int)output.QuotationDetail.SupportItemId);
            //    output.SupportItemDescription = _lookupSupportItem.Description.ToString();
            //}

            //if (output.QuotationDetail.WorkOrderId != null)
            //{
            //    var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.QuotationDetail.WorkOrderId);
            //    output.WorkOrderSubject = _lookupWorkOrder.Subject.ToString();
            //}

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_QuotationDetailsEdit)]
        public async Task<GetQuotationDetailForEditOutput> GetQuotationDetailForEdit(EntityDto input)
        {
            var quotationDetail = await _quotationDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetQuotationDetailForEditOutput { QuotationDetail = ObjectMapper.Map<CreateOrEditQuotationDetailDto>(quotationDetail) };

            if (output.QuotationDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.QuotationDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.QuotationDetail != null && output.QuotationDetail.QuotationId > 0)
            {
                var _lookupQuotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)output.QuotationDetail.QuotationId);
                output.QuotationTitle = _lookupQuotation.Title.ToString();
            }

            if (output.QuotationDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.QuotationDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditQuotationDetailDto input)
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

            input.Cost = costPrice;
            input.Charge = (costPrice - discountPrice) + taxPrice;

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_QuotationDetailsCreate)]
        protected virtual async Task Create(CreateOrEditQuotationDetailDto input)
        {
            var quotationDetail = ObjectMapper.Map<QuotationDetail>(input);


            if (AbpSession.TenantId != null)
            {
                quotationDetail.TenantId = (int?)AbpSession.TenantId;
            }


            await _quotationDetailRepository.InsertAsync(quotationDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_QuotationDetailsEdit)]
        protected virtual async Task Update(CreateOrEditQuotationDetailDto input)
        {
            var quotationDetail = await _quotationDetailRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, quotationDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations_QuotationDetailsDelete)]
        public async Task Delete(EntityDto input)
        {
            await _quotationDetailRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetQuotationDetailsToExcel(GetAllQuotationDetailsForExcelInput input)
        {
            var filteredQuotationDetails = _quotationDetailRepository.GetAll()
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.QuotationFk)
                        .Include(e => e.UomFk)
                        //.Include(e => e.AssetFk)
                        //.Include(e => e.AssetClassFk)
                        //.Include(e => e.SupportTypeFk)
                        //.Include(e => e.SupportItemFk)
                        //.Include(e => e.WorkOrderFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter) || e.Remark.Contains(input.Filter) || e.Loc8GUID.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Loc8GUIDFilter), e => e.Loc8GUID.ToLower() == input.Loc8GUIDFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ItemTypeTypeFilter), e => e.ItemTypeFk != null && e.ItemTypeFk.Type.ToLower() == input.ItemTypeTypeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.QuotationTitleFilter), e => e.QuotationFk != null && e.QuotationFk.Title.ToLower() == input.QuotationTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UomUnitOfMeasurementFilter), e => e.UomFk != null && e.UomFk.UnitOfMeasurement.ToLower() == input.UomUnitOfMeasurementFilter.ToLower().Trim());
            //.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
            //.WhereIf(!string.IsNullOrWhiteSpace(input.AssetClassClassFilter), e => e.AssetClassFk != null && e.AssetClassFk.Class.ToLower() == input.AssetClassClassFilter.ToLower().Trim())
            //.WhereIf(!string.IsNullOrWhiteSpace(input.SupportTypeTypeFilter), e => e.SupportTypeFk != null && e.SupportTypeFk.Type.ToLower() == input.SupportTypeTypeFilter.ToLower().Trim())
            //.WhereIf(!string.IsNullOrWhiteSpace(input.SupportItemDescriptionFilter), e => e.SupportItemFk != null && e.SupportItemFk.Description.ToLower() == input.SupportItemDescriptionFilter.ToLower().Trim())
            //.WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim());

            var query = (from o in filteredQuotationDetails

                         join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o5 in _lookup_quotationRepository.GetAll() on o.QuotationId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         join o6 in _lookup_uomRepository.GetAll() on o.UomId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()

                             //join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                             //from s1 in j1.DefaultIfEmpty()

                             //join o2 in _lookup_assetClassRepository.GetAll() on o.AssetClassId equals o2.Id into j2
                             //from s2 in j2.DefaultIfEmpty()


                             //join o4 in _lookup_supportTypeRepository.GetAll() on o.SupportTypeId equals o4.Id into j4
                             //from s4 in j4.DefaultIfEmpty()

                             //join o7 in _lookup_supportItemRepository.GetAll() on o.SupportItemId equals o7.Id into j7
                             //from s7 in j7.DefaultIfEmpty()

                             //join o8 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o8.Id into j8
                             //from s8 in j8.DefaultIfEmpty()

                         select new GetQuotationDetailForViewDto()
                         {
                             QuotationDetail = new QuotationDetailDto
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
                             UomUnitOfMeasurement = s6 == null ? "" : s6.UnitOfMeasurement.ToString()
                             //AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                             //AssetClassClass = s2 == null ? "" : s2.Class.ToString(),
                             //SupportTypeType = s4 == null ? "" : s4.Type.ToString(),
                             //SupportItemDescription = s7 == null ? "" : s7.Description.ToString(),
                             //WorkOrderSubject = s8 == null ? "" : s8.Subject.ToString()
                         });


            var quotationDetailListDtos = await query.ToListAsync();

            return _quotationDetailsExcelExporter.ExportToFile(quotationDetailListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationDetailQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_quotationRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Title.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var quotationList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<QuotationDetailQuotationLookupTableDto>();
            foreach (var quotation in quotationList)
            {
                lookupTableDtoList.Add(new QuotationDetailQuotationLookupTableDto
                {
                    Id = quotation.Id,
                    DisplayName = quotation.Title?.ToString()
                });
            }

            return new PagedResultDto<QuotationDetailQuotationLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<PagedResultDto<QuotationDetailWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
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

                var lookupTableDtoList = new List<QuotationDetailWorkOrderLookupTableDto>();
                foreach (var workOrder in workOrderList)
                {
                    lookupTableDtoList.Add(new QuotationDetailWorkOrderLookupTableDto
                    {
                        Id = workOrder.Id,
                        DisplayName = workOrder.Subject?.ToString()
                    });
                }

                return new PagedResultDto<QuotationDetailWorkOrderLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }


        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public void UpdateQuotationPrices(int quotationId)
        {
            var quotation = _lookup_quotationRepository.Get(quotationId);

            if (quotation != null)
            {
                var filteredQuotationDetails = _quotationDetailRepository.GetAll()
                            .Where(e => e.QuotationId == quotationId && !e.IsDeleted);

                if (filteredQuotationDetails != null && filteredQuotationDetails.Count() > 0)
                {
                    decimal totalPrice = 0, totalTax = 0, totalDiscount = 0, totalCharge = 0;

                    foreach (var item in filteredQuotationDetails)
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

                    quotation.TotalPrice = totalPrice;
                    quotation.TotalTax = totalTax;
                    quotation.TotalDiscount = totalDiscount;
                    quotation.TotalCharge = totalCharge;
                }
                else
                {
                    quotation.TotalPrice = 0;
                    quotation.TotalTax = 0;
                    quotation.TotalDiscount = 0;
                    quotation.TotalCharge = 0;
                }

                _lookup_quotationRepository.Update(quotation);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Main_Quotations)]
        public async Task<QuotationPdfDto> GetQuotationPDFInfo(int quotationId)
        {
            QuotationPdfDto qPdf = new QuotationPdfDto();

            qPdf.AuthenticationKey = await SettingManager.GetSettingValueAsync(Configuration.AppSettings.WebApiManagement.AuthorizationKey);
            qPdf.QuotationInfo = ObjectMapper.Map<QuotationDto>(await _lookup_quotationRepository.GetAsync(quotationId));

            if (qPdf.QuotationInfo != null && qPdf.QuotationInfo.SupportContractId > 0)
            {
                PdfQuotationDetailListDto qDetailInfo = new PdfQuotationDetailListDto();

                var filteredQuotationDetails = _quotationDetailRepository.GetAll().Where(e => e.QuotationId == quotationId);

                qDetailInfo.DetailList = (from o in filteredQuotationDetails

                                          join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                                          from s1 in j1.DefaultIfEmpty()

                                          join o2 in _lookup_uomRepository.GetAll() on o.UomId equals o2.Id into j2
                                          from s2 in j2.DefaultIfEmpty()

                                          select new PdfQuotationDetailForViewDto()
                                          {
                                              QuotationDetail = new PdfQuotationDetailDto
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
                                                  QuotationId = o.QuotationId,
                                                  Id = o.Id
                                              },
                                              ItemTypeType = s1 == null ? "" : s1.Type.ToString(),
                                              UomUnitOfMeasurement = s2 == null ? "" : s2.UnitOfMeasurement.ToString()
                                          }).ToList();


                if (qDetailInfo.DetailList != null && qDetailInfo.DetailList.Count > 0)
                {
                    decimal totalTax = 0;

                    foreach (var dItem in qDetailInfo.DetailList)
                    {
                        decimal markupPrice = 0;
                        decimal discountPrice = 0;

                        if (dItem.QuotationDetail.MarkUp > 0)
                            markupPrice = dItem.QuotationDetail.UnitPrice * (dItem.QuotationDetail.MarkUp / 100);

                        if (dItem.QuotationDetail.Discount > 0)
                            discountPrice = (dItem.QuotationDetail.UnitPrice + markupPrice) * (dItem.QuotationDetail.Discount / 100);

                        dItem.QuotationDetail.CalculatedUnitPrice = dItem.QuotationDetail.UnitPrice + markupPrice - discountPrice;
                        dItem.QuotationDetail.CalculatedAmount = (dItem.QuotationDetail.UnitPrice + markupPrice - discountPrice) * dItem.QuotationDetail.Quantity;

                        totalTax += dItem.QuotationDetail.CalculatedAmount * (dItem.QuotationDetail.Tax / 100);
                    }

                    qDetailInfo.TotalTax = totalTax;
                    qDetailInfo.TotalAmount = totalTax + qDetailInfo.DetailList.Sum(s => s.QuotationDetail.CalculatedAmount);
                    qPdf.QuotationDetailList = qDetailInfo;
                }

                var supportContractInfo = await _supportContractRepository.GetAsync(qPdf.QuotationInfo.SupportContractId);
                if (supportContractInfo != null)
                {
                    var aoAddress = _addressRepository
                        .GetAll()
                        .Where(e => e.AssetOwnerId == supportContractInfo.AssetOwnerId)
                        .FirstOrDefault();

                    var aoCnt = _contactRepository
                        .GetAll()
                        .Where(e => e.AssetOwnerId == supportContractInfo.AssetOwnerId)
                        .FirstOrDefault();

                    var vendorAddress = _addressRepository
                        .GetAll()
                        .Where(e => e.VendorId == supportContractInfo.VendorId)
                        .FirstOrDefault();

                    var vendorCnt = _contactRepository
                        .GetAll()
                        .Where(e => e.VendorId == supportContractInfo.VendorId)
                        .FirstOrDefault();

                    qPdf.AssetOwnerInfo = ObjectMapper.Map<Assets.Dtos.AssetOwnerDto>(await _assetOwnerRepository.GetAsync((int)supportContractInfo.AssetOwnerId));
                    qPdf.AssetOwnerAddress = ObjectMapper.Map<Organizations.Dtos.AddressDto>(aoAddress);
                    qPdf.AssetOwnerContact = ObjectMapper.Map<Organizations.Dtos.ContactDto>(aoCnt);
                    qPdf.VendorInfo = ObjectMapper.Map<Vendors.Dtos.VendorDto>(await _vendorRepository.GetAsync((int)supportContractInfo.VendorId));
                    qPdf.VendorAddress = ObjectMapper.Map<Organizations.Dtos.AddressDto>(vendorAddress);
                    qPdf.VendorContact = ObjectMapper.Map<Organizations.Dtos.ContactDto>(vendorCnt);
                }
            }

            return qPdf;
        }
    }
}