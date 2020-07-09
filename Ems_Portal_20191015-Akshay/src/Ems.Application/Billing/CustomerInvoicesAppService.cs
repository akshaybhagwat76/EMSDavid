using Ems.Customers;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Billing.Exporting;
using Ems.Billing.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using Ems.Assets;
using Abp.Domain.Uow;
using Ems.Support;
using Ems.Metrics;
using Ems.Authorization.Users;
using Xero.NetStandard.OAuth2.Model;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Client;
using Ems.MultiTenancy;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using Ems.Utilities;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Ems.XeroModel;

namespace Ems.Billing
{
    [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
    public class CustomerInvoicesAppService : EmsAppServiceBase, ICustomerInvoicesAppService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<CustomerInvoice> _customerInvoiceRepository;
        private readonly IRepository<CustomerInvoiceDetail> _customerInvoiceDetailRepository;
        private readonly ICustomerInvoicesExcelExporter _customerInvoicesExcelExporter;
        private readonly IRepository<Customer, int> _lookup_customerRepository;
        private readonly IRepository<Currency, int> _lookup_currencyRepository;
        private readonly IRepository<BillingRule, int> _lookup_billingRuleRepository;
        private readonly IRepository<BillingEvent, int> _lookup_billingEventRepository;
        private readonly IRepository<CustomerInvoiceStatus, int> _lookup_invoiceStatusRepository;
        private readonly IRepository<CustomerInvoice, int> _lookup_customerInvoiceRepository;
        private readonly IRepository<WorkOrder, int> _lookup_workOrderRepository;
        private readonly IRepository<Estimate, int> _lookup_estimateRepository;
        private readonly IRepository<EstimateDetail, int> _lookup_estimateDetailRepository;
        private readonly IRepository<AssetOwnership, int> _lookup_assetOwnershipRepository;
        private readonly IRepository<Quotations.Quotation, int> _lookup_quotationRepository;
        private readonly IRepository<EstimateStatus, int> _lookup_estimateStatusRepository;
        private readonly IRepository<Uom, int> _lookup_uomRepository;
        private readonly IRepository<Quotations.ItemType, int> _lookup_itemTypeRepository;
        private readonly IRepository<WorkOrderAction, int> _lookup_workOrderActionRepository;
        private readonly IRepository<WorkOrderUpdate, int> _lookup_workOrderUpdateRepository;
        private readonly IRepository<WorkOrderPriority, int> _lookup_workOrderPriorityRepository;
        private readonly IRepository<WorkOrderType, int> _lookup_workOrderTypeRepository;
        private readonly IRepository<WorkOrderStatus, int> _lookup_workOrderStatusRepository;
        private readonly IRepository<SupportItem, int> _lookup_supportItemRepository;
        private readonly IRepository<Vendors.Vendor, int> _lookup_vendorRepository;
        private readonly IRepository<Incident, int> _lookup_incidentRepository;
        private readonly IRepository<Authorization.Users.User, long> _lookup_userRepository;
        private readonly IRepository<LeaseItem, int> _lookup_leaseItemRepository;


        private readonly IRepository<Ems.Organizations.Address, int> _lookup_addressRepository;
        private readonly IRepository<Ems.Organizations.Contact, int> _lookup_contactRepository;
        private readonly IRepository<AssetOwner, int> _lookup_assetOwnerRepository;

        private readonly IOptions<XeroConfiguration> XeroConfig;
        private readonly IHttpClientFactory httpClientFactory;

        private int InvoiceId { get; set; }

        public CustomerInvoicesAppService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<CustomerInvoice> customerInvoiceRepository,
            IRepository<CustomerInvoiceDetail> customerInvoiceDetailRepository,
            ICustomerInvoicesExcelExporter customerInvoicesExcelExporter,
            IRepository<Customer, int> lookup_customerRepository,
            IRepository<Currency, int> lookup_currencyRepository,
            IRepository<BillingRule, int> lookup_billingRuleRepository,
            IRepository<BillingEvent, int> lookup_billingEventRepository,
            IRepository<CustomerInvoiceStatus, int> lookup_invoiceStatusRepository,
            IRepository<CustomerInvoice, int> lookup_customerInvoiceRepository,
            IRepository<WorkOrder, int> lookup_workOrderRepository,
            IRepository<Estimate, int> lookup_estimateRepository,
            IRepository<EstimateDetail, int> lookup_estimateDetailRepository,
            IRepository<AssetOwnership, int> lookup_assetOwnershipRepository,
            IRepository<Quotations.Quotation, int> lookup_quotationRepository,
            IRepository<EstimateStatus, int> lookup_estimateStatusRepository,
            IRepository<Uom, int> lookup_uomRepository,
            IRepository<Quotations.ItemType, int> lookup_itemTypeRepository,
            IRepository<WorkOrderAction, int> lookup_workOrderActionRepository,
            IRepository<WorkOrderUpdate, int> lookup_workOrderUpdateRepository,
            IRepository<WorkOrderPriority, int> lookup_workOrderPriorityRepository,
            IRepository<WorkOrderType, int> lookup_workOrderTypeRepository,
            IRepository<WorkOrderStatus, int> lookup_workOrderStatusRepository,
            IRepository<SupportItem, int> lookup_supportItemRepository,
            IRepository<Vendors.Vendor, int> lookup_vendorRepository,
            IRepository<Incident, int> lookup_incidentRepository,
            IRepository<Authorization.Users.User, long> lookup_userRepository,
            IRepository<LeaseItem, int> lookup_leaseItemRepository,

            IRepository<Ems.Organizations.Address, int> lookup_addressRepository,
            IRepository<Ems.Organizations.Contact, int> lookup_contactRepository,
            IRepository<AssetOwner, int> lookup_assetOwnerRepository,

            IOptions<XeroConfiguration> XeroConfig,
            IHttpClientFactory httpClientFactory

            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _customerInvoiceRepository = customerInvoiceRepository;
            _customerInvoiceDetailRepository = customerInvoiceDetailRepository;
            _customerInvoicesExcelExporter = customerInvoicesExcelExporter;
            _lookup_customerRepository = lookup_customerRepository;
            _lookup_currencyRepository = lookup_currencyRepository;
            _lookup_billingRuleRepository = lookup_billingRuleRepository;
            _lookup_billingEventRepository = lookup_billingEventRepository;
            _lookup_invoiceStatusRepository = lookup_invoiceStatusRepository;
            _lookup_customerInvoiceRepository = lookup_customerInvoiceRepository;
            _lookup_workOrderRepository = lookup_workOrderRepository;
            _lookup_estimateRepository = lookup_estimateRepository;
            _lookup_estimateDetailRepository = lookup_estimateDetailRepository;
            _lookup_assetOwnershipRepository = lookup_assetOwnershipRepository;
            _lookup_quotationRepository = lookup_quotationRepository;
            _lookup_estimateStatusRepository = lookup_estimateStatusRepository;
            _lookup_uomRepository = lookup_uomRepository;
            _lookup_itemTypeRepository = lookup_itemTypeRepository;
            _lookup_workOrderActionRepository = lookup_workOrderActionRepository;
            _lookup_workOrderUpdateRepository = lookup_workOrderUpdateRepository;
            _lookup_workOrderPriorityRepository = lookup_workOrderPriorityRepository;
            _lookup_workOrderTypeRepository = lookup_workOrderTypeRepository;
            _lookup_workOrderStatusRepository = lookup_workOrderStatusRepository;
            _lookup_supportItemRepository = lookup_supportItemRepository;
            _lookup_vendorRepository = lookup_vendorRepository;
            _lookup_incidentRepository = lookup_incidentRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_leaseItemRepository = lookup_leaseItemRepository;

            _lookup_addressRepository = lookup_addressRepository;
            _lookup_contactRepository = lookup_contactRepository;
            _lookup_assetOwnerRepository = lookup_assetOwnerRepository;
            this.XeroConfig = XeroConfig;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<PagedResultDto<GetCustomerInvoiceForViewDto>> GetAll(GetAllCustomerInvoicesInput input)
        {

            var filteredCustomerInvoices = _customerInvoiceRepository.GetAll()
                        .Include(e => e.CustomerFk)
                        .Include(e => e.WorkOrderFk)
                        .Include(e => e.EstimateFk)
                        .Include(e => e.CurrencyFk)
                        .Include(e => e.BillingRuleFk)
                        .Include(e => e.BillingEventFk)
                        .Include(e => e.InvoiceStatusFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CustomerReference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.InvoiceRecipient.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerReferenceFilter), e => e.CustomerReference.ToLower() == input.CustomerReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinDateIssuedFilter != null, e => e.DateIssued >= input.MinDateIssuedFilter)
                        .WhereIf(input.MaxDateIssuedFilter != null, e => e.DateIssued <= input.MaxDateIssuedFilter)
                        .WhereIf(input.MinDateDueFilter != null, e => e.DateDue >= input.MinDateDueFilter)
                        .WhereIf(input.MaxDateDueFilter != null, e => e.DateDue <= input.MaxDateDueFilter)
                        .WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
                        .WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
                        .WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
                        .WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
                        .WhereIf(input.MinTotalNetFilter != null, e => e.TotalNet >= input.MinTotalNetFilter)
                        .WhereIf(input.MaxTotalNetFilter != null, e => e.TotalNet <= input.MaxTotalNetFilter)
                        .WhereIf(input.MinTotalDiscountFilter != null, e => e.TotalDiscount >= input.MinTotalDiscountFilter)
                        .WhereIf(input.MaxTotalDiscountFilter != null, e => e.TotalDiscount <= input.MaxTotalDiscountFilter)
                        .WhereIf(input.MinTotalChargeFilter != null, e => e.TotalCharge >= input.MinTotalChargeFilter)
                        .WhereIf(input.MaxTotalChargeFilter != null, e => e.TotalCharge <= input.MaxTotalChargeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InvoiceRecipientFilter), e => e.InvoiceRecipient.ToLower() == input.InvoiceRecipientFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower() == input.RemarksFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.WorkOrderSubjectFilter), e => e.WorkOrderFk != null && e.WorkOrderFk.Subject.ToLower() == input.WorkOrderSubjectFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EstimateTitleFilter), e => e.EstimateFk != null && e.EstimateFk.Title.ToLower() == input.EstimateTitleFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleNameFilter), e => e.BillingRuleFk != null && e.BillingRuleFk.Name.ToLower() == input.BillingRuleNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventPurposeFilter), e => e.BillingEventFk != null && e.BillingEventFk.Purpose.ToLower() == input.BillingEventPurposeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InvoiceStatusStatusFilter), e => e.InvoiceStatusFk != null && e.InvoiceStatusFk.Status.ToLower() == input.InvoiceStatusStatusFilter.ToLower().Trim());

            var pagedAndFilteredCustomerInvoices = filteredCustomerInvoices
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);

            var customerInvoices = from o in pagedAndFilteredCustomerInvoices
                                   join o1 in _lookup_customerRepository.GetAll() on o.CustomerId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   join o3 in _lookup_billingRuleRepository.GetAll() on o.BillingRuleId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   join o4 in _lookup_billingEventRepository.GetAll() on o.BillingEventId equals o4.Id into j4
                                   from s4 in j4.DefaultIfEmpty()

                                   join o5 in _lookup_invoiceStatusRepository.GetAll() on o.InvoiceStatusId equals o5.Id into j5
                                   from s5 in j5.DefaultIfEmpty()

                                   join o7 in _lookup_workOrderRepository.GetAll() on o.WorkOrderId equals o7.Id into j7
                                   from s7 in j7.DefaultIfEmpty()

                                   join o8 in _lookup_estimateRepository.GetAll() on o.EstimateId equals o8.Id into j8
                                   from s8 in j8.DefaultIfEmpty()

                                   select new GetCustomerInvoiceForViewDto()
                                   {
                                       CustomerInvoice = new CustomerInvoiceDto
                                       {
                                           CustomerReference = o.CustomerReference,
                                           Description = o.Description,
                                           DateIssued = o.DateIssued,
                                           DateDue = o.DateDue,
                                           TotalTax = o.TotalTax,
                                           TotalPrice = o.TotalPrice,
                                           TotalNet = o.TotalNet,
                                           TotalDiscount = o.TotalDiscount,
                                           TotalCharge = o.TotalCharge,
                                           InvoiceRecipient = o.InvoiceRecipient,
                                           Remarks = o.Remarks,
                                           Id = o.Id,
                                           CustomerId = o.CustomerId,
                                           EstimateId = o.EstimateId,
                                           WorkOrderId = o.WorkOrderId
                                       },
                                       CustomerName = s1 == null ? "" : s1.Name.ToString(),
                                       CurrencyCode = s2 == null ? "" : s2.Code.ToString(),
                                       BillingRuleName = s3 == null ? "" : s3.Name.ToString(),
                                       BillingEventPurpose = s4 == null ? "" : s4.Purpose.ToString(),
                                       InvoiceStatusStatus = s5 == null ? "" : s5.Status.ToString(),
                                       WorkOrderSubject = s7 == null ? "" : s7.Subject,
                                       EstimateTitle = s8 == null ? "" : s8.Title
                                   };

            var totalCount = await filteredCustomerInvoices.CountAsync();

            return new PagedResultDto<GetCustomerInvoiceForViewDto>(
                totalCount,
                await customerInvoices.ToListAsync()
            );
        }

        public async Task<GetCustomerInvoiceForViewDto> GetCustomerInvoiceForView(int id, PagedAndSortedResultRequestDto input)
        {
            var customerInvoice = await _customerInvoiceRepository.GetAsync(id);

            var output = new GetCustomerInvoiceForViewDto { CustomerInvoice = ObjectMapper.Map<CustomerInvoiceDto>(customerInvoice) };

            if (output?.CustomerInvoice != null)
            {
                if (output.CustomerInvoice.Id > 0)
                {
                    var filteredCustomerInvoiceDetails = _customerInvoiceDetailRepository.GetAll()
                        .Include(e => e.LeaseItemFk)
                        .Include(e => e.CustomerInvoiceFk)
                        .Where(e => e.CustomerInvoiceId == (int)output.CustomerInvoice.Id);

                    var pagedAndFilteredCustomerInvoiceDetails = filteredCustomerInvoiceDetails
                        .OrderBy(input.Sorting ?? "id asc")
                        .PageBy(input);

                    var customerInvoiceDetails = from o in pagedAndFilteredCustomerInvoiceDetails
                                                 join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                                                 from s1 in j1.DefaultIfEmpty()

                                                 join o2 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o2.Id into j2
                                                 from s2 in j2.DefaultIfEmpty()

                                                 join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                                                 from s3 in j3.DefaultIfEmpty()

                                                 join o4 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o4.Id into j4
                                                 from s4 in j4.DefaultIfEmpty()

                                                 join o5 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o5.Id into j5
                                                 from s5 in j5.DefaultIfEmpty()

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
                                                         CustomerInvoiceId = o.CustomerInvoiceId,
                                                         LeaseItemId = o.LeaseItemId
                                                     },
                                                     ItemTypeType = s1 == null ? "" : s1.Type,
                                                     CustomerInvoiceDescription = s2 == null ? "" : s2.Description.ToString(),
                                                     UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement,
                                                     ActionWorkOrderAction = s4 == null ? "" : s4.Action,
                                                     LeaseItemItem = s5 == null ? "" : s5.Item + " (" + s5.Description + ")"
                                                 };

                    var totalCount = await filteredCustomerInvoiceDetails.CountAsync();

                    output.CustomerInvoiceDetails = new PagedResultDto<GetCustomerInvoiceDetailForViewDto>(
                        totalCount,
                        await customerInvoiceDetails.ToListAsync()
                    );
                }

                if (output.CustomerInvoice.CustomerId > 0)
                {
                    var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CustomerId);
                    output.CustomerName = _lookupCustomer.Name.ToString();
                }

                if (output.CustomerInvoice.WorkOrderId > 0)
                {
                    var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.WorkOrderId);
                    output.WorkOrderSubject = _lookupWorkOrder.Subject;
                }

                if (output.CustomerInvoice.EstimateId > 0)
                {
                    var _lookupEstimate = await _lookup_estimateRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.EstimateId);
                    output.EstimateTitle = _lookupEstimate.Title;
                }

                if (output.CustomerInvoice.CurrencyId > 0)
                {
                    var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CurrencyId);
                    output.CurrencyCode = _lookupCurrency.Code.ToString();
                }

                if (output.CustomerInvoice.BillingRuleId > 0)
                {
                    var _lookupBillingRule = await _lookup_billingRuleRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingRuleId);
                    output.BillingRuleName = _lookupBillingRule.Name.ToString();
                }

                if (output.CustomerInvoice.BillingEventId > 0)
                {
                    var _lookupBillingEvent = await _lookup_billingEventRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingEventId);
                    output.BillingEventPurpose = _lookupBillingEvent.Purpose.ToString();
                }

                if (output.CustomerInvoice.InvoiceStatusId > 0)
                {
                    var _lookupInvoiceStatus = await _lookup_invoiceStatusRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.InvoiceStatusId);
                    output.InvoiceStatusStatus = _lookupInvoiceStatus.Status.ToString();
                }
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_Edit)]
        public async Task<GetCustomerInvoiceForEditOutput> GetCustomerInvoiceForEdit(EntityDto input)
        {
            var customerInvoice = await _customerInvoiceRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetCustomerInvoiceForEditOutput { CustomerInvoice = ObjectMapper.Map<CreateOrEditCustomerInvoiceDto>(customerInvoice) };

            if (output.CustomerInvoice.CustomerId > 0)
            {
                var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CustomerId);
                output.CustomerName = _lookupCustomer.Name.ToString();
            }

            if (output.CustomerInvoice.WorkOrderId > 0)
            {
                var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.WorkOrderId);
                output.WorkOrderSubject = _lookupWorkOrder.Subject;
            }

            if (output.CustomerInvoice.EstimateId > 0)
            {
                var _lookupEstimate = await _lookup_estimateRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.EstimateId);
                output.EstimateTitle = _lookupEstimate.Title;
            }

            if (output.CustomerInvoice.CurrencyId > 0)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

            if (output.CustomerInvoice.BillingRuleId > 0)
            {
                var _lookupBillingRule = await _lookup_billingRuleRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingRuleId);
                output.BillingRuleName = _lookupBillingRule.Name.ToString();
            }

            if (output.CustomerInvoice.BillingEventId > 0)
            {
                var _lookupBillingEvent = await _lookup_billingEventRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingEventId);
                output.BillingEventPurpose = _lookupBillingEvent.Purpose.ToString();
            }

            if (output.CustomerInvoice.InvoiceStatusId != null)
            {
                var _lookupInvoiceStatus = await _lookup_invoiceStatusRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.InvoiceStatusId);
                output.InvoiceStatusStatus = _lookupInvoiceStatus.Status.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditCustomerInvoiceDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_Create)]
        protected virtual async Task Create(CreateOrEditCustomerInvoiceDto input)
        {
            var customerInvoice = ObjectMapper.Map<CustomerInvoice>(input);

            if (AbpSession.TenantId != null)
                customerInvoice.TenantId = (int?)AbpSession.TenantId;

            int invoiceId = await _customerInvoiceRepository.InsertAndGetIdAsync(customerInvoice);
            
            //if (invoiceId > 0 && input.EstimateId > 0)
            //    CloneAllEstimateDetails(invoiceId, (int)input.EstimateId);
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_Edit)]
        protected virtual async Task Update(CreateOrEditCustomerInvoiceDto input)
        {
            var customerInvoice = await _customerInvoiceRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, customerInvoice);

            /*
            if (input.Id > 0 && input.EstimateId > 0)
            {
                var invoiceDetailList = _customerInvoiceDetailRepository.GetAll()
                    .Where(e => e.CustomerInvoiceId == input.Id && !e.IsDeleted);

                if (invoiceDetailList == null || invoiceDetailList.Count() == 0)
                    CloneAllEstimateDetails((int)input.Id, (int)input.EstimateId);
            }
            */
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _customerInvoiceRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetCustomerInvoicesToExcel(GetAllCustomerInvoicesForExcelInput input)
        {

            var filteredCustomerInvoices = _customerInvoiceRepository.GetAll()
                        .Include(e => e.CustomerFk)
                        .Include(e => e.CurrencyFk)
                        .Include(e => e.BillingRuleFk)
                        .Include(e => e.BillingEventFk)
                        .Include(e => e.InvoiceStatusFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CustomerReference.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.InvoiceRecipient.Contains(input.Filter) || e.Remarks.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerReferenceFilter), e => e.CustomerReference.ToLower() == input.CustomerReferenceFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
                        .WhereIf(input.MinDateIssuedFilter != null, e => e.DateIssued >= input.MinDateIssuedFilter)
                        .WhereIf(input.MaxDateIssuedFilter != null, e => e.DateIssued <= input.MaxDateIssuedFilter)
                        .WhereIf(input.MinDateDueFilter != null, e => e.DateDue >= input.MinDateDueFilter)
                        .WhereIf(input.MaxDateDueFilter != null, e => e.DateDue <= input.MaxDateDueFilter)
                        .WhereIf(input.MinTotalTaxFilter != null, e => e.TotalTax >= input.MinTotalTaxFilter)
                        .WhereIf(input.MaxTotalTaxFilter != null, e => e.TotalTax <= input.MaxTotalTaxFilter)
                        .WhereIf(input.MinTotalPriceFilter != null, e => e.TotalPrice >= input.MinTotalPriceFilter)
                        .WhereIf(input.MaxTotalPriceFilter != null, e => e.TotalPrice <= input.MaxTotalPriceFilter)
                        .WhereIf(input.MinTotalNetFilter != null, e => e.TotalNet >= input.MinTotalNetFilter)
                        .WhereIf(input.MaxTotalNetFilter != null, e => e.TotalNet <= input.MaxTotalNetFilter)
                        .WhereIf(input.MinTotalDiscountFilter != null, e => e.TotalDiscount >= input.MinTotalDiscountFilter)
                        .WhereIf(input.MaxTotalDiscountFilter != null, e => e.TotalDiscount <= input.MaxTotalDiscountFilter)
                        .WhereIf(input.MinTotalChargeFilter != null, e => e.TotalCharge >= input.MinTotalChargeFilter)
                        .WhereIf(input.MaxTotalChargeFilter != null, e => e.TotalCharge <= input.MaxTotalChargeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InvoiceRecipientFilter), e => e.InvoiceRecipient.ToLower() == input.InvoiceRecipientFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RemarksFilter), e => e.Remarks.ToLower() == input.RemarksFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CustomerNameFilter), e => e.CustomerFk != null && e.CustomerFk.Name.ToLower() == input.CustomerNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleNameFilter), e => e.BillingRuleFk != null && e.BillingRuleFk.Name.ToLower() == input.BillingRuleNameFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventPurposeFilter), e => e.BillingEventFk != null && e.BillingEventFk.Purpose.ToLower() == input.BillingEventPurposeFilter.ToLower().Trim())
                        .WhereIf(!string.IsNullOrWhiteSpace(input.InvoiceStatusStatusFilter), e => e.InvoiceStatusFk != null && e.InvoiceStatusFk.Status.ToLower() == input.InvoiceStatusStatusFilter.ToLower().Trim());

            var query = (from o in filteredCustomerInvoices
                         join o1 in _lookup_customerRepository.GetAll() on o.CustomerId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_billingRuleRepository.GetAll() on o.BillingRuleId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_billingEventRepository.GetAll() on o.BillingEventId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_invoiceStatusRepository.GetAll() on o.InvoiceStatusId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         select new GetCustomerInvoiceForViewDto()
                         {
                             CustomerInvoice = new CustomerInvoiceDto
                             {
                                 CustomerReference = o.CustomerReference,
                                 Description = o.Description,
                                 DateIssued = o.DateIssued,
                                 DateDue = o.DateDue,
                                 TotalTax = o.TotalTax,
                                 TotalPrice = o.TotalPrice,
                                 TotalNet = o.TotalNet,
                                 TotalDiscount = o.TotalDiscount,
                                 TotalCharge = o.TotalCharge,
                                 InvoiceRecipient = o.InvoiceRecipient,
                                 Remarks = o.Remarks,
                                 Id = o.Id
                             },
                             CustomerName = s1 == null ? "" : s1.Name.ToString(),
                             CurrencyCode = s2 == null ? "" : s2.Code.ToString(),
                             BillingRuleName = s3 == null ? "" : s3.Name.ToString(),
                             BillingEventPurpose = s4 == null ? "" : s4.Purpose.ToString(),
                             InvoiceStatusStatus = s5 == null ? "" : s5.Status.ToString()
                         });


            var customerInvoiceListDtos = await query.ToListAsync();

            return _customerInvoicesExcelExporter.ExportToFile(customerInvoiceListDtos);
        }

        public void UpdateCustomerInvoicePrices(int customerInvoiceId)
        {
            var invoice = _customerInvoiceRepository.Get(customerInvoiceId);

            if (invoice != null)
            {
                var invoiceDetailList = _customerInvoiceDetailRepository.GetAll()
                    .Where(e => e.CustomerInvoiceId == invoice.Id && !e.IsDeleted)
                    .ToList();

                if (invoiceDetailList != null && invoiceDetailList.Count() > 0)
                {
                    decimal totalPrice = 0, totalTax = 0, totalDiscount = 0, totalCharge = 0;

                    foreach (var item in invoiceDetailList)
                    {
                        decimal discountPrice = 0, taxPrice = 0, costPrice = 0;

                        costPrice = item.UnitPrice * item.Quantity;

                        if (item.Net > 0)
                            costPrice += costPrice * (item.Net / 100);

                        if (item.Discount > 0)
                            discountPrice = costPrice * ((decimal)item.Discount / 100);

                        if (item.Tax > 0)
                            taxPrice = (costPrice - discountPrice) * ((decimal)item.Tax / 100);

                        totalDiscount += discountPrice;
                        totalTax += taxPrice;
                        totalPrice += costPrice;
                        totalCharge += (costPrice - discountPrice) + taxPrice;
                    }

                    invoice.TotalPrice = totalPrice;
                    invoice.TotalTax = totalTax;
                    invoice.TotalDiscount = totalDiscount;
                    invoice.TotalCharge = totalCharge;
                }
                else
                {
                    invoice.TotalPrice = 0;
                    invoice.TotalTax = 0;
                    invoice.TotalDiscount = 0;
                    invoice.TotalCharge = 0;
                }

                //using (var unitOfWork = _unitOfWorkManager.Begin())
                //{
                    _customerInvoiceRepository.Update(invoice);
                //    unitOfWork.Complete();
                //    unitOfWork.Dispose();
                //}
            }
        }

        public async Task<GetCustomerInvoiceForViewDto> GetCustomerInvoiceForPDF(int id)
        {
            var customerInvoice = await _customerInvoiceRepository.GetAsync(id);

            var output = new GetCustomerInvoiceForViewDto { CustomerInvoice = ObjectMapper.Map<CustomerInvoiceDto>(customerInvoice) };

            if (output?.CustomerInvoice != null)
            {
                //EstimatePdfDto ePdf = new EstimatePdfDto();

                output.AuthenticationKey = await SettingManager.GetSettingValueAsync(Configuration.AppSettings.WebApiManagement.AuthorizationKey);
                if (output.CustomerInvoice.Id > 0)
                {

                    var filteredCustomerInvoiceDetails = _customerInvoiceDetailRepository.GetAll()
                        .Include(e => e.LeaseItemFk)
                        .Include(e => e.CustomerInvoiceFk)
                        .Where(e => e.CustomerInvoiceId == (int)output.CustomerInvoice.Id);

                    var pagedAndFilteredCustomerInvoiceDetails = filteredCustomerInvoiceDetails
                        .OrderBy("id asc");

                    var customerInvoiceDetails = from o in pagedAndFilteredCustomerInvoiceDetails
                                                 join o1 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o1.Id into j1
                                                 from s1 in j1.DefaultIfEmpty()

                                                 join o2 in _lookup_customerInvoiceRepository.GetAll() on o.CustomerInvoiceId equals o2.Id into j2
                                                 from s2 in j2.DefaultIfEmpty()

                                                 join o3 in _lookup_uomRepository.GetAll() on o.UomId equals o3.Id into j3
                                                 from s3 in j3.DefaultIfEmpty()

                                                 join o4 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o4.Id into j4
                                                 from s4 in j4.DefaultIfEmpty()

                                                 join o5 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o5.Id into j5
                                                 from s5 in j5.DefaultIfEmpty()

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
                                                         CustomerInvoiceId = o.CustomerInvoiceId,
                                                         LeaseItemId = o.LeaseItemId
                                                     },
                                                     ItemTypeType = s1 == null ? "" : s1.Type,
                                                     CustomerInvoiceDescription = s2 == null ? "" : s2.Description.ToString(),
                                                     UomUnitOfMeasurement = s3 == null ? "" : s3.UnitOfMeasurement,
                                                     ActionWorkOrderAction = s4 == null ? "" : s4.Action,
                                                     LeaseItemItem = s5 == null ? "" : s5.Item + " (" + s5.Description + ")"
                                                 };

                    var totalCount = await filteredCustomerInvoiceDetails.CountAsync();

                    output.CustomerInvoiceDetails = new PagedResultDto<GetCustomerInvoiceDetailForViewDto>(
                        totalCount,
                        await customerInvoiceDetails.ToListAsync()
                    );
                }

                int assetOwnerId = 0;

                if (output.CustomerInvoice.CustomerId > 0)
                {
                    var _lookupCustomer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CustomerId);
                    output.CustomerName = _lookupCustomer.Name.ToString();
                }

                if (output.CustomerInvoice.WorkOrderId > 0)
                {
                    var _lookupWorkOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.WorkOrderId);
                    output.WorkOrderSubject = _lookupWorkOrder.Subject;

                    if (_lookupWorkOrder != null)
                    {
                        var _lookupAssetOwnership = await _lookup_assetOwnershipRepository.GetAll().Include(a => a.AssetOwnerFk).Where(a => a.Id == (int)_lookupWorkOrder.AssetOwnershipId).FirstOrDefaultAsync();
                        assetOwnerId = (_lookupAssetOwnership != null && _lookupAssetOwnership.AssetOwnerFk != null) ? _lookupAssetOwnership.AssetOwnerFk.Id : 0;
                    }
                }

                if (output.CustomerInvoice.EstimateId > 0)
                {
                    var _lookupEstimate = await _lookup_estimateRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.EstimateId);
                    output.EstimateTitle = _lookupEstimate.Title;
                }

                if (output.CustomerInvoice.CurrencyId > 0)
                {
                    var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CurrencyId);
                    output.CurrencyCode = _lookupCurrency.Code.ToString();
                }

                if (output.CustomerInvoice.BillingRuleId > 0)
                {
                    var _lookupBillingRule = await _lookup_billingRuleRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingRuleId);
                    output.BillingRuleName = _lookupBillingRule.Name.ToString();
                }

                if (output.CustomerInvoice.BillingEventId > 0)
                {
                    var _lookupBillingEvent = await _lookup_billingEventRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.BillingEventId);
                    output.BillingEventPurpose = _lookupBillingEvent.Purpose.ToString();
                }

                if (output.CustomerInvoice.InvoiceStatusId > 0)
                {
                    var _lookupInvoiceStatus = await _lookup_invoiceStatusRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.InvoiceStatusId);
                    output.InvoiceStatusStatus = _lookupInvoiceStatus.Status.ToString();
                }

                var custAddress = _lookup_addressRepository
                    .GetAll()
                    .Where(e => e.CustomerId == output.CustomerInvoice.Id)
                    .FirstOrDefault();

                var custCnt = _lookup_contactRepository
                    .GetAll()
                    .Where(e => e.CustomerId == output.CustomerInvoice.CustomerId)
                    .FirstOrDefault();

                output.AssetOwnerInfo = ObjectMapper.Map<Assets.Dtos.AssetOwnerDto>(await _lookup_assetOwnerRepository.GetAsync(assetOwnerId));
                output.CustomerInfo = ObjectMapper.Map<Customers.Dtos.CustomerDto>(await _lookup_customerRepository.GetAsync(output.CustomerInvoice.CustomerId));
                output.CustomerAddress = ObjectMapper.Map<Organizations.Dtos.AddressDto>(custAddress);
                output.CustomerContact = ObjectMapper.Map<Organizations.Dtos.ContactDto>(custCnt);
            }

            return output;
        }

        private async Task<string> SubmitCustomerInvoice()
        {
            try
            {
                Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();
                string accessToken = xeroToken.AccessToken;
                string xeroTenantId = xeroToken.Tenants[0].TenantId.ToString();


                var customerInvoice = await _customerInvoiceRepository.GetAsync(this.InvoiceId);

                var output = new GetCustomerInvoiceForViewDto { CustomerInvoice = ObjectMapper.Map<CustomerInvoiceDto>(customerInvoice) };

                List<LineItem> lineItems = new List<LineItem>();

                if (output?.CustomerInvoice != null)
                {
                    if (output.CustomerInvoice.Id > 0)
                    {
                        var filteredCustomerInvoiceDetails = _customerInvoiceDetailRepository.GetAll()
                            .Where(item => item.CustomerInvoiceId == this.InvoiceId);

                        foreach (CustomerInvoiceDetail item in filteredCustomerInvoiceDetails)
                        {
                            LineItem lineItem = new LineItem();
                            lineItem.LineItemID = Guid.NewGuid();
                            lineItem.Description = item.Description;
                            lineItem.ItemCode = "CAFE001";
                            lineItem.Quantity = Convert.ToDouble(item.Quantity);
                            lineItem.UnitAmount = Convert.ToDouble(item.UnitPrice);
                            lineItem.TaxAmount = Convert.ToDouble((item.Gross * item.Tax) / 100);
                            //lineItem.DiscountRate = item.Discount.ToString(); // Need to check with EMS team (Discount logic wrongly implemented in EMS system)
                            lineItem.LineAmount = Convert.ToDouble(item.Gross);
                            lineItem.AccountCode = "200"; //60020
                            lineItem.TaxType = "OUTPUT";

                            lineItems.Add(lineItem);
                        }
                    }
                }

                var AccountingApi = new AccountingApi();
                var ContactResponse = await AccountingApi.GetContactsAsync(accessToken, xeroTenantId);

                if (ContactResponse != null && ContactResponse._Contacts.Count > 0)
                {
                    Customer customer = await _lookup_customerRepository.FirstOrDefaultAsync((int)output.CustomerInvoice.CustomerId);
                    var getXeroContact = ContactResponse._Contacts.Where(item => item.Name.Trim().ToLower() == customer.Name.Trim().ToLower());

                    if (getXeroContact.FirstOrDefault() != null)
                    {
                        var strContactID = getXeroContact.FirstOrDefault().ContactID;

                        Guid contactIdGuid = new Guid(strContactID.ToString());
                        ContactResponse = await AccountingApi.GetContactAsync(accessToken, xeroTenantId, contactIdGuid);
                        Contact con = new Contact();
                        con.ContactID = ContactResponse._Contacts.FirstOrDefault().ContactID;
                        con.Name = ContactResponse._Contacts.FirstOrDefault().Name;

                        Invoice inv = new Invoice();

                        inv.Type = Invoice.TypeEnum.ACCREC;
                        inv.Contact = con;
                        inv.LineItems = lineItems;
                        inv.LineAmountTypes = LineAmountTypes.Exclusive;
                        inv.InvoiceNumber = this.InvoiceId.ToString();
                        inv.SentToContact = true;
                        inv.Status = Invoice.StatusEnum.AUTHORISED;
                        inv.DueDate = Convert.ToDateTime(customerInvoice.DateDue);

                        var InvoiceAPI = await AccountingApi.CreateInvoiceAsync(accessToken, xeroTenantId, inv);

                        return InvoiceAPI._Invoices.FirstOrDefault().Status.ToString();
                    }
                    else
                    {
                        return Constant.XeroContactMessage;
                    }
                }
                else
                {
                    return Constant.XeroContactMessage;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Other methods
        private async Task<string> PaidCustomerInvoice()
        {
            try
            {
                var AccountingApi = new AccountingApi();

                Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();
                string accessToken = xeroToken.AccessToken;
                string xeroTenantId = xeroToken.Tenants[0].TenantId.ToString();

                var filteredcustomerData = _customerInvoiceRepository.GetAll().Where(item => item.Id == this.InvoiceId);

                Payment p = new Payment();
                Account account1 = new Account();
                Invoice invoice = new Invoice();
                invoice.InvoiceNumber = this.InvoiceId.ToString();

                account1.Code = "090"; //090
                p.Invoice = invoice;
                p.Account = account1;
                p.PaymentType = Payment.PaymentTypeEnum.ACCRECPAYMENT;
                p.Amount = Convert.ToDouble(filteredcustomerData.FirstOrDefault().TotalCharge);

                var PaymentAPI = await AccountingApi.CreatePaymentAsync(accessToken, xeroTenantId, p);

                return PaymentAPI._Payments.FirstOrDefault().Invoice.Status.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async void EMSUpdateStatus(int id, int invStatusId)
        {
            CustomerInvoice customerInvoice = await _customerInvoiceRepository.FirstOrDefaultAsync((int)id);
            if (customerInvoice != null)
            {
                customerInvoice.Id = id;
                customerInvoice.InvoiceStatusId = invStatusId;
                CustomerInvoice updatecustomerInvoice = await _customerInvoiceRepository.UpdateAsync(customerInvoice);
            }
        }
        #endregion

        #region Xero Callbacks and Tokanizable methods
        private async Task<Xero.NetStandard.OAuth2.Token.XeroOAuth2Token> GetXeroToken()
        {
            bool result = TokenUtilities.TokenExists();

            if (result)
            {
                var xeroToken = TokenUtilities.GetStoredToken();
                var utcTimeNow = DateTime.UtcNow;

                if (utcTimeNow > xeroToken.ExpiresAtUtc)
                {
                    var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                    xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                    TokenUtilities.StoreToken(xeroToken);
                }
                return xeroToken;

            }
            else
            {
                return null;
            }

        }
        private async Task<Xero.NetStandard.OAuth2.Token.XeroOAuth2Token> GetToken()
        {
            bool result = TokenUtilities.TokenExists();

            if (result)
            {
                var xeroToken = TokenUtilities.GetStoredToken();
                var utcTimeNow = DateTime.UtcNow;

                if (utcTimeNow > xeroToken.ExpiresAtUtc)
                {
                    var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                    xeroToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);
                    TokenUtilities.StoreToken(xeroToken);
                }
                return xeroToken;

            }
            else
            {
                return null;
            }

        }
        public async Task<bool> GetCallback(string code, string state)
        {
            try
            {
                var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                var xeroToken = (XeroOAuth2Token)await client.RequestXeroTokenAsync(code);

                List<Xero.NetStandard.OAuth2.Models.Tenant> tenants = await client.GetConnectionsAsync(xeroToken);

                Xero.NetStandard.OAuth2.Models.Tenant firstTenant = tenants[0];

                TokenUtilities.StoreToken(xeroToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Xero Invoicing API Methods
        [AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions_Create)]
        public async Task<XeroResponse> XeroCommunication(int id, string flag)
        {
            string strResult = string.Empty;
            XeroResponse xeroResponse = new XeroResponse();
            try
            {
                this.InvoiceId = id;

                var filteredCustomerInvoiceDetails = _customerInvoiceDetailRepository.GetAll()
                           .Where(item => item.CustomerInvoiceId == this.InvoiceId);

                if (filteredCustomerInvoiceDetails.Count() > 0)
                {
                    Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();

                    if (xeroToken != null)
                    {
                        string strStatus = string.Empty;
                        string accessToken = xeroToken.AccessToken;
                        string xeroTenantId = xeroToken.Tenants[0].TenantId.ToString();

                        if (flag == Constant.Submit)
                        {
                            strStatus = await SubmitCustomerInvoice();
                            if (strStatus == Constant.AuthorizedStatus)
                            {
                                EMSUpdateStatus(id, 2);
                                xeroResponse.Result = Constant.Submitted;
                                return xeroResponse;
                            }
                            else if (strStatus == Constant.XeroContactMessage)
                            {
                                xeroResponse.Result = Constant.XeroContactMessage;
                                return xeroResponse;
                            }
                            else
                            {
                                xeroResponse.Result = Constant.Error;
                                return xeroResponse;
                            }
                        }
                        else
                        {
                            xeroResponse.Result = Constant.Error;
                            return xeroResponse;
                        }
                    }
                    else
                    {
                        var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                        xeroResponse.Result = client.BuildLoginUri().ToString();
                        return xeroResponse;
                    }
                }
                else
                {
                    xeroResponse.Result = Constant.Details;
                    return xeroResponse;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> RefereshXeroInvoices()
        {
            try
            {
                string strResult = string.Empty;

                Xero.NetStandard.OAuth2.Token.XeroOAuth2Token xeroToken = await GetToken();


                if (xeroToken != null)
                {
                    string accessToken = xeroToken.AccessToken;
                    string xeroTenantId = xeroToken.Tenants[0].TenantId.ToString();

                    var AccountingApi = new AccountingApi();

                    var filteredCustomerInvoices = _customerInvoiceRepository.GetAll()
                           .Where(item => item.InvoiceStatusId == 2);

                    List<string> invoiceList = new List<string>();
                    foreach (CustomerInvoice customerInvoice in filteredCustomerInvoices)
                    {
                        invoiceList.Add(customerInvoice.Id.ToString());
                    }

                    var xeroInvoices = await AccountingApi.GetInvoicesAsync(accessToken, xeroTenantId, null, null, null, null, invoiceList, null, null);

                    foreach (Xero.NetStandard.OAuth2.Model.Invoice item in xeroInvoices._Invoices)
                    {
                        if (item.Status.ToString() == Constant.Paid.ToUpper())
                        {
                            EMSUpdateStatus(Convert.ToInt32(item.InvoiceNumber), 4);
                        }
                    }

                    strResult = Constant.Refreshed;
                }
                else
                {
                    var client = new XeroClient(XeroConfig.Value, httpClientFactory);
                    strResult = client.BuildLoginUri().ToString();
                }

                return strResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        /*
        public void CloneAllEstimateDetails(int customerInvoiceId, int estimateId)
        {
            var estimate = _lookup_estimateRepository.Get(estimateId);

            if (estimate != null)
            {
                var estimateDetailList = _lookup_estimateDetailRepository.GetAll()
                    .Where(e => e.EstimateId == estimate.Id && !e.IsDeleted);

                if (estimateDetailList != null && estimateDetailList.Count() > 0)
                {
                    foreach (var item in estimateDetailList)
                    {
                        CreateOrEditCustomerInvoiceDetailDto invDetail = new CreateOrEditCustomerInvoiceDetailDto()
                        {
                            Charge = item.Charge,
                            CustomerInvoiceId = customerInvoiceId,
                            Description = item.Description,
                            Discount = item.Discount,
                            Gross = item.Cost,
                            Net = item.MarkUp,
                            Quantity = item.Quantity,
                            ItemTypeId = item.ItemTypeId,
                            UomId = item.UomId,
                            WorkOrderActionId = item.WorkOrderActionId,
                            Tax = item.Tax,
                            UnitPrice = item.UnitPrice,
                            Id = null
                        };

                        var customerInvoiceDetail = ObjectMapper.Map<CustomerInvoiceDetail>(invDetail);

                        if (AbpSession.TenantId != null)
                            customerInvoiceDetail.TenantId = (int?)AbpSession.TenantId;

                        _customerInvoiceDetailRepository.InsertAndGetId(customerInvoiceDetail);
                    }


                    UpdateCustomerInvoicePrices(customerInvoiceId);
                }
            }
        }
        */



        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForInvoiceLookupTableInput input)
        {
            IQueryable<Customer> query;
            var tenantInfo = await TenantManager.GetTenantInfo();

            if (input.EstimateId > 0)
            {
                int customerId = _lookup_estimateRepository.Get(input.EstimateId)?.CustomerId ?? 0;

                query = _lookup_customerRepository.GetAll()
                    .Where(e => e.Id == customerId);
            }
            else if (input.WorkOrderId > 0)
            {
                int assetId = 0;
                var assetOwnershipId = _lookup_workOrderRepository.Get(input.WorkOrderId)?.AssetOwnershipId ?? 0;

                if (assetOwnershipId > 0)
                    assetId = _lookup_assetOwnershipRepository.Get(assetOwnershipId)?.AssetId ?? 0;

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

            var lookupTableDtoList = new List<CustomerInvoiceCustomerLookupTableDto>();
            foreach (var customer in customerList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceCustomerLookupTableDto
                {
                    Id = customer.Id,
                    DisplayName = customer.Name?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceCustomerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_currencyRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Code.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceCurrencyLookupTableDto>();
            foreach (var currency in currencyList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceCurrencyLookupTableDto
                {
                    Id = currency.Id,
                    DisplayName = currency.Code?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceBillingRuleLookupTableDto>> GetAllBillingRuleForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_billingRuleRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var billingRuleList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceBillingRuleLookupTableDto>();
            foreach (var billingRule in billingRuleList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceBillingRuleLookupTableDto
                {
                    Id = billingRule.Id,
                    DisplayName = billingRule.Name?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceBillingRuleLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceBillingEventLookupTableDto>> GetAllBillingEventForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_billingEventRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Purpose.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var billingEventList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceBillingEventLookupTableDto>();
            foreach (var billingEvent in billingEventList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceBillingEventLookupTableDto
                {
                    Id = billingEvent.Id,
                    DisplayName = billingEvent.Purpose?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceBillingEventLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        [AbpAuthorize(AppPermissions.Pages_Main_CustomerInvoices)]
        public async Task<PagedResultDto<CustomerInvoiceInvoiceStatusLookupTableDto>> GetAllInvoiceStatusForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_invoiceStatusRepository.GetAll().WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Status.ToString().Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var invoiceStatusList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<CustomerInvoiceInvoiceStatusLookupTableDto>();
            foreach (var invoiceStatus in invoiceStatusList)
            {
                lookupTableDtoList.Add(new CustomerInvoiceInvoiceStatusLookupTableDto
                {
                    Id = invoiceStatus.Id,
                    DisplayName = invoiceStatus.Status?.ToString()
                });
            }

            return new PagedResultDto<CustomerInvoiceInvoiceStatusLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }


        public async Task<PagedResultDto<CustomerInvoiceWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input)
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

                var lookupTableDtoList = new List<CustomerInvoiceWorkOrderLookupTableDto>();
                foreach (var workOrder in workOrderList)
                {
                    lookupTableDtoList.Add(new CustomerInvoiceWorkOrderLookupTableDto
                    {
                        Id = workOrder.Id,
                        DisplayName = workOrder.Subject?.ToString()
                    });
                }

                return new PagedResultDto<CustomerInvoiceWorkOrderLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        public async Task<PagedResultDto<CustomerInvoiceEstimateLookupTableDto>> GetAllEstimateForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input)
        {
            //input.FilterId => WorkOrder ID

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "Estimate");

                IQueryable<Estimate> query;

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                        query = _lookup_estimateRepository
                            .GetAll()
                            .WhereIf(input.FilterId > 0, e => e.WorkOrderId == input.FilterId)
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.Contains(input.Filter));
                        break;

                    case "H":
                        query = _lookup_estimateRepository
                            .GetAll()
                            .WhereIf(input.FilterId > 0, e => e.WorkOrderId == input.FilterId)
                            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Title.Contains(input.Filter));
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var totalCount = await query.CountAsync();

                var estimateList = await query
                    .PageBy(input)
                    .ToListAsync();

                var lookupTableDtoList = new List<CustomerInvoiceEstimateLookupTableDto>();
                foreach (var estimate in estimateList)
                {
                    lookupTableDtoList.Add(new CustomerInvoiceEstimateLookupTableDto
                    {
                        Id = estimate.Id,
                        DisplayName = estimate.Title
                    });
                }

                return new PagedResultDto<CustomerInvoiceEstimateLookupTableDto>(
                    totalCount,
                    lookupTableDtoList
                );
            }
        }

        public async Task<CustomerInvoiceWorkOrderFkListDto> GetWorkOrderFkData(int workOrderId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                var tenantInfo = await TenantManager.GetTenantInfo();
                var crossTenantPermissions = await TenantManager.GetCrossTenantPermissions(tenantInfo, "WorkOrder");
                int assetId = 0;

                IQueryable<Estimate> estimateQuery;
                IQueryable<Customer> customerQuery = null;
                //IQueryable<LeaseItem> leaseItemQuery = null;

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

                    //leaseItemQuery = _lookup_leaseItemRepository
                    //    .GetAll()
                    //    .Include(e => e.LeaseAgreementFk)
                    //    .Where(e => e.AssetId == assetId && DateTime.UtcNow >= e.StartDate && DateTime.UtcNow <= e.EndDate);
                }

                switch (tenantInfo.Tenant.TenantType)
                {
                    case "C":
                    case "A":
                        estimateQuery = _lookup_estimateRepository
                            .GetAll()
                            .WhereIf(tenantInfo.Tenant.Id != 0 && crossTenantPermissions != null, e => crossTenantPermissions.Contains((int)e.TenantId)) // CROSS TENANT AUTH
                            .Where(w => w.WorkOrderId == workOrderId);
                        break;

                    case "V":
                        estimateQuery = null;
                        break;

                    case "H":
                        estimateQuery = _lookup_estimateRepository
                            .GetAll()
                            .Where(w => w.WorkOrderId == workOrderId);
                        break;

                    default:
                        throw new Exception($"Cannot determine TenantType for {tenantInfo.Tenant.TenancyName}!");
                }

                var estimateTableDtoList = new List<CustomerInvoiceEstimateLookupTableDto>();
                var customerTableDtoList = new List<CustomerInvoiceCustomerLookupTableDto>();
                //var leaseItemTableDtoList = new List<CustomerInvoiceLeaseItemLookupTableDto>();

                if (estimateQuery?.Count() > 0)
                {
                    foreach (var estimate in estimateQuery)
                    {
                        estimateTableDtoList.Add(new CustomerInvoiceEstimateLookupTableDto
                        {
                            Id = estimate.Id,
                            DisplayName = estimate.Title
                        });
                    }
                }

                if (customerQuery?.Count() > 0)
                {
                    foreach (var customer in customerQuery)
                    {
                        customerTableDtoList.Add(new CustomerInvoiceCustomerLookupTableDto
                        {
                            Id = customer.Id,
                            DisplayName = customer.Name
                        });
                    }
                }

                //if (leaseItemQuery?.Count() > 0)
                //{
                //    foreach (var leaseItem in leaseItemQuery)
                //    {
                //        leaseItemTableDtoList.Add(new CustomerInvoiceLeaseItemLookupTableDto
                //        {
                //            Id = leaseItem.Id,
                //            DisplayName = leaseItem.Item + " - " + leaseItem.Description
                //        });
                //    }
                //}

                return new CustomerInvoiceWorkOrderFkListDto { EstimateList = estimateTableDtoList, CustomerList = customerTableDtoList };
            }
        }

        public CustomerInvoiceEstimateFkListDto GetEstimateFkData(int estimateId)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))  // BYPASS TENANT FILTER to include Users
            {
                IQueryable<Customer> customerQuery = null;
                //IQueryable<LeaseItem> leaseItemQuery = null;

                int customerId = _lookup_estimateRepository.GetAll().Where(e => e.Id == estimateId).Select(s => s.CustomerId).FirstOrDefault();

                if (customerId > 0)
                {
                    customerQuery = _lookup_customerRepository
                        .GetAll()
                        .Where(e => e.Id == customerId);

                    //leaseItemQuery = _lookup_leaseItemRepository
                    //    .GetAll()
                    //    .Include(e => e.LeaseAgreementFk)
                    //    .Where(e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.CustomerId == customerId && DateTime.UtcNow >= e.StartDate && DateTime.UtcNow <= e.EndDate);
                }

                var customerTableDtoList = new List<CustomerInvoiceCustomerLookupTableDto>();
                //var leaseItemTableDtoList = new List<CustomerInvoiceLeaseItemLookupTableDto>();

                if (customerQuery?.Count() > 0)
                {
                    foreach (var customer in customerQuery)
                    {
                        customerTableDtoList.Add(new CustomerInvoiceCustomerLookupTableDto
                        {
                            Id = customer.Id,
                            DisplayName = customer.Name
                        });
                    }
                }

                //if (leaseItemQuery?.Count() > 0)
                //{
                //    foreach (var leaseItem in leaseItemQuery)
                //    {
                //        leaseItemTableDtoList.Add(new CustomerInvoiceLeaseItemLookupTableDto
                //        {
                //            Id = leaseItem.Id,
                //            DisplayName = leaseItem.Item + " - " + leaseItem.Description
                //        });
                //    }
                //}

                return new CustomerInvoiceEstimateFkListDto { CustomerList = customerTableDtoList };
            }
        }

        public async Task<GetCustomerInvoiceEstimateForViewDto> GetCustomerInvoiceEstimateForView(int customerInvoiceId, PagedAndSortedResultRequestDto input)
        {
            var output = new GetCustomerInvoiceEstimateForViewDto();

            if (customerInvoiceId > 0)
            {
                var customerInvoice = await _customerInvoiceRepository.FirstOrDefaultAsync(customerInvoiceId);
                if (customerInvoice != null && customerInvoice.EstimateId > 0)
                {
                    var estimate = await _lookup_estimateRepository.FirstOrDefaultAsync((int)customerInvoice.EstimateId);
                    var estimateOutput = new Support.Dtos.GetEstimateForViewDto { Estimate = ObjectMapper.Map<Support.Dtos.EstimateDto>(estimate) };

                    if (estimateOutput.Estimate != null && estimateOutput.Estimate.Id > 0)
                    {
                        if (estimateOutput.Estimate.QuotationId > 0)
                        {
                            var quotation = await _lookup_quotationRepository.FirstOrDefaultAsync((int)estimateOutput.Estimate.QuotationId);
                            estimateOutput.QuotationTitle = quotation?.Title;
                        }

                        if (estimateOutput.Estimate.EstimateStatusId > 0)
                        {
                            var _lookupEstimateStatus = await _lookup_estimateStatusRepository.FirstOrDefaultAsync((int)estimateOutput.Estimate.EstimateStatusId);
                            estimateOutput.EstimateStatusStatus = _lookupEstimateStatus.Status;
                        }

                        if (estimateOutput.Estimate.WorkOrderId > 0)
                        {
                            var workOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)estimateOutput.Estimate.WorkOrderId);
                            estimateOutput.WorkOrderSubject = workOrder.Subject;
                        }

                        if (estimateOutput.Estimate.CustomerId > 0)
                        {
                            var customer = await _lookup_customerRepository.FirstOrDefaultAsync(estimateOutput.Estimate.CustomerId);
                            estimateOutput.CustomerName = customer.Name;
                        }


                        //Estimate Details Block
                        var filteredEstimateDetails = _lookup_estimateDetailRepository.GetAll()
                            .Include(e => e.ItemTypeFk)
                            .Include(e => e.EstimateFk)
                            .Include(e => e.UomFk)
                            .Include(e => e.WorkOrderActionFk)
                            .Where(e => e.EstimateId == estimateOutput.Estimate.Id);

                        var pagedAndFilteredEstimateDetails = filteredEstimateDetails
                            .OrderBy(input.Sorting ?? "id asc")
                            .PageBy(input);

                        var estimateDetails = from o in pagedAndFilteredEstimateDetails

                                              join o1 in _lookup_estimateRepository.GetAll() on o.EstimateId equals o1.Id into j1
                                              from s1 in j1.DefaultIfEmpty()

                                              join o2 in _lookup_uomRepository.GetAll() on o.UomId equals o2.Id into j2
                                              from s2 in j2.DefaultIfEmpty()

                                              join o3 in _lookup_itemTypeRepository.GetAll() on o.ItemTypeId equals o3.Id into j3
                                              from s3 in j3.DefaultIfEmpty()

                                              join o4 in _lookup_workOrderActionRepository.GetAll() on o.WorkOrderActionId equals o4.Id into j4
                                              from s4 in j4.DefaultIfEmpty()


                                              select new Support.Dtos.GetEstimateDetailForViewDto()
                                              {
                                                  EstimateDetail = new Support.Dtos.EstimateDetailDto
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
                                                  EstimateTitle = s1 == null ? "" : s1.Title,
                                                  UomUnitOfMeasurement = s2 == null ? "" : s2.UnitOfMeasurement,
                                                  ItemTypeType = s3 == null ? "" : s3.Type,
                                                  ActionWorkOrderAction = s4 == null ? "" : s4.Action
                                              };

                        var totalCount = await filteredEstimateDetails.CountAsync();

                        var estimateDetailsOutput = new PagedResultDto<Support.Dtos.GetEstimateDetailForViewDto>(
                            totalCount,
                            await estimateDetails.ToListAsync()
                        );

                        output.Estimate = estimateOutput;
                        output.EstimateDetails = estimateDetailsOutput;
                    }
                }
            }

            return output;
        }

        public async Task<GetCustomerInvoiceWorkOrderForViewDto> GetCustomerInvoiceWorkOrderForView(int customerInvoiceId, PagedAndSortedResultRequestDto input)
        {
            var output = new GetCustomerInvoiceWorkOrderForViewDto();

            if (customerInvoiceId > 0)
            {
                var customerInvoice = await _customerInvoiceRepository.FirstOrDefaultAsync(customerInvoiceId);
                if (customerInvoice != null && customerInvoice.WorkOrderId > 0)
                {
                    var workOrder = await _lookup_workOrderRepository.FirstOrDefaultAsync((int)customerInvoice.WorkOrderId);
                    var woOutput = new Support.Dtos.GetWorkOrderForViewDto { WorkOrder = ObjectMapper.Map<Support.Dtos.WorkOrderDto>(workOrder) };

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

                                               select new Support.Dtos.GetWorkOrderUpdateForViewDto()
                                               {
                                                   WorkOrderUpdate = new Support.Dtos.WorkOrderUpdateDto
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

                        var woUpdatesOutput = new PagedResultDto<Support.Dtos.GetWorkOrderUpdateForViewDto>(
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
    }
}