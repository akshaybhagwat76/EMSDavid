using Ems.Assets;
using Ems.Vendors;
using Ems.Billing;


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
    public class BillingEventsAppService : EmsAppServiceBase, IBillingEventsAppService
    {
		 private readonly IRepository<BillingEvent> _billingEventRepository;
		 private readonly IBillingEventsExcelExporter _billingEventsExcelExporter;
		 private readonly IRepository<LeaseAgreement,int> _lookup_leaseAgreementRepository;
		 private readonly IRepository<VendorCharge,int> _lookup_vendorChargeRepository;
		 private readonly IRepository<BillingEventType,int> _lookup_billingEventTypeRepository;
		 

		  public BillingEventsAppService(IRepository<BillingEvent> billingEventRepository, IBillingEventsExcelExporter billingEventsExcelExporter , IRepository<LeaseAgreement, int> lookup_leaseAgreementRepository, IRepository<VendorCharge, int> lookup_vendorChargeRepository, IRepository<BillingEventType, int> lookup_billingEventTypeRepository) 
		  {
			_billingEventRepository = billingEventRepository;
			_billingEventsExcelExporter = billingEventsExcelExporter;
			_lookup_leaseAgreementRepository = lookup_leaseAgreementRepository;
		_lookup_vendorChargeRepository = lookup_vendorChargeRepository;
		_lookup_billingEventTypeRepository = lookup_billingEventTypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetBillingEventForViewDto>> GetAll(GetAllBillingEventsInput input)
         {
			
			var filteredBillingEvents = _billingEventRepository.GetAll()
						.Include( e => e.LeaseAgreementFk)
						.Include( e => e.VendorChargeFk)
						.Include( e => e.BillingEventTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.TriggeredBy.Contains(input.Filter) || e.Purpose.Contains(input.Filter))
						.WhereIf(input.MinBillingEventDateFilter != null, e => e.BillingEventDate >= input.MinBillingEventDateFilter)
						.WhereIf(input.MaxBillingEventDateFilter != null, e => e.BillingEventDate <= input.MaxBillingEventDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TriggeredByFilter),  e => e.TriggeredBy.ToLower() == input.TriggeredByFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.PurposeFilter),  e => e.Purpose.ToLower() == input.PurposeFilter.ToLower().Trim())
						.WhereIf(input.WasInvoiceGeneratedFilter > -1,  e => Convert.ToInt32(e.WasInvoiceGenerated) == input.WasInvoiceGeneratedFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorChargeReferenceFilter), e => e.VendorChargeFk != null && e.VendorChargeFk.Reference.ToLower() == input.VendorChargeReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventTypeTypeFilter), e => e.BillingEventTypeFk != null && e.BillingEventTypeFk.Type.ToLower() == input.BillingEventTypeTypeFilter.ToLower().Trim());

			var pagedAndFilteredBillingEvents = filteredBillingEvents
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var billingEvents = from o in pagedAndFilteredBillingEvents
                         join o1 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_vendorChargeRepository.GetAll() on o.VendorChargeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_billingEventTypeRepository.GetAll() on o.BillingEventTypeId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetBillingEventForViewDto() {
							BillingEvent = new BillingEventDto
							{
                                BillingEventDate = o.BillingEventDate,
                                TriggeredBy = o.TriggeredBy,
                                Purpose = o.Purpose,
                                WasInvoiceGenerated = o.WasInvoiceGenerated,
                                Id = o.Id
							},
                         	LeaseAgreementTitle = s1 == null ? "" : s1.Title.ToString(),
                         	VendorChargeReference = s2 == null ? "" : s2.Reference.ToString(),
                         	BillingEventTypeType = s3 == null ? "" : s3.Type.ToString()
						};

            var totalCount = await filteredBillingEvents.CountAsync();

            return new PagedResultDto<GetBillingEventForViewDto>(
                totalCount,
                await billingEvents.ToListAsync()
            );
         }
		 
		 public async Task<GetBillingEventForViewDto> GetBillingEventForView(int id)
         {
            var billingEvent = await _billingEventRepository.GetAsync(id);

            var output = new GetBillingEventForViewDto { BillingEvent = ObjectMapper.Map<BillingEventDto>(billingEvent) };

		    if (output.BillingEvent.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.BillingEvent.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

		    if (output.BillingEvent.VendorChargeId != null)
            {
                var _lookupVendorCharge = await _lookup_vendorChargeRepository.FirstOrDefaultAsync((int)output.BillingEvent.VendorChargeId);
                output.VendorChargeReference = _lookupVendorCharge.Reference.ToString();
            }

		    if (output.BillingEvent != null)
            {
                var _lookupBillingEventType = await _lookup_billingEventTypeRepository.FirstOrDefaultAsync((int)output.BillingEvent.BillingEventTypeId);
                output.BillingEventTypeType = _lookupBillingEventType.Type.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_Edit)]
		 public async Task<GetBillingEventForEditOutput> GetBillingEventForEdit(EntityDto input)
         {
            var billingEvent = await _billingEventRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetBillingEventForEditOutput {BillingEvent = ObjectMapper.Map<CreateOrEditBillingEventDto>(billingEvent)};

		    if (output.BillingEvent.LeaseAgreementId != null)
            {
                var _lookupLeaseAgreement = await _lookup_leaseAgreementRepository.FirstOrDefaultAsync((int)output.BillingEvent.LeaseAgreementId);
                output.LeaseAgreementTitle = _lookupLeaseAgreement.Title.ToString();
            }

		    if (output.BillingEvent.VendorChargeId != null)
            {
                var _lookupVendorCharge = await _lookup_vendorChargeRepository.FirstOrDefaultAsync((int)output.BillingEvent.VendorChargeId);
                output.VendorChargeReference = _lookupVendorCharge.Reference.ToString();
            }

		    if (output.BillingEvent != null)
            {
                var _lookupBillingEventType = await _lookup_billingEventTypeRepository.FirstOrDefaultAsync((int)output.BillingEvent.BillingEventTypeId);
                output.BillingEventTypeType = _lookupBillingEventType.Type.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditBillingEventDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_Create)]
		 protected virtual async Task Create(CreateOrEditBillingEventDto input)
         {
            var billingEvent = ObjectMapper.Map<BillingEvent>(input);

			
			if (AbpSession.TenantId != null)
			{
				billingEvent.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _billingEventRepository.InsertAsync(billingEvent);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_Edit)]
		 protected virtual async Task Update(CreateOrEditBillingEventDto input)
         {
            var billingEvent = await _billingEventRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, billingEvent);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_BillingEvents_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _billingEventRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetBillingEventsToExcel(GetAllBillingEventsForExcelInput input)
         {
			
			var filteredBillingEvents = _billingEventRepository.GetAll()
						.Include( e => e.LeaseAgreementFk)
						.Include( e => e.VendorChargeFk)
						.Include( e => e.BillingEventTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.TriggeredBy.Contains(input.Filter) || e.Purpose.Contains(input.Filter))
						.WhereIf(input.MinBillingEventDateFilter != null, e => e.BillingEventDate >= input.MinBillingEventDateFilter)
						.WhereIf(input.MaxBillingEventDateFilter != null, e => e.BillingEventDate <= input.MaxBillingEventDateFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.TriggeredByFilter),  e => e.TriggeredBy.ToLower() == input.TriggeredByFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.PurposeFilter),  e => e.Purpose.ToLower() == input.PurposeFilter.ToLower().Trim())
						.WhereIf(input.WasInvoiceGeneratedFilter > -1,  e => Convert.ToInt32(e.WasInvoiceGenerated) == input.WasInvoiceGeneratedFilter )
						.WhereIf(!string.IsNullOrWhiteSpace(input.LeaseAgreementTitleFilter), e => e.LeaseAgreementFk != null && e.LeaseAgreementFk.Title.ToLower() == input.LeaseAgreementTitleFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorChargeReferenceFilter), e => e.VendorChargeFk != null && e.VendorChargeFk.Reference.ToLower() == input.VendorChargeReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.BillingEventTypeTypeFilter), e => e.BillingEventTypeFk != null && e.BillingEventTypeFk.Type.ToLower() == input.BillingEventTypeTypeFilter.ToLower().Trim());

			var query = (from o in filteredBillingEvents
                         join o1 in _lookup_leaseAgreementRepository.GetAll() on o.LeaseAgreementId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_vendorChargeRepository.GetAll() on o.VendorChargeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         join o3 in _lookup_billingEventTypeRepository.GetAll() on o.BillingEventTypeId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()
                         
                         select new GetBillingEventForViewDto() { 
							BillingEvent = new BillingEventDto
							{
                                BillingEventDate = o.BillingEventDate,
                                TriggeredBy = o.TriggeredBy,
                                Purpose = o.Purpose,
                                WasInvoiceGenerated = o.WasInvoiceGenerated,
                                Id = o.Id
							},
                         	LeaseAgreementTitle = s1 == null ? "" : s1.Title.ToString(),
                         	VendorChargeReference = s2 == null ? "" : s2.Reference.ToString(),
                         	BillingEventTypeType = s3 == null ? "" : s3.Type.ToString()
						 });


            var billingEventListDtos = await query.ToListAsync();

            return _billingEventsExcelExporter.ExportToFile(billingEventListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_leaseAgreementRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Title.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var leaseAgreementList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventLeaseAgreementLookupTableDto>();
			foreach(var leaseAgreement in leaseAgreementList){
				lookupTableDtoList.Add(new BillingEventLeaseAgreementLookupTableDto
				{
					Id = leaseAgreement.Id,
					DisplayName = leaseAgreement.Title?.ToString()
				});
			}

            return new PagedResultDto<BillingEventLeaseAgreementLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventVendorChargeLookupTableDto>> GetAllVendorChargeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_vendorChargeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Reference.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var vendorChargeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventVendorChargeLookupTableDto>();
			foreach(var vendorCharge in vendorChargeList){
				lookupTableDtoList.Add(new BillingEventVendorChargeLookupTableDto
				{
					Id = vendorCharge.Id,
					DisplayName = vendorCharge.Reference?.ToString()
				});
			}

            return new PagedResultDto<BillingEventVendorChargeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_BillingEvents)]
         public async Task<PagedResultDto<BillingEventBillingEventTypeLookupTableDto>> GetAllBillingEventTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_billingEventTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var billingEventTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<BillingEventBillingEventTypeLookupTableDto>();
			foreach(var billingEventType in billingEventTypeList){
				lookupTableDtoList.Add(new BillingEventBillingEventTypeLookupTableDto
				{
					Id = billingEventType.Id,
					DisplayName = billingEventType.Type?.ToString()
				});
			}

            return new PagedResultDto<BillingEventBillingEventTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}