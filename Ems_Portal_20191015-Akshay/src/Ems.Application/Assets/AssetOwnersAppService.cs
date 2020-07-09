using Ems.Billing;
using Ems.Organizations;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Exporting;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Assets
{
	[AbpAuthorize(AppPermissions.Pages_Main_AssetOwners)]
    public class AssetOwnersAppService : EmsAppServiceBase, IAssetOwnersAppService
    {
		private readonly IRepository<AssetOwner> _assetOwnerRepository;
		private readonly IAssetOwnersExcelExporter _assetOwnersExcelExporter;
		private readonly IRepository<Currency,int> _lookup_currencyRepository;
		private readonly IRepository<SsicCode,int> _lookup_ssicCodeRepository;

        public AssetOwnersAppService(IRepository<AssetOwner> assetOwnerRepository, 
            IAssetOwnersExcelExporter assetOwnersExcelExporter, 
            IRepository<Currency, int> lookup_currencyRepository, 
            IRepository<SsicCode, int> lookup_ssicCodeRepository)
        {
			_assetOwnerRepository = assetOwnerRepository;
			_assetOwnersExcelExporter = assetOwnersExcelExporter;
			_lookup_currencyRepository = lookup_currencyRepository;
		    _lookup_ssicCodeRepository = lookup_ssicCodeRepository;
        }

		 public async Task<PagedResultDto<GetAssetOwnerForViewDto>> GetAll(GetAllAssetOwnersInput input)
         {
			
			var filteredAssetOwners = _assetOwnerRepository.GetAll()
						.Include( e => e.CurrencyFk)
						.Include( e => e.SsicCodeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.WebsiteFilter),  e => e.Website.ToLower() == input.WebsiteFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.Name.ToLower() == input.CurrencyNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SsicCodeCodeFilter), e => e.SsicCodeFk != null && e.SsicCodeFk.Code.ToLower() == input.SsicCodeCodeFilter.ToLower().Trim());

			var pagedAndFilteredAssetOwners = filteredAssetOwners
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var assetOwners = from o in pagedAndFilteredAssetOwners
                              join o1 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o1.Id into j1
                              from s1 in j1.DefaultIfEmpty()

                              join o2 in _lookup_ssicCodeRepository.GetAll() on o.SsicCodeId equals o2.Id into j2
                              from s2 in j2.DefaultIfEmpty()

                              select new GetAssetOwnerForViewDto() {
                                  AssetOwner = new AssetOwnerDto
                                  {
                                      Reference = o.Reference,
                                      Name = o.Name,
                                      Identifier = o.Identifier,
                                      LogoUrl = o.LogoUrl,
                                      Website = o.Website,
                                      Id = o.Id
                                  },
                                  CurrencyCode = s1 == null ? "" : s1.Code.ToString(),
                                  SsicCodeCode = s2 == null ? "" : $"{s2.Code} ({s2.SSIC})" 
						};

            var totalCount = await filteredAssetOwners.CountAsync();

            return new PagedResultDto<GetAssetOwnerForViewDto>(
                totalCount,
                await assetOwners.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetOwnerForViewDto> GetAssetOwnerForView(int? id)
         {
            // START UPDATE // Add this code to enable the Customer Profile to work -- NB: made 'id' nullable and updated the Interface accordingly

            AssetOwner currentAssetOwner;
            int assetOwnerId;

            if (id == null)
            {
                currentAssetOwner = _assetOwnerRepository.GetAll().Where(e => e.TenantId == AbpSession.TenantId).FirstOrDefault();
                if (currentAssetOwner == null)
                {
                    return new GetAssetOwnerForViewDto();
                }
                assetOwnerId = currentAssetOwner.Id;
            }
            else
            {
                assetOwnerId = (int)id;
            }

            var assetOwner = await _assetOwnerRepository.GetAsync(assetOwnerId);
            
            // END UPDATE //

            var output = new GetAssetOwnerForViewDto { AssetOwner = ObjectMapper.Map<AssetOwnerDto>(assetOwner) };

		    if (output.AssetOwner.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.AssetOwner.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

		    if (output.AssetOwner.SsicCodeId != null)
            {
                var _lookupSsicCode = await _lookup_ssicCodeRepository.FirstOrDefaultAsync((int)output.AssetOwner.SsicCodeId);
                output.SsicCodeCode = $"{_lookupSsicCode.Code} ({_lookupSsicCode.SSIC})";
            }

            if(!string.IsNullOrWhiteSpace(output.AssetOwner.LogoUrl))
            {
                int length = (output.AssetOwner.LogoUrl.Length >= 36) ? 36 : output.AssetOwner.LogoUrl.Length;
                output.AssetOwner.LogoUrl = string.Format("{0}...", output.AssetOwner.LogoUrl.Substring(0, 36));
            }
            if (!string.IsNullOrWhiteSpace(output.AssetOwner.Website))
            {
                int length = (output.AssetOwner.Website.Length >= 36) ? 36 : output.AssetOwner.Website.Length;
                output.AssetOwner.Website = string.Format("{0}...", output.AssetOwner.Website.Substring(0, 36));
            }

            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwners_Edit)]
		 public async Task<GetAssetOwnerForEditOutput> GetAssetOwnerForEdit(EntityDto input)
         {
            var assetOwner = await _assetOwnerRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetOwnerForEditOutput {AssetOwner = ObjectMapper.Map<CreateOrEditAssetOwnerDto>(assetOwner)};

		    if (output.AssetOwner.CurrencyId != null)
            {
                var _lookupCurrency = await _lookup_currencyRepository.FirstOrDefaultAsync((int)output.AssetOwner.CurrencyId);
                output.CurrencyCode = _lookupCurrency.Code.ToString();
            }

		    if (output.AssetOwner.SsicCodeId != null)
            {
                var _lookupSsicCode = await _lookup_ssicCodeRepository.FirstOrDefaultAsync((int)output.AssetOwner.SsicCodeId);
                output.SsicCodeCode = $"{_lookupSsicCode.Code} ({_lookupSsicCode.SSIC})";
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetOwnerDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwners_Create)]
		 protected virtual async Task Create(CreateOrEditAssetOwnerDto input)
         {
            var assetOwner = ObjectMapper.Map<AssetOwner>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetOwner.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetOwnerRepository.InsertAsync(assetOwner);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwners_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetOwnerDto input)
         {
            var assetOwner = await _assetOwnerRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetOwner);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwners_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetOwnerRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetOwnersToExcel(GetAllAssetOwnersForExcelInput input)
         {
			
			var filteredAssetOwners = _assetOwnerRepository.GetAll()
						.Include( e => e.CurrencyFk)
						.Include( e => e.SsicCodeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Reference.Contains(input.Filter) || e.Name.Contains(input.Filter) || e.Identifier.Contains(input.Filter) || e.LogoUrl.Contains(input.Filter) || e.Website.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ReferenceFilter),  e => e.Reference.ToLower() == input.ReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.IdentifierFilter),  e => e.Identifier.ToLower() == input.IdentifierFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.WebsiteFilter),  e => e.Website.ToLower() == input.WebsiteFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.CurrencyNameFilter), e => e.CurrencyFk != null && e.CurrencyFk.Name.ToLower() == input.CurrencyNameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.SsicCodeCodeFilter), e => e.SsicCodeFk != null && e.SsicCodeFk.Code.ToLower() == input.SsicCodeCodeFilter.ToLower().Trim());

			var query = (from o in filteredAssetOwners
                         join o1 in _lookup_currencyRepository.GetAll() on o.CurrencyId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_ssicCodeRepository.GetAll() on o.SsicCodeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetAssetOwnerForViewDto() { 
							AssetOwner = new AssetOwnerDto
							{
                                Reference = o.Reference,
                                Name = o.Name,
                                Identifier = o.Identifier,
                                LogoUrl = o.LogoUrl,
                                Website = o.Website,
                                Id = o.Id
							},
                         	CurrencyCode = s1 == null ? "" : s1.Code.ToString(),
                         	SsicCodeCode = s2 == null ? "" : s2.Code.ToString()
						 });


            var assetOwnerListDtos = await query.ToListAsync();

            return _assetOwnersExcelExporter.ExportToFile(assetOwnerListDtos);
         }


        //[UnitOfWork] // <-- added this
        [AbpAuthorize(AppPermissions.Pages_Main_AssetOwners)]
         public virtual async Task<PagedResultDto<AssetOwnerCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input)  // <--- added 'virtual'
         {
            //IQueryable<Currency> query; // <--- added this

            //using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant)) // <--- added this
            //{
                var query = _lookup_currencyRepository.GetAll().WhereIf( // <-- removed 'var'
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e => e.Name.ToString().Contains(input.Filter)
                );
            //}

            var totalCount = await query.CountAsync();

            var currencyList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetOwnerCurrencyLookupTableDto>();
			foreach(var currency in currencyList){
				lookupTableDtoList.Add(new AssetOwnerCurrencyLookupTableDto
				{
					Id = currency.Id,
					DisplayName = currency.Name?.ToString()
				});
			}

            return new PagedResultDto<AssetOwnerCurrencyLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetOwners)]
         public async Task<PagedResultDto<AssetOwnerSsicCodeLookupTableDto>> GetAllSsicCodeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_ssicCodeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Code.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var ssicCodeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetOwnerSsicCodeLookupTableDto>();
			foreach(var ssicCode in ssicCodeList){
				lookupTableDtoList.Add(new AssetOwnerSsicCodeLookupTableDto
				{
					Id = ssicCode.Id,
					DisplayName = $"{ssicCode.Code} ({ssicCode.SSIC})"
                });
			}

            return new PagedResultDto<AssetOwnerSsicCodeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}