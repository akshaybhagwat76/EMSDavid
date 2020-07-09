

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
	[AbpAuthorize(AppPermissions.Pages_Configuration_AssetStatuses)]
    public class AssetStatusesAppService : EmsAppServiceBase, IAssetStatusesAppService
    {
		 private readonly IRepository<AssetStatus> _assetStatusRepository;
		 

		  public AssetStatusesAppService(IRepository<AssetStatus> assetStatusRepository ) 
		  {
			_assetStatusRepository = assetStatusRepository;
			
		  }

		 public async Task<PagedResultDto<GetAssetStatusForViewDto>> GetAll(GetAllAssetStatusesInput input)
         {
			
			var filteredAssetStatuses = _assetStatusRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Status.Contains(input.Filter) || e.Description.Contains(input.Filter));

			var pagedAndFilteredAssetStatuses = filteredAssetStatuses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetStatuses = from o in pagedAndFilteredAssetStatuses
                         select new GetAssetStatusForViewDto() {
							AssetStatus = new AssetStatusDto
							{
                                Status = o.Status,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAssetStatuses.CountAsync();

            return new PagedResultDto<GetAssetStatusForViewDto>(
                totalCount,
                await assetStatuses.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetStatusForViewDto> GetAssetStatusForView(int id)
         {
            var assetStatus = await _assetStatusRepository.GetAsync(id);

            var output = new GetAssetStatusForViewDto { AssetStatus = ObjectMapper.Map<AssetStatusDto>(assetStatus) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetStatuses_Edit)]
		 public async Task<GetAssetStatusForEditOutput> GetAssetStatusForEdit(EntityDto input)
         {
            var assetStatus = await _assetStatusRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetStatusForEditOutput {AssetStatus = ObjectMapper.Map<CreateOrEditAssetStatusDto>(assetStatus)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetStatusDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetStatuses_Create)]
		 protected virtual async Task Create(CreateOrEditAssetStatusDto input)
         {
            var assetStatus = ObjectMapper.Map<AssetStatus>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetStatus.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetStatusRepository.InsertAsync(assetStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetStatuses_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetStatusDto input)
         {
            var assetStatus = await _assetStatusRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetStatus);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetStatuses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetStatusRepository.DeleteAsync(input.Id);
         } 
    }
}