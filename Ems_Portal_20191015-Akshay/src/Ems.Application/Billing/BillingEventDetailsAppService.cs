using Ems.Billing;
using Ems.Assets;
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
	[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
    public class BillingEventDetailsAppService : EmsAppServiceBase, IBillingEventDetailsAppService
    {
		 private readonly IRepository<BillingEventDetail> _billingEventDetailRepository;
		 private readonly IBillingEventDetailsExcelExporter _billingEventDetailsExcelExporter;
		 private readonly IRepository<BillingRule,int> _lookup_billingRuleRepository;
		 private readonly IRepository<LeaseItem,int> _lookup_leaseItemRepository;
		 private readonly IRepository<BillingEvent,int> _lookup_billingEventRepository;
		 

		  public BillingEventDetailsAppService(IRepository<BillingEventDetail> billingEventDetailRepository, IBillingEventDetailsExcelExporter billingEventDetailsExcelExporter , IRepository<BillingRule, int> lookup_billingRuleRepository, IRepository<LeaseItem, int> lookup_leaseItemRepository, IRepository<BillingEvent, int> lookup_billingEventRepository) 
		  {
			_billingEventDetailRepository = billingEventDetailRepository;
			_billingEventDetailsExcelExporter = billingEventDetailsExcelExporter;
			_lookup_billingRuleRepository = lookup_billingRuleRepository;
		_lookup_leaseItemRepository = lookup_leaseItemRepository;
		_lookup_billingEventRepository = lookup_billingEventRepository;
		
		  }

		 public async Task<PagedResultDto<GetBillingEventDetailForViewDto>> GetAll(GetAllBillingEventDetailsInput input)
         {
			
			var filteredBillingEventDetails = _billingEventDetailRepository.GetAll()
						.Include( e => e.BillingRuleFk)
						.Include( e => e.LeaseItemFk)
						.Include( e => e.BillingEventFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Exception.Contains(input.Filter))
						.WhereIf(input.RuleExecutedSuccessfullyFilter > -1,  e => Convert.ToInt32(e.RuleExecutedSuccessfully) == input.RuleExecutedSuccessfullyFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ExceptionFilter),  e => e.Exception.ToLower() == input.ExceptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleNameFilter), e => e.BillingRuleFk != null && e.BillingRuleFk.Name.ToLower() == input.BillingRuleNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventPurposeFilter), e => e.BillingEventFk != null && e.BillingEventFk.Purpose.ToLower() == input.BillingEventPurposeFilter.ToLower().Trim());

			var pagedAndFilteredBillingEventDetails = filteredBillingEventDetails
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var billingEventDetails = from o in pagedAndFilteredBillingEventDetails
                         join o1 in _lookup_billingRuleRepository.GetAll() on o.BillingRuleId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_billingEventRepository.GetAll() on o.BillingEventId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetBillingEventDetailForViewDto() {
							BillingEventDetail = new BillingEventDetailDto
							{
                                RuleExecutedSuccessfully = o.RuleExecutedSuccessfully,
                                Exception = o.Exception,
                                Id = o.Id
							},
                         	BillingRuleName = s1 == null ? "" : s1.Name.ToString(),
                         	LeaseItemItem = s2 == null ? "" : s2.Item.ToString(),
                         	BillingEventPurpose = s3 == null ? "" : s3.Purpose.ToString()
						};

            var totalCount = await filteredBillingEventDetails.CountAsync();

            return new PagedResultDto<GetBillingEventDetailForViewDto>(
                totalCount,
                await billingEventDetails.ToListAsync()
            );
         }
		 
		 public async Task<GetBillingEventDetailForViewDto> GetBillingEventDetailForView(int id)
         {
            var billingEventDetail = await _billingEventDetailRepository.GetAsync(id);

            var output = new GetBillingEventDetailForViewDto { BillingEventDetail = ObjectMapper.Map<BillingEventDetailDto>(billingEventDetail) };

		    if (output.BillingEventDetail.BillingRuleId != null)
            {
                var _lookupBillingRule = await _lookup_billingRuleRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.BillingRuleId);
                output.BillingRuleName = _lookupBillingRule.Name.ToString();
            }

		    if (output.BillingEventDetail.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item.ToString();
            }

		    if (output.BillingEventDetail.BillingEventId != null)
            {
                var _lookupBillingEvent = await _lookup_billingEventRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.BillingEventId);
                output.BillingEventPurpose = _lookupBillingEvent.Purpose.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_EditEventDetails)]
		 public async Task<GetBillingEventDetailForEditOutput> GetBillingEventDetailForEdit(EntityDto input)
         {
            var billingEventDetail = await _billingEventDetailRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBillingEventDetailForEditOutput {BillingEventDetail = ObjectMapper.Map<CreateOrEditBillingEventDetailDto>(billingEventDetail)};

		    if (output.BillingEventDetail.BillingRuleId != null)
            {
                var _lookupBillingRule = await _lookup_billingRuleRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.BillingRuleId);
                output.BillingRuleName = _lookupBillingRule.Name.ToString();
            }

		    if (output.BillingEventDetail.LeaseItemId != null)
            {
                var _lookupLeaseItem = await _lookup_leaseItemRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.LeaseItemId);
                output.LeaseItemItem = _lookupLeaseItem.Item.ToString();
            }

		    if (output.BillingEventDetail.BillingEventId != null)
            {
                var _lookupBillingEvent = await _lookup_billingEventRepository.FirstOrDefaultAsync((int)output.BillingEventDetail.BillingEventId);
                output.BillingEventPurpose = _lookupBillingEvent.Purpose.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBillingEventDetailDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_CreateEventDetails)]
		 protected virtual async Task Create(CreateOrEditBillingEventDetailDto input)
         {
            var billingEventDetail = ObjectMapper.Map<BillingEventDetail>(input);

			
			if (AbpSession.TenantId != null)
			{
				billingEventDetail.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _billingEventDetailRepository.InsertAsync(billingEventDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_EditEventDetails)]
		 protected virtual async Task Update(CreateOrEditBillingEventDetailDto input)
         {
            var billingEventDetail = await _billingEventDetailRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, billingEventDetail);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_DeleteEventDetails)]
         public async Task Delete(EntityDto input)
         {
            await _billingEventDetailRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetBillingEventDetailsToExcel(GetAllBillingEventDetailsForExcelInput input)
         {
			
			var filteredBillingEventDetails = _billingEventDetailRepository.GetAll()
						.Include( e => e.BillingRuleFk)
						.Include( e => e.LeaseItemFk)
						.Include( e => e.BillingEventFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Exception.Contains(input.Filter))
						.WhereIf(input.RuleExecutedSuccessfullyFilter > -1,  e => Convert.ToInt32(e.RuleExecutedSuccessfully) == input.RuleExecutedSuccessfullyFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.ExceptionFilter),  e => e.Exception.ToLower() == input.ExceptionFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingRuleNameFilter), e => e.BillingRuleFk != null && e.BillingRuleFk.Name.ToLower() == input.BillingRuleNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseItemItemFilter), e => e.LeaseItemFk != null && e.LeaseItemFk.Item.ToLower() == input.LeaseItemItemFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventPurposeFilter), e => e.BillingEventFk != null && e.BillingEventFk.Purpose.ToLower() == input.BillingEventPurposeFilter.ToLower().Trim());

			var query = (from o in filteredBillingEventDetails
                         join o1 in _lookup_billingRuleRepository.GetAll() on o.BillingRuleId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_leaseItemRepository.GetAll() on o.LeaseItemId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_billingEventRepository.GetAll() on o.BillingEventId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetBillingEventDetailForViewDto() { 
							BillingEventDetail = new BillingEventDetailDto
							{
                                RuleExecutedSuccessfully = o.RuleExecutedSuccessfully,
                                Exception = o.Exception,
                                Id = o.Id
							},
                         	BillingRuleName = s1 == null ? "" : s1.Name.ToString(),
                         	LeaseItemItem = s2 == null ? "" : s2.Item.ToString(),
                         	BillingEventPurpose = s3 == null ? "" : s3.Purpose.ToString()
						 });


            var billingEventDetailListDtos = await query.ToListAsync();

            return _billingEventDetailsExcelExporter.ExportToFile(billingEventDetailListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventDetailBillingRuleLookupTableDto>> GetAllBillingRuleForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_billingRuleRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var billingRuleList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventDetailBillingRuleLookupTableDto>();
			foreach(var billingRule in billingRuleList){
				lookupTableDtoList.Add(new BillingEventDetailBillingRuleLookupTableDto
				{
					Id = billingRule.Id,
					DisplayName = billingRule.Name?.ToString()
				});
			}

            return new PagedResultDto<BillingEventDetailBillingRuleLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventDetailLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_leaseItemRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Item.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var leaseItemList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventDetailLeaseItemLookupTableDto>();
			foreach(var leaseItem in leaseItemList){
				lookupTableDtoList.Add(new BillingEventDetailLeaseItemLookupTableDto
				{
					Id = leaseItem.Id,
					DisplayName = leaseItem.Item?.ToString()
				});
			}

            return new PagedResultDto<BillingEventDetailLeaseItemLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventDetailBillingEventLookupTableDto>> GetAllBillingEventForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_billingEventRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Purpose.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var billingEventList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventDetailBillingEventLookupTableDto>();
			foreach(var billingEvent in billingEventList){
				lookupTableDtoList.Add(new BillingEventDetailBillingEventLookupTableDto
				{
					Id = billingEvent.Id,
					DisplayName = billingEvent.Purpose?.ToString()
				});
			}

            return new PagedResultDto<BillingEventDetailBillingEventLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}