using Ems.Assets;
using System;
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
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Assets
{
	[AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships)]
    public class AssetOwnershipsAppService : EmsAppServiceBase, IAssetOwnershipsAppService
    {
		 private readonly IRepository<AssetOwnership> _assetOwnershipRepository;
		 private readonly IAssetOwnershipsExcelExporter _assetOwnershipsExcelExporter;
		 private readonly IRepository<Asset,int> _lookup_assetRepository;
		 private readonly IRepository<AssetOwner,int> _lookup_assetOwnerRepository;
		 

		  public AssetOwnershipsAppService(IRepository<AssetOwnership> assetOwnershipRepository, IAssetOwnershipsExcelExporter assetOwnershipsExcelExporter , IRepository<Asset, int> lookup_assetRepository, IRepository<AssetOwner, int> lookup_assetOwnerRepository) 
		  {
			_assetOwnershipRepository = assetOwnershipRepository;
			_assetOwnershipsExcelExporter = assetOwnershipsExcelExporter;
			_lookup_assetRepository = lookup_assetRepository;
		_lookup_assetOwnerRepository = lookup_assetOwnerRepository;
		
		  }

		 public async Task<PagedResultDto<GetAssetOwnershipForViewDto>> GetAll(GetAllAssetOwnershipsInput input)
         {
			
			var filteredAssetOwnerships = _assetOwnershipRepository.GetAll()
						.Include( e => e.AssetFk)
						.Include( e => e.AssetOwnerFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
						.WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
						.WhereIf(input.MinFinishDateFilter != null, e => e.FinishDate >= input.MinFinishDateFilter)
						.WhereIf(input.MaxFinishDateFilter != null, e => e.FinishDate <= input.MaxFinishDateFilter)
						.WhereIf(input.MinPercentageOwnershipFilter != null, e => e.PercentageOwnership >= input.MinPercentageOwnershipFilter)
						.WhereIf(input.MaxPercentageOwnershipFilter != null, e => e.PercentageOwnership <= input.MaxPercentageOwnershipFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim());

			var pagedAndFilteredAssetOwnerships = filteredAssetOwnerships
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetOwnerships = from o in pagedAndFilteredAssetOwnerships
                         join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetAssetOwnershipForViewDto() {
							AssetOwnership = new AssetOwnershipDto
							{
                                StartDate = o.StartDate,
                                FinishDate = o.FinishDate,
                                PercentageOwnership = o.PercentageOwnership,
                                Id = o.Id
							},
                         	AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                         	AssetOwnerName = s2 == null ? "" : s2.Name.ToString()
						};

            var totalCount = await filteredAssetOwnerships.CountAsync();

            return new PagedResultDto<GetAssetOwnershipForViewDto>(
                totalCount,
                await assetOwnerships.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetOwnershipForViewDto> GetAssetOwnershipForView(int id)
         {
            var assetOwnership = await _assetOwnershipRepository.GetAsync(id);

            var output = new GetAssetOwnershipForViewDto { AssetOwnership = ObjectMapper.Map<AssetOwnershipDto>(assetOwnership) };

		    if (output.AssetOwnership.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetOwnership.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

		    if (output.AssetOwnership.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.AssetOwnership.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships_Edit)]
		 public async Task<GetAssetOwnershipForEditOutput> GetAssetOwnershipForEdit(EntityDto input)
         {
            var assetOwnership = await _assetOwnershipRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetOwnershipForEditOutput {AssetOwnership = ObjectMapper.Map<CreateOrEditAssetOwnershipDto>(assetOwnership)};

		    if (output.AssetOwnership.AssetId != null)
            {
                var _lookupAsset = await _lookup_assetRepository.FirstOrDefaultAsync((int)output.AssetOwnership.AssetId);
                output.AssetReference = _lookupAsset.Reference.ToString();
            }

		    if (output.AssetOwnership.AssetOwnerId != null)
            {
                var _lookupAssetOwner = await _lookup_assetOwnerRepository.FirstOrDefaultAsync((int)output.AssetOwnership.AssetOwnerId);
                output.AssetOwnerName = _lookupAssetOwner.Name.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetOwnershipDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships_Create)]
		 protected virtual async Task Create(CreateOrEditAssetOwnershipDto input)
         {
            var assetOwnership = ObjectMapper.Map<AssetOwnership>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetOwnership.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetOwnershipRepository.InsertAsync(assetOwnership);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetOwnershipDto input)
         {
            var assetOwnership = await _assetOwnershipRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetOwnership);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetOwnershipRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetOwnershipsToExcel(GetAllAssetOwnershipsForExcelInput input)
         {
			
			var filteredAssetOwnerships = _assetOwnershipRepository.GetAll()
						.Include( e => e.AssetFk)
						.Include( e => e.AssetOwnerFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false )
						.WhereIf(input.MinStartDateFilter != null, e => e.StartDate >= input.MinStartDateFilter)
						.WhereIf(input.MaxStartDateFilter != null, e => e.StartDate <= input.MaxStartDateFilter)
						.WhereIf(input.MinFinishDateFilter != null, e => e.FinishDate >= input.MinFinishDateFilter)
						.WhereIf(input.MaxFinishDateFilter != null, e => e.FinishDate <= input.MaxFinishDateFilter)
						.WhereIf(input.MinPercentageOwnershipFilter != null, e => e.PercentageOwnership >= input.MinPercentageOwnershipFilter)
						.WhereIf(input.MaxPercentageOwnershipFilter != null, e => e.PercentageOwnership <= input.MaxPercentageOwnershipFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetReferenceFilter), e => e.AssetFk != null && e.AssetFk.Reference.ToLower() == input.AssetReferenceFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetOwnerNameFilter), e => e.AssetOwnerFk != null && e.AssetOwnerFk.Name.ToLower() == input.AssetOwnerNameFilter.ToLower().Trim());

			var query = (from o in filteredAssetOwnerships
                         join o1 in _lookup_assetRepository.GetAll() on o.AssetId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         join o2 in _lookup_assetOwnerRepository.GetAll() on o.AssetOwnerId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()
                         
                         select new GetAssetOwnershipForViewDto() { 
							AssetOwnership = new AssetOwnershipDto
							{
                                StartDate = o.StartDate,
                                FinishDate = o.FinishDate,
                                PercentageOwnership = o.PercentageOwnership,
                                Id = o.Id
							},
                         	AssetReference = s1 == null ? "" : s1.Reference.ToString(),
                         	AssetOwnerName = s2 == null ? "" : s2.Name.ToString()
						 });


            var assetOwnershipListDtos = await query.ToListAsync();

            return _assetOwnershipsExcelExporter.ExportToFile(assetOwnershipListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships)]
         public async Task<PagedResultDto<AssetOwnershipAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Reference.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetOwnershipAssetLookupTableDto>();
			foreach(var asset in assetList){
				lookupTableDtoList.Add(new AssetOwnershipAssetLookupTableDto
				{
					Id = asset.Id,
					DisplayName = asset.Reference?.ToString()
				});
			}

            return new PagedResultDto<AssetOwnershipAssetLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }

		[AbpAuthorize(AppPermissions.Pages_Main_AssetOwnerships)]
         public async Task<PagedResultDto<AssetOwnershipAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetOwnerRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Name.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetOwnerList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetOwnershipAssetOwnerLookupTableDto>();
			foreach(var assetOwner in assetOwnerList){
				lookupTableDtoList.Add(new AssetOwnershipAssetOwnerLookupTableDto
				{
					Id = assetOwner.Id,
					DisplayName = assetOwner.Name?.ToString()
				});
			}

            return new PagedResultDto<AssetOwnershipAssetOwnerLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}