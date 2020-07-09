

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Ems.Authorization.Exporting;
using Ems.Authorization.Dtos;
using Ems.Dto;
using Abp.Application.Services.Dto;
using Ems.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Ems.Authorization
{
	[AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions)]
    public class CrossTenantPermissionsAppService : EmsAppServiceBase, ICrossTenantPermissionsAppService
    {
		 private readonly IRepository<CrossTenantPermission> _crossTenantPermissionRepository;
		 private readonly ICrossTenantPermissionsExcelExporter _crossTenantPermissionsExcelExporter;
		 

		  public CrossTenantPermissionsAppService(IRepository<CrossTenantPermission> crossTenantPermissionRepository, ICrossTenantPermissionsExcelExporter crossTenantPermissionsExcelExporter ) 
		  {
			_crossTenantPermissionRepository = crossTenantPermissionRepository;
			_crossTenantPermissionsExcelExporter = crossTenantPermissionsExcelExporter;
			
		  }

		 public async Task<PagedResultDto<GetCrossTenantPermissionForViewDto>> GetAll(GetAllCrossTenantPermissionsInput input)
         {
			
			var filteredCrossTenantPermissions = _crossTenantPermissionRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EntityType.Contains(input.Filter) || e.Tenants.Contains(input.Filter) || e.TenantType.Contains(input.Filter))
						.WhereIf(input.MinTenantRefIdFilter != null, e => e.TenantRefId >= input.MinTenantRefIdFilter)
						.WhereIf(input.MaxTenantRefIdFilter != null, e => e.TenantRefId <= input.MaxTenantRefIdFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter),  e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TenantsFilter),  e => e.Tenants.ToLower() == input.TenantsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TenantTypeFilter),  e => e.TenantType.ToLower() == input.TenantTypeFilter.ToLower().Trim());

			var pagedAndFilteredCrossTenantPermissions = filteredCrossTenantPermissions
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var crossTenantPermissions = from o in pagedAndFilteredCrossTenantPermissions
                         select new GetCrossTenantPermissionForViewDto() {
							CrossTenantPermission = new CrossTenantPermissionDto
							{
                                TenantRefId = o.TenantRefId,
                                EntityType = o.EntityType,
                                Tenants = o.Tenants,
                                TenantType = o.TenantType,
                                Id = o.Id
							}
						};

            var totalCount = await filteredCrossTenantPermissions.CountAsync();

            return new PagedResultDto<GetCrossTenantPermissionForViewDto>(
                totalCount,
                await crossTenantPermissions.ToListAsync()
            );
         }
		 
		 public async Task<GetCrossTenantPermissionForViewDto> GetCrossTenantPermissionForView(int id)
         {
            var crossTenantPermission = await _crossTenantPermissionRepository.GetAsync(id);

            var output = new GetCrossTenantPermissionForViewDto { CrossTenantPermission = ObjectMapper.Map<CrossTenantPermissionDto>(crossTenantPermission) };
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions_Edit)]
		 public async Task<GetCrossTenantPermissionForEditOutput> GetCrossTenantPermissionForEdit(EntityDto input)
         {
            var crossTenantPermission = await _crossTenantPermissionRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetCrossTenantPermissionForEditOutput {CrossTenantPermission = ObjectMapper.Map<CreateOrEditCrossTenantPermissionDto>(crossTenantPermission)};
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditCrossTenantPermissionDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions_Create)]
		 protected virtual async Task Create(CreateOrEditCrossTenantPermissionDto input)
         {
            var crossTenantPermission = ObjectMapper.Map<CrossTenantPermission>(input);

			
			if (AbpSession.TenantId != null)
			{
				crossTenantPermission.TenantId = (int?) AbpSession.TenantId;
			}
		

            await _crossTenantPermissionRepository.InsertAsync(crossTenantPermission);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions_Edit)]
		 protected virtual async Task Update(CreateOrEditCrossTenantPermissionDto input)
         {
            var crossTenantPermission = await _crossTenantPermissionRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, crossTenantPermission);
         }

		 [AbpAuthorize(AppPermissions.Pages_Administration_CrossTenantPermissions_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _crossTenantPermissionRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetCrossTenantPermissionsToExcel(GetAllCrossTenantPermissionsForExcelInput input)
         {
			
			var filteredCrossTenantPermissions = _crossTenantPermissionRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.EntityType.Contains(input.Filter) || e.Tenants.Contains(input.Filter) || e.TenantType.Contains(input.Filter))
						.WhereIf(input.MinTenantRefIdFilter != null, e => e.TenantRefId >= input.MinTenantRefIdFilter)
						.WhereIf(input.MaxTenantRefIdFilter != null, e => e.TenantRefId <= input.MaxTenantRefIdFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.EntityTypeFilter),  e => e.EntityType.ToLower() == input.EntityTypeFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TenantsFilter),  e => e.Tenants.ToLower() == input.TenantsFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.TenantTypeFilter),  e => e.TenantType.ToLower() == input.TenantTypeFilter.ToLower().Trim());

			var query = (from o in filteredCrossTenantPermissions
                         select new GetCrossTenantPermissionForViewDto() { 
							CrossTenantPermission = new CrossTenantPermissionDto
							{
                                TenantRefId = o.TenantRefId,
                                EntityType = o.EntityType,
                                Tenants = o.Tenants,
                                TenantType = o.TenantType,
                                Id = o.Id
							}
						 });


            var crossTenantPermissionListDtos = await query.ToListAsync();

            return _crossTenantPermissionsExcelExporter.ExportToFile(crossTenantPermissionListDtos);
         }


    }
}