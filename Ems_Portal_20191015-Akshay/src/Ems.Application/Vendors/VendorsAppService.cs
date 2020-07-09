using Ems.Organizations;
using Ems.Billing;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Vendors.Exporting;
using Ems.Vendors.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Vendors
{
	[AbpAuthorize(AppPermissions.Pages_Main_Vendors)]
    public class VendorsAppService : EmsAppServiceBase, IVendorsAppService
    {
		 private readonly IRepository<Vendor> _vendorRepository;
		 private readonly IVendorsExcelExporter _vendorsExcelExporter;
		 private readonly IRepository<SsicCode,int> _lookup_ssicCodeRepository;
		 private readonly IRepository<Currency,int> _lookup_currencyRepository;
		 

		  public VendorsAppService(IRepository<Vendor> vendorRepository, IVendorsExcelExporter vendorsExcelExporter , IRepository<SsicCode, int> lookup_ssicCodeRepository, IRepository<Currency, int> lookup_currencyRepository) 
		  {
			_vendorRepository = vendorRepository;
			_vendorsExcelExporter = vendorsExcelExporter;
			_lookup_ssicCodeRepository = lookup_ssicCodeRepository;
		_lookup_currencyRepository = lookup_currencyRepository;
		
		  }

		 public async Task<PagedResultDto<GetVendorForViewDto>> GetAll(GetAllVendorsInput input)
         {
			
			var filteredVendors = _vendorRepository.GetAll()
						.Include( e => e.SsicCodeFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.VendorLoc8GUID.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorLoc8GUIDFilter),  e => e.VendorLoc8GUID.ToLower() == input.VendorLoc8GUIDFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SsicCodeCodeFilter), e => e.SsicCodeFk != null && e.SsicCodeFk.Code.ToLower() == input.SsicCodeCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var pagedAndFilteredVendors = filteredVendors
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var vendors = from o in pagedAndFilteredVendors
                         join o1 in _lookup_ssicCodeRepository.GetAll() on o.SsicCodeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetVendorForViewDto() {
							Vendor = new VendorDto
							{
                                Reference = o.Reference,
                                Name = o.Name,
                                Identifier = o.Identifier,
                                LogoUrl = o.LogoUrl,
                                Website = o.Website,
                                VendorLoc8GUID = o.VendorLoc8GUID,
                                Id = o.Id
							},
                         	SsicCodeCode = s1 == null ? "" : s1.Code.ToString(),
                         	CurrencyCode = s2 == null ? "" : s2.Code.ToString()
						};

            var totalCount = await filteredVendors.CountAsync();

            return new PagedResultDto<GetVendorForViewDto>(
                totalCount,
                await vendors.ToListAsync()
            );
         }

        [AbpAuthorize(AppPermissions.Pages_Main_Vendors_Profile)]
        public async Task<GetVendorForViewDto> GetVendorForView(int? id)
         {

            // START UPDATE // Add this code to enable the Vendor Profile to work -- NB: made 'id' nullable and updated the Interface accordingly

            Vendor currentVendor;
            int vendorId;

            if (id == null)
            {
                currentVendor = _vendorRepository.GetAll().Where(e => e.TenantId == AbpSession.TenantId).FirstOrDefault();
                if (currentVendor == null)
                {
                    return new GetVendorForViewDto();
                }
                vendorId = currentVendor.Id;
            }
            else
            {
                vendorId = (int)id;
            }

            var vendor = await _vendorRepository.GetAsync(vendorId);
            
            // END UPDATE // 

            var output = new GetVendorForViewDto { Vendor = ObjectMapper.Map<VendorDto>(vendor) };

		    if (output.Vendor.SsicCodeId != null)
            {
                var _lookupSsicCode = await _lookup_ssicCodeRepository.FirstOrDefaultAsync((int)output.Vendor.SsicCodeId);
                output.SsicCodeCode = _lookupSsicCode.Code.ToString();
            }

		    if (output.Vendor != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Vendor.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

            if (output.Vendor.LogoUrl != null)
            {
                int length = (output.Vendor.LogoUrl.Length >= 36 ) ? 36 : output.Vendor.LogoUrl.Length;
                output.Vendor.LogoUrl = string.Format("{0}...", output.Vendor.LogoUrl.Substring(0, length));
            }
            if (output.Vendor.Website != null)
            {
                int length = (output.Vendor.Website.Length >= 36) ? 36 : output.Vendor.Website.Length;
                output.Vendor.Website = string.Format("{0}...", output.Vendor.Website.Substring(0, length));
            }

            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_Vendors_Edit)]
		 public async Task<GetVendorForEditOutput> GetVendorForEdit(EntityDto input)
         {
            var vendor = await _vendorRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetVendorForEditOutput {Vendor = ObjectMapper.Map<CreateOrEditVendorDto>(vendor)};

		    if (output.Vendor.SsicCodeId != null)
            {
                var _lookupSsicCode = await _lookup_ssicCodeRepository.FirstOrDefaultAsync((int)output.Vendor.SsicCodeId);
                output.SsicCodeCode = _lookupSsicCode.Code.ToString();
            }

		    if (output.Vendor != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.Vendor.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditVendorDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Vendors_Create)]
		 protected virtual async Task Create(CreateOrEditVendorDto input)
         {
            var vendor = ObjectMapper.Map<Vendor>(input);

			
			if (AbpSession.TenantId != null)
			{
				vendor.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _vendorRepository.InsertAsync(vendor);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Vendors_Edit)]
		 protected virtual async Task Update(CreateOrEditVendorDto input)
         {
            var vendor = await _vendorRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, vendor);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_Vendors_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _vendorRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetVendorsToExcel(GetAllVendorsForExcelInput input)
         {
			
			var filteredVendors = _vendorRepository.GetAll()
						.Include( e => e.SsicCodeFk)
						.Include( e => e.CurrencyFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter) || e.VendorLoc8GUID.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.VendorLoc8GUIDFilter),  e => e.VendorLoc8GUID.ToLower() == input.VendorLoc8GUIDFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SsicCodeCodeFilter), e => e.SsicCodeFk != null && e.SsicCodeFk.Code.ToLower() == input.SsicCodeCodeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyCodeFilter), e => e.CurrencyFk != null && e.CurrencyFk.Code.ToLower() == input.CurrencyCodeFilter.ToLower().Trim());

			var query = (from o in filteredVendors
                         join o1 in _lookup_ssicCodeRepository.GetAll() on o.SsicCodeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetVendorForViewDto() { 
							Vendor = new VendorDto
							{
                                Reference = o.Reference,
                                Name = o.Name,
                                Identifier = o.Identifier,
                                LogoUrl = o.LogoUrl,
                                Website = o.Website,
                                VendorLoc8GUID = o.VendorLoc8GUID,
                                Id = o.Id
							},
                         	SsicCodeCode = s1 == null ? "" : s1.Code.ToString(),
                         	CurrencyCode = s2 == null ? "" : s2.Code.ToString()
						 });


            var vendorListDtos = await query.ToListAsync();

            return _vendorsExcelExporter.ExportToFile(vendorListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_Vendors)]
         public async Task<PagedResultDto<VendorSsicCodeLookupTableDto>> GetAllSsicCodeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_ssicCodeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Code.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var ssicCodeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorSsicCodeLookupTableDto>();
			foreach(var ssicCode in ssicCodeList){
				lookupTableDtoList.Add(new VendorSsicCodeLookupTableDto
				{
					Id = ssicCode.Id,
					DisplayName = ssicCode.Code?.ToString()
				});
			}

            return new PagedResultDto<VendorSsicCodeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_Vendors)]
         public async Task<PagedResultDto<VendorCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_currencyRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Code.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<VendorCurrencyLookupTableDto>();
			foreach(var currency in currencyList){
				lookupTableDtoList.Add(new VendorCurrencyLookupTableDto
				{
					Id = currency.Id,
					DisplayName = currency.Code?.ToString()
				});
			}

            return new PagedResultDto<VendorCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}