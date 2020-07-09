

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
	[AbpAuthorize(AppPermissions.Pages_Main_AssetPartStatuses)]
    public class AssetPartStatusesAppService : EmsAppServiceBase, IAssetPartStatusesAppService
    {
		 private readonly IRepository<AssetPartStatus> _assetPartStatusRepository;
		 

		  public AssetPartStatusesAppService(IRepository<AssetPartStatus> assetPartStatusRepository ) 
		  {
			_assetPartStatusRepository = assetPartStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetAssetPartStatusForViewDto>> GetAll(GetAllAssetPartStatusesInput input)
         {
			
			var filteredAssetPartStatuses = _assetPartStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredAssetPartStatuses = filteredAssetPartStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetPartStatuses = from o in pagedAndFilteredAssetPartStatuses
                         select new GetAssetPartStatusForViewDto() {
							AssetPartStatus = new AssetPartStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAssetPartStatuses.CountAsync();

            return new PagedResultDto<GetAssetPartStatusForViewDto>(
                totalCount,
                await assetPartStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetPartStatusForViewDto> GetAssetPartStatusForView(int id)
         {
            var assetPartStatus = await _assetPartStatusRepository.GetAsync(id);

            var output = new GetAssetPartStatusForViewDto { AssetPartStatus = ObjectMapper.Map<AssetPartStatusDto>(assetPartStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Main_AssetPartStatuses_Edit)]
		 public async Task<GetAssetPartStatusForEditOutput> GetAssetPartStatusForEdit(EntityDto input)
         {
            var assetPartStatus = await _assetPartStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetPartStatusForEditOutput {AssetPartStatus = ObjectMapper.Map<CreateOrEditAssetPartStatusDto>(assetPartStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetPartStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetPartStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditAssetPartStatusDto input)
         {
            var assetPartStatus = ObjectMapper.Map<AssetPartStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetPartStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetPartStatusRepository.InsertAsync(assetPartStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetPartStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetPartStatusDto input)
         {
            var assetPartStatus = await _assetPartStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetPartStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Main_AssetPartStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetPartStatusRepository.DeleteAsync(input.Id);
         } 
    }
}