

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Assets.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Assets
{
	[AbpAuthorize(AppPermissions.Pages_Configuration_AssetTypes)]
    public class AssetTypesAppService : EmsAppServiceBase, IAssetTypesAppService
    {
		 private readonly IRepository<AssetType> _assetTypeRepository;
		 

		  public AssetTypesAppService(IRepository<AssetType> assetTypeRepository ) 
		  {
			_assetTypeRepository = assetTypeRepository;
			
		  }

		 public async Task<PagedResultDto<GetAssetTypeForViewDto>> GetAll(GetAllAssetTypesInput input)
         {
			
			var filteredAssetTypes = _assetTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Code.Contains(input.Filter) || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredAssetTypes = filteredAssetTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetTypes = from o in pagedAndFilteredAssetTypes
                         select new GetAssetTypeForViewDto() {
							AssetType = new AssetTypeDto
							{
                                Code = o.Code,
                                Type = o.Type,
                                Description = o.Description,
                                Sort = o.Sort,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAssetTypes.CountAsync();

            return new PagedResultDto<GetAssetTypeForViewDto>(
                totalCount,
                await assetTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetTypeForViewDto> GetAssetTypeForView(int id)
         {
            var assetType = await _assetTypeRepository.GetAsync(id);

            var output = new GetAssetTypeForViewDto { AssetType = ObjectMapper.Map<AssetTypeDto>(assetType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetTypes_Edit)]
		 public async Task<GetAssetTypeForEditOutput> GetAssetTypeForEdit(EntityDto input)
         {
            var assetType = await _assetTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetTypeForEditOutput {AssetType = ObjectMapper.Map<CreateOrEditAssetTypeDto>(assetType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetTypes_Create)]
		 protected virtual async Task Create(CreateOrEditAssetTypeDto input)
         {
            var assetType = ObjectMapper.Map<AssetType>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetTypeRepository.InsertAsync(assetType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetTypeDto input)
         {
            var assetType = await _assetTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetType);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetTypeRepository.DeleteAsync(input.Id);
         } 
    }
}