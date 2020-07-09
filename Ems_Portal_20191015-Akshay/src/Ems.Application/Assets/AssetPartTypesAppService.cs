

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
	[AbpAuthorize(AppPermissions.Pages_AssetPartTypes)]
    public class AssetPartTypesAppService : EmsAppServiceBase, IAssetPartTypesAppService
    {
		 private readonly IRepository<AssetPartType> _assetPartTypeRepository;
		 private readonly IAssetPartTypesExcelExporter _assetPartTypesExcelExporter;
		 

		  public AssetPartTypesAppService(IRepository<AssetPartType> assetPartTypeRepository, IAssetPartTypesExcelExporter assetPartTypesExcelExporter ) 
		  {
			_assetPartTypeRepository = assetPartTypeRepository;
			_assetPartTypesExcelExporter = assetPartTypesExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetAssetPartTypeForViewDto>> GetAll(GetAllAssetPartTypesInput input)
         {
			
			var filteredAssetPartTypes = _assetPartTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter),  e => e.Type == input.TypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter);

			var pagedAndFilteredAssetPartTypes = filteredAssetPartTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetPartTypes = from o in pagedAndFilteredAssetPartTypes
                         select new GetAssetPartTypeForViewDto() {
							AssetPartType = new AssetPartTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						};

            var totalCount = await filteredAssetPartTypes.CountAsync();

            return new PagedResultDto<GetAssetPartTypeForViewDto>(
                totalCount,
                await assetPartTypes.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetPartTypeForViewDto> GetAssetPartTypeForView(int id)
         {
            var assetPartType = await _assetPartTypeRepository.GetAsync(id);

            var output = new GetAssetPartTypeForViewDto { AssetPartType = ObjectMapper.Map<AssetPartTypeDto>(assetPartType) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_AssetPartTypes_Edit)]
		 public async Task<GetAssetPartTypeForEditOutput> GetAssetPartTypeForEdit(EntityDto input)
         {
            var assetPartType = await _assetPartTypeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetPartTypeForEditOutput {AssetPartType = ObjectMapper.Map<CreateOrEditAssetPartTypeDto>(assetPartType)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetPartTypeDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_AssetPartTypes_Create)]
		 protected virtual async Task Create(CreateOrEditAssetPartTypeDto input)
         {
            var assetPartType = ObjectMapper.Map<AssetPartType>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetPartType.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetPartTypeRepository.InsertAsync(assetPartType);
         }

		 [AbpAuthorize(AppPermissions.Pages_AssetPartTypes_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetPartTypeDto input)
         {
            var assetPartType = await _assetPartTypeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetPartType);
         }

		 [AbpAuthorize(AppPermissions.Pages_AssetPartTypes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetPartTypeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetPartTypesToExcel(GetAllAssetPartTypesForExcelInput input)
         {
			
			var filteredAssetPartTypes = _assetPartTypeRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Type.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.TypeFilter),  e => e.Type == input.TypeFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description == input.DescriptionFilter);

			var query = (from o in filteredAssetPartTypes
                         select new GetAssetPartTypeForViewDto() { 
							AssetPartType = new AssetPartTypeDto
							{
                                Type = o.Type,
                                Description = o.Description,
                                Id = o.Id
							}
						 });


            var assetPartTypeListDtos = await query.ToListAsync();

            return _assetPartTypesExcelExporter.ExportToFile(assetPartTypeListDtos);
         }


    }
}