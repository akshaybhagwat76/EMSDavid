using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Billing.Dtos;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Metrics;
using Ems.Quotations;
using Ems.Support;
using Ems.Assets;
using System;

namespace Ems.Billing
{
    [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
    public class CustomerInvoiceDetailsAppService : EmsAppServiceBase, ICustomerInvoiceDetailsAppService
    {
        private readonly IRepository<CustomerInvoiceDetail> _customerInvoiceDetailRepository;
        private readonly IRepository<CustomerInvoice, int> _lookup_customerInvoiceRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;
        private readonly IRepository<LeaseItem, int> _lookup_leaseItemRepository;

        public CustomerInvoiceDetailsAppService(
            IRepository<CustomerInvoiceDetail> customerInvoiceDetailRepository,
            IRepository<CustomerInvoice, int> lookup_customerInvoiceRepository,
            IRepository<ItemType, int> lookup_itemTypeRepository,
            IRepository<Uom, int> lookup_uomRepository,
            IRepository<WorkOrderAction, int> lookup_workOrderActionRepository,
            IRepository<LeaseItem, int> lookup_leaseItemRepository)
        {
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _lookup_customerInvoiceRepository = lookup_customerInvoiceRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
            _lookup_leaseItemRepository = lookup_leaseItemRepository;
        }

        public async Task<PagedResultDto<GetCustomerInvoiceDetailForViewDto>> GetAll(GetAllCustomerInvoiceDetailsInput input)
        {

            var filteredCustomerInvoiceDetails = _customerInvoiceDetailRepository.GetAll()
                        .Include(e => e.CustomerInvoiceFk)
                        .Include(e => e.LeaseItemFk)
                        .Include(e => e.ItemTypeFk)
                        .Include(e => e.UomFk)
                        .Include(e => e.WorkOrderActionFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
                        .WhereIf(input.MinQuantityFilter != null, e => e.Quantity >= input.MinQuantityFilter)
                        .WhereIf(input.MaxQuantityFilter != null, e => e.Quantity <= input.MaxQuantityFilter)
                        .WhereIf(input.MinUnitPriceFilter != null, e => e.UnitPrice >= input.MinUnitPriceFilter)
                        .WhereIf(input.MaxUnitPriceFilter != null, e => e.UnitPrice <= input.MaxUnitPriceFilter)
                        .WhereIf(input.MinGrossFilter != null, e => e.Gross >= input.MinGrossFilter)
                        .WhereIf(input.MaxGrossFilter != null, e => e.Gross <= input.MaxGrossFilter)
                        .WhereIf(input.MinTaxFilter != null, e => e.Tax >= input.MinTaxFilter)
                        .WhereIf(input.MaxTaxFilter != null, e => e.Tax <= input.MaxTaxFilter)
                        .WhereIf(input.MinNetFilter != null, e => e.Net >= input.MinNetFilter)
                        .WhereIf(input.MaxNetFilter != null, e => e.Net <= input.MaxNetFilter)
                        .WhereIf(input.MinDiscountFilter != null, e => e.Discount >= input.MinDiscountFilter)
                        .WhereIf(input.MaxDiscountFilter != null, e => e.Discount <= input.MaxDiscountFilter)
                        .WhereIf(input.MinChargeFilter != null, e => e.Charge >= input.MinChargeFilter)
                        .WhereIf(input.MaxChargeFilter != null, e => e.Charge <= input.MaxChargeFilter)
                        .WhereIf(input.MinBillingRuleRefIdFilter != null, e => e.BillingRuleRefId >= input.MinBillingRuleRefIdFilter)
                        .WhereIf(input.MaxBillingRuleRefIdFilter != null, e => e.BillingRuleRefId <= input.MaxBillingRuleRefIdFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerInvoiceDescriptionFilter), e => e.CustomerInvoiceFk != null && e.CustomerInvoiceFk.Description.ToLower() == input.CustomerInvoiceDescriptionFilter.ToLower().Trim());

            var pagedAndFilteredCustomerInvoiceDetails = filteredCustomerInvoiceDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var customerInvoiceDetails = from o in pagedAndFilteredCustomerInvoiceDetails

                                         join o2 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o2.Id into j2
                                         from s2 in j2.DefaultIfEmpty()

                                         join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                                         from s3 in j3.DefaultIfEmpty()

                                         join o4 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o4.Id into j4
                                         from s4 in j4.DefaultIfEmpty()

                                         join o6 in _lookup_uomRepository.GetAll() on o.UomId equals o6.Id into j6
                                         from s6 in j6.DefaultIfEmpty()

                                         join o7 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o7.Id into j7
                                         from s7 in j7.DefaultIfEmpty()

                                         select new GetCustomerInvoiceDetailForViewDto()
                                         {
                                             CustomerInvoiceDetail = new CustomerInvoiceDetailDto
                                             {
                                                 Description = o.Description,
                                                 Quantity = o.Quantity,
                                                 UnitPrice = o.UnitPrice,
                                                 Gross = o.Gross,
                                                 Tax = o.Tax,
                                                 Net = o.Net,
                                                 Discount = o.Discount,
                                                 Charge = o.Charge,
                                                 BillingRuleRefId = o.BillingRuleRefId,
                                                 Id = o.Id,
                                                 LeaseItemId = o.LeaseItemId
                                             },
                                             ItemTypeType = s3 == null ? "" : s3.Type,
                                             CustomerInvoiceDescription = s2 == null ? "" : s2.Description,
                                             UomUnitOfMeasurement = s6 == null ? "" : s6.UnitOfMeasurement.ToString(),
                                             ActionWorkOrderAction = s7 == null ? "" : s7.Action,
                                             LeaseItemItem = s4 == null ? "" : s4.Item + " (" + s4.Description + ")"
                                         };

            var totalCount = await filteredCustomerInvoiceDetails.CountAsync();

            return new PagedResultDto<GetCustomerInvoiceDetailForViewDto>(
                totalCount,
                await customerInvoiceDetails.ToListAsync()
            );
        }

        public async Task<GetCustomerInvoiceDetailForViewDto> GetCustomerInvoiceDetailForView(int id)
        {
            var customerInvoiceDetail = await _customerInvoiceDetailRepository.GetAsync(id);

            var output = new GetCustomerInvoiceDetailForViewDto { CustomerInvoiceDetail = ObjectMapper.Map<CustomerInvoiceDetailDto>(customerInvoiceDetail) };

            if (output.CustomerInvoiceDetail.CustomerInvoiceId > 0)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.CustomerInvoiceId);
                output.CustomerInvoiceDescription = _lookupCustomerInvoice.Description.ToString();
            }

            if (output.CustomerInvoiceDetail.LeaseItemId > 0)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item + " (" + _lookupLeaseItem.Description + ")";
            }

            if (output.CustomerInvoiceDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.CustomerInvoiceDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.CustomerInvoiceDetail.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.WorkOrderActionId);
                output.ActionWorkOrderAction = _lookupWorkOrderAction.Action;
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_EditCustomerInvoiceDetail)]
        public async Task<GetCustomerInvoiceDetailForEditOutput> GetCustomerInvoiceDetailForEdit(EntityDto input)
        {
            var customerInvoiceDetail = await _customerInvoiceDetailRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCustomerInvoiceDetailForEditOutput { CustomerInvoiceDetail = ObjectMapper.Map<CreateOrEditCustomerInvoiceDetailDto>(customerInvoiceDetail) };

            if (output.CustomerInvoiceDetail.CustomerInvoiceId > 0)
            {
                var _lookupCustomerInvoice = await _lookup_customerInvoiceRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.CustomerInvoiceId);
                output.CustomerInvoiceDescription = _lookupCustomerInvoice.Description.ToString();
            }

            if (output.CustomerInvoiceDetail.LeaseItemId > 0)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item + " (" + _lookupLeaseItem.Description + ")";
            }

            if (output.CustomerInvoiceDetail.ItemTypeId != null)
            {
                var _lookupItemType = await _lookup_itemTypeRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.ItemTypeId);
                output.ItemTypeType = _lookupItemType.Type.ToString();
            }

            if (output.CustomerInvoiceDetail.UomId != null)
            {
                var _lookupUom = await _lookup_uomRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.UomId);
                output.UomUnitOfMeasurement = _lookupUom.UnitOfMeasurement.ToString();
            }

            if (output.CustomerInvoiceDetail.WorkOrderActionId != null)
            {
                var _lookupWorkOrderAction = await _lookup_workOrderActionRepository.FirstOrDefaultAsync((int)output.CustomerInvoiceDetail.WorkOrderActionId);
                output.ActionWorkOrderAction = _lookupWorkOrderAction.Action;
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCustomerInvoiceDetailDto input)
        {
            decimal discountPrice = 0, taxPrice = 0;

            decimal markUp = input.Net;
            decimal unitPrice = input.UnitPrice;
            decimal quantity = input.Quantity;
            decimal tax = input.Tax ?? 0;
            decimal discount = input.Discount ?? 0;
            decimal costPrice = unitPrice * quantity;

            if (markUp > 0)
                costPrice += costPrice * (markUp / 100);

            if (discount > 0)
                discountPrice = costPrice * (discount / 100);

            if (tax > 0)
                taxPrice = (costPrice - discountPrice) * (tax / 100);

            input.Gross = costPrice;
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

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_CreateCustomerInvoiceDetail)]
        protected virtual async Task Create(CreateOrEditCustomerInvoiceDetailDto input)
        {
            var customerInvoiceDetail = ObjectMapper.Map<CustomerInvoiceDetail>(input);


            if (AbpSession.TenantId != null)
            {
                customerInvoiceDetail.TenantId = (int?)AbpSession.TenantId;
            }


            await _customerInvoiceDetailRepository.InsertAsync(customerInvoiceDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_EditCustomerInvoiceDetail)]
        protected virtual async Task Update(CreateOrEditCustomerInvoiceDetailDto input)
        {
            var customerInvoiceDetail = await _customerInvoiceDetailRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, customerInvoiceDetail);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_DeleteCustomerInvoiceDetail)]
        public async Task Delete(EntityDto input)
        {
            await _customerInvoiceDetailRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceDetailCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_customerInvoiceRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Description.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var customerInvoiceList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceDetailCustomerInvoiceLookupTableDto>();
            foreach (var customerInvoice in customerInvoiceList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceDetailCustomerInvoiceLookupTableDto
                {
                    Id = customerInvoice.Id,
                    DisplayName = customerInvoice.Description?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceDetailCustomerInvoiceLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input)
        {
            var tenantInfo = await TenantManager.GetTenantInfo();
            var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "LeaseItem");

            IQueryable<LeaseItem>  query = _lookup_leaseItemRepository.GetAll()
                    .Where(e => DateTime.UtcNow >= e.StartDate && DateTime.UtcNow <= e.EndDate)
                    .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Item.Contains(input.Filter) || e.Description.Contains(input.Filter));

            var totalCount = await query.CountAsync();

            var leaseItemList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceLeaseItemLookupTableDto>();
            foreach (var leaseItem in leaseItemList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceLeaseItemLookupTableDto
                {
                    Id = leaseItem.Id,
                    DisplayName = leaseItem.Item + " (" + leaseItem.Description + ")"
                });
            }

            return new PagedResultDto<CustomerInvoiceLeaseItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

    }
}