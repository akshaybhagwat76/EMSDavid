using Ems.Billing;
using Ems.Telematics;
using Ems.Assets;
using Ems.Vendors;
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
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Billing
{
	[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
    public class BillingRulesAppService : EmsAppServiceBase, IBillingRulesAppService
    {
		 private readonly IRepository<BillingRule> _billingRuleRepository;
		 private readonly IBillingRulesExcelExporter _billingRulesExcelExporter;
		 private readonly IRepository<BillingRuleType,int> _lookup_billingRuleTypeRepository;
		 private readonly IRepository<UsageMetric,int> _lookup_usageMetricRepository;
		 private readonly IRepository<LeaseAgreement,int> _lookup_leaseAgreementRepository;
		 private readonly IRepository<Vendor,int> _lookup_vendorRepository;
		 private readonly IRepository<LeaseItem,int> _lookup_leaseItemRepository;
		 private readonly IRepository<Currency,int> _lookup_currencyRepository;
		 

		  public BillingRulesAppService(IRepository<BillingRule> billingRuleRepository, IBillingRulesExcelExporter billingRulesExcelExporter , IRepository<BillingRuleType, int> lookup_billingRuleTypeRepository, IRepository<UsageMetric, int> lookup_usageMetricRepository, IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository, IRepository<Vendor, int> lookup_vendorRepository, IRepository<LeaseItem, int> lookup_leaseItemRepository, IRepository<Currency, int> lookup_currencyRepository) 
		  {
			_billingRuleRepository = billingRuleRepository;
			_billingRulesExcelExporter = billingRulesExcelExporter;
			_lookup_billingRuleTypeRepository = lookup_billingRuleTypeRepository;
		_lookup_usageMetricRepository = lookup_usageMetricRepository;
		_lookup_leaseAgreementRepository = lookup_leaseAgreementRepository;
		_lookup_vendorRepository = lookup_vendorRepository;
		_lookup_leaseItemRepository = lookup_leaseItemRepository;
		_lookup_currencyRepository = lookup_currencyRepository;
		
		  }

		 public async Task<PagedResultDto<GetBillingRuleForViewDto>> GetAll(GetAllBillingRulesInput input)
         {
			
			var filteredBillingRules = _billingRuleRepository.GetAll()
						.Include( e => e.BillingRuleTypeFk)
						.Include( e => e.UsageMetricFk)
						.Include( e => e.LeaseAgreementFk)
						.Include( e => e.VendorFk)
						.Include( e => e.LeaseItemFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.DefaultInvoiceDescription.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(input.IsParentFilter > -1,  e => Convert.ToInt32(e.IsParent) == input.IsParentFilter )
						.WhereIf(input.MinParentBillingRuleRefIdFilter != null, e => e.ParentBillingRuleRefId >= input.MinParentBillingRuleRefIdFilter)
						.WhereIf(input.MaxParentBillingRuleRefIdFilter != null, e => e.ParentBillingRuleRefId <= input.MaxParentBillingRuleRefIdFilter)
						.WhereIf(input.MinChargePerUnitFilter != null, e => e.ChargePerUnit >= input.MinChargePerUnitFilter)
						.WhereIf(input.MaxChargePerUnitFilter != null, e => e.ChargePerUnit <= input.MaxChargePerUnitFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleTypeTypeFilter), e => e.BillingRuleTypeFk != null && e.BillingRuleTypeFk.Type.ToLower() == input.BillingRuleTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric.ToLower() == input.UsageMetricMetricFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var pagedAndFilteredBillingRules = filteredBillingRules
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var billingRules = from o in pagedAndFilteredBillingRules
                         join o1 in _lookup_billingRuleTypeRepository.GetAll() on o.BillingRuleTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_vendorRepository.GetAll() on o.VendorId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         select new GetBillingRuleForViewDto() {
							BillingRule = new BillingRuleDto
							{
                                Name = o.Name,
                                IsParent = o.IsParent,
                                ParentBillingRuleRefId = o.ParentBillingRuleRefId,
                                ChargePerUnit = o.ChargePerUnit,
                                DefaultInvoiceDescription = o.DefaultInvoiceDescription,
                                Id = o.Id
							},
                         	BillingRuleTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	UsageMetricMetric = s2 == null ? "" : s2.Metric.ToString(),
                         	LeaseAgreementTitle = s3 == null ? "" : s3.Title.ToString(),
                         	VendorName = s4 == null ? "" : s4.Name.ToString(),
                         	LeaseItemItem = s5 == null ? "" : s5.Item.ToString(),
                         	CurrencyCode = s6 == null ? "" : s6.Code.ToString()
						};

            var totalCount = await filteredBillingRules.CountAsync();

            return new PagedResultDto<GetBillingRuleForViewDto>(
                totalCount,
                await billingRules.ToListAsync()
            );
         }
		 
		 public async Task<GetBillingRuleForViewDto> GetBillingRuleForView(int id)
         {
            var billingRule = await _billingRuleRepository.GetAsync(id);

            var output = new GetBillingRuleForViewDto { BillingRule = ObjectMapper.Map<BillingRuleDto>(billingRule) };

		    if (output.BillingRule != null)
            {
                var _lookupBillingRuleType = await _lookup_billingRuleTypeRepository.FirstOrDefaultAsync((int)output.BillingRule.BillingRuleTypeId);
                output.BillingRuleTypeType = _lookupBillingRuleType.Type.ToString();
            }

		    if (output.BillingRule != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.BillingRule.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

		    if (output.BillingRule.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.BillingRule.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

		    if (output.BillingRule.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.BillingRule.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.BillingRule.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.BillingRule.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item.ToString();
            }

		    if (output.BillingRule.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.BillingRule.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_BillingRules_Edit)]
		 public async Task<GetBillingRuleForEditOutput> GetBillingRuleForEdit(EntityDto input)
         {
            var billingRule = await _billingRuleRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBillingRuleForEditOutput {BillingRule = ObjectMapper.Map<CreateOrEditBillingRuleDto>(billingRule)};

		    if (output.BillingRule != null)
            {
                var _lookupBillingRuleType = await _lookup_billingRuleTypeRepository.FirstOrDefaultAsync((int)output.BillingRule.BillingRuleTypeId);
                output.BillingRuleTypeType = _lookupBillingRuleType.Type.ToString();
            }

		    if (output.BillingRule != null)
            {
                var _lookupUsageMetric = await _lookup_usageMetricRepository.FirstOrDefaultAsync((int)output.BillingRule.UsageMetricId);
                output.UsageMetricMetric = _lookupUsageMetric.Metric.ToString();
            }

		    if (output.BillingRule.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.BillingRule.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

		    if (output.BillingRule.VendorId != null)
            {
                var _lookupVendor = await _lookup_vendorRepository.FirstOrDefaultAsync((int)output.BillingRule.VendorId);
                output.VendorName = _lookupVendor.Name.ToString();
            }

		    if (output.BillingRule.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.BillingRule.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item.ToString();
            }

		    if (output.BillingRule.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.BillingRule.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBillingRuleDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingRules_Create)]
		 protected virtual async Task Create(CreateOrEditBillingRuleDto input)
         {
            var billingRule = ObjectMapper.Map<BillingRule>(input);

			
			if (AbpSession.TenantId != null)
			{
				billingRule.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _billingRuleRepository.InsertAsync(billingRule);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingRules_Edit)]
		 protected virtual async Task Update(CreateOrEditBillingRuleDto input)
         {
            var billingRule = await _billingRuleRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, billingRule);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingRules_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _billingRuleRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetBillingRulesToExcel(GetAllBillingRulesForExcelInput input)
         {
			
			var filteredBillingRules = _billingRuleRepository.GetAll()
						.Include( e => e.BillingRuleTypeFk)
						.Include( e => e.UsageMetricFk)
						.Include( e => e.LeaseAgreementFk)
						.Include( e => e.VendorFk)
						.Include( e => e.LeaseItemFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.DefaultInvoiceDescription.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(input.IsParentFilter > -1,  e => Convert.ToInt32(e.IsParent) == input.IsParentFilter )
						.WhereIf(input.MinParentBillingRuleRefIdFilter != null, e => e.ParentBillingRuleRefId >= input.MinParentBillingRuleRefIdFilter)
						.WhereIf(input.MaxParentBillingRuleRefIdFilter != null, e => e.ParentBillingRuleRefId <= input.MaxParentBillingRuleRefIdFilter)
						.WhereIf(input.MinChargePerUnitFilter != null, e => e.ChargePerUnit >= input.MinChargePerUnitFilter)
						.WhereIf(input.MaxChargePerUnitFilter != null, e => e.ChargePerUnit <= input.MaxChargePerUnitFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleTypeTypeFilter), e => e.BillingRuleTypeFk != null && e.BillingRuleTypeFk.Type.ToLower() == input.BillingRuleTypeTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.UsageMetricMetricFilter), e => e.UsageMetricFk != null && e.UsageMetricFk.Metric.ToLower() == input.UsageMetricMetricFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorNameFilter), e => e.VendorFk != null && e.VendorFk.Name.ToLower() == input.VendorNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var query = (from o in filteredBillingRules
                         join o1 in _lookup_billingRuleTypeRepository.GetAll() on o.BillingRuleTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_usageMetricRepository.GetAll() on o.UsageMetricId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         join o4 in _lookup_vendorRepository.GetAll() on o.VendorId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()
                         
                         join o5 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()
                         
                         join o6 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o6.Id into j6
                         from s6 in j6.DefaultIfEmpty()
                         
                         select new GetBillingRuleForViewDto() { 
							BillingRule = new BillingRuleDto
							{
                                Name = o.Name,
                                IsParent = o.IsParent,
                                ParentBillingRuleRefId = o.ParentBillingRuleRefId,
                                ChargePerUnit = o.ChargePerUnit,
                                DefaultInvoiceDescription = o.DefaultInvoiceDescription,
                                Id = o.Id
							},
                         	BillingRuleTypeType = s1 == null ? "" : s1.Type.ToString(),
                         	UsageMetricMetric = s2 == null ? "" : s2.Metric.ToString(),
                         	LeaseAgreementTitle = s3 == null ? "" : s3.Title.ToString(),
                         	VendorName = s4 == null ? "" : s4.Name.ToString(),
                         	LeaseItemItem = s5 == null ? "" : s5.Item.ToString(),
                         	CurrencyCode = s6 == null ? "" : s6.Code.ToString()
						 });


            var billingRuleListDtos = await query.ToListAsync();

            return _billingRulesExcelExporter.ExportToFile(billingRuleListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleBillingRuleTypeLookupTableDto>> GetAllBillingRuleTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_billingRuleTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var billingRuleTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleBillingRuleTypeLookupTableDto>();
			foreach(var billingRuleType in billingRuleTypeList){
				lookupTableDtoList.Add(new BillingRuleBillingRuleTypeLookupTableDto
				{
					Id = billingRuleType.Id,
					DisplayName = billingRuleType.Type?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleBillingRuleTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_usageMetricRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Metric.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var usageMetricList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleUsageMetricLookupTableDto>();
			foreach(var usageMetric in usageMetricList){
				lookupTableDtoList.Add(new BillingRuleUsageMetricLookupTableDto
				{
					Id = usageMetric.Id,
					DisplayName = usageMetric.Metric?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleUsageMetricLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_leaseAgreementRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Title.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var leaseAgreementList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleLeaseAgreementLookupTableDto>();
			foreach(var leaseAgreement in leaseAgreementList){
				lookupTableDtoList.Add(new BillingRuleLeaseAgreementLookupTableDto
				{
					Id = leaseAgreement.Id,
					DisplayName = leaseAgreement.Title?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleLeaseAgreementLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleVendorLookupTableDto>> GetAllVendorForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleVendorLookupTableDto>();
			foreach(var vendor in vendorList){
				lookupTableDtoList.Add(new BillingRuleVendorLookupTableDto
				{
					Id = vendor.Id,
					DisplayName = vendor.Name?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleVendorLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_leaseItemRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Item.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var leaseItemList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleLeaseItemLookupTableDto>();
			foreach(var leaseItem in leaseItemList){
				lookupTableDtoList.Add(new BillingRuleLeaseItemLookupTableDto
				{
					Id = leaseItem.Id,
					DisplayName = leaseItem.Item?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleLeaseItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingRules)]
         public async Task<PagedResultDto<BillingRuleCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_currencyRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Code.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingRuleCurrencyLookupTableDto>();
			foreach(var currency in currencyList){
				lookupTableDtoList.Add(new BillingRuleCurrencyLookupTableDto
				{
					Id = currency.Id,
					DisplayName = currency.Code?.ToString()
				});
			}

            return new PagedResultDto<BillingRuleCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}