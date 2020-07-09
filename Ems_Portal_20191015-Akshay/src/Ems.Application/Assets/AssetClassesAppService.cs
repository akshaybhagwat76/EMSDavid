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
	[AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses)]
    public class AssetClassesAppService : EmsAppServiceBase, IAssetClassesAppService
    {
		 private readonly IRepository<AssetClass> _assetClassRepository;
		 private readonly IAssetClassesExcelExporter _assetClassesExcelExporter;
		 private readonly IRepository<AssetType,int> _lookup_assetTypeRepository;
		 

		  public AssetClassesAppService(IRepository<AssetClass> assetClassRepository, IAssetClassesExcelExporter assetClassesExcelExporter , IRepository<AssetType, int> lookup_assetTypeRepository) 
		  {
			_assetClassRepository = assetClassRepository;
			_assetClassesExcelExporter = assetClassesExcelExporter;
			_lookup_assetTypeRepository = lookup_assetTypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetAssetClassForViewDto>> GetAll(GetAllAssetClassesInput input)
         {
			
			var filteredAssetClasses = _assetClassRepository.GetAll()
						.Include( e => e.AssetTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Manufacturer.Contains(input.Filter) || e.Model.Contains(input.Filter) || e.Specification.Contains(input.Filter) || e.Class.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ClassFilter),  e => e.Class.ToLower() == input.ClassFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetTypeTypeFilter), e => e.AssetTypeFk != null && e.AssetTypeFk.Type.ToLower() == input.AssetTypeTypeFilter.ToLower().Trim());

			var pagedAndFilteredAssetClasses = filteredAssetClasses
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var assetClasses = from o in pagedAndFilteredAssetClasses
                         join o1 in _lookup_assetTypeRepository.GetAll() on o.AssetTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetAssetClassForViewDto() {
							AssetClass = new AssetClassDto
							{
                                Manufacturer = o.Manufacturer,
                                Model = o.Model,
                                Specification = o.Specification,
                                Class = o.Class,
                                Id = o.Id
							},
                         	AssetTypeType = s1 == null ? "" : s1.Type.ToString()
						};

            var totalCount = await filteredAssetClasses.CountAsync();

            return new PagedResultDto<GetAssetClassForViewDto>(
                totalCount,
                await assetClasses.ToListAsync()
            );
         }
		 
		 public async Task<GetAssetClassForViewDto> GetAssetClassForView(int id)
         {
            var assetClass = await _assetClassRepository.GetAsync(id);

            var output = new GetAssetClassForViewDto { AssetClass = ObjectMapper.Map<AssetClassDto>(assetClass) };

		    if (output.AssetClass != null)
            {
                var _lookupAssetType = await _lookup_assetTypeRepository.FirstOrDefaultAsync((int)output.AssetClass.AssetTypeId);
                output.AssetTypeType = _lookupAssetType.Type.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses_Edit)]
		 public async Task<GetAssetClassForEditOutput> GetAssetClassForEdit(EntityDto input)
         {
            var assetClass = await _assetClassRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetAssetClassForEditOutput {AssetClass = ObjectMapper.Map<CreateOrEditAssetClassDto>(assetClass)};

		    if (output.AssetClass != null)
            {
                var _lookupAssetType = await _lookup_assetTypeRepository.FirstOrDefaultAsync((int)output.AssetClass.AssetTypeId);
                output.AssetTypeType = _lookupAssetType.Type.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditAssetClassDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses_Create)]
		 protected virtual async Task Create(CreateOrEditAssetClassDto input)
         {
            var assetClass = ObjectMapper.Map<AssetClass>(input);

			
			if (AbpSession.TenantId != null)
			{
				assetClass.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _assetClassRepository.InsertAsync(assetClass);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses_Edit)]
		 protected virtual async Task Update(CreateOrEditAssetClassDto input)
         {
            var assetClass = await _assetClassRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, assetClass);
         }

		 [AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _assetClassRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetAssetClassesToExcel(GetAllAssetClassesForExcelInput input)
         {
			
			var filteredAssetClasses = _assetClassRepository.GetAll()
						.Include( e => e.AssetTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Manufacturer.Contains(input.Filter) || e.Model.Contains(input.Filter) || e.Specification.Contains(input.Filter) || e.Class.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.ClassFilter),  e => e.Class.ToLower() == input.ClassFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.AssetTypeTypeFilter), e => e.AssetTypeFk != null && e.AssetTypeFk.Type.ToLower() == input.AssetTypeTypeFilter.ToLower().Trim());

			var query = (from o in filteredAssetClasses
                         join o1 in _lookup_assetTypeRepository.GetAll() on o.AssetTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetAssetClassForViewDto() { 
							AssetClass = new AssetClassDto
							{
                                Manufacturer = o.Manufacturer,
                                Model = o.Model,
                                Specification = o.Specification,
                                Class = o.Class,
                                Id = o.Id
							},
                         	AssetTypeType = s1 == null ? "" : s1.Type.ToString()
						 });


            var assetClassListDtos = await query.ToListAsync();

            return _assetClassesExcelExporter.ExportToFile(assetClassListDtos);
         }



		[AbpAuthorize(AppPermissions.Pages_Configuration_AssetClasses)]
         public async Task<PagedResultDto<AssetClassAssetTypeLookupTableDto>> GetAllAssetTypeForLookupTable(GetAllForLookupTableInput input)
         {
             var query = _lookup_assetTypeRepository.GetAll().WhereIf(
                    !string.IsNullOrWhiteSpace(input.Filter),
                   e=> e.Type.ToString().Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var assetTypeList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<AssetClassAssetTypeLookupTableDto>();
			foreach(var assetType in assetTypeList){
				lookupTableDtoList.Add(new AssetClassAssetTypeLookupTableDto
				{
					Id = assetType.Id,
					DisplayName = assetType.Type?.ToString()
				});
			}

            return new PagedResultDto<AssetClassAssetTypeLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
         }
    }
}