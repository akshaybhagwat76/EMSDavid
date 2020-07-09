using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Authorization.Dtos;
using Ems.Dto;

namespace Ems.Authorization
{
    public interface ICrossTenantPermissionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCrossTenantPermissionForViewDto>> GetAll(GetAllCrossTenantPermissionsInput input);

        Task<GetCrossTenantPermissionForViewDto> GetCrossTenantPermissionForView(int id);

		Task<GetCrossTenantPermissionForEditOutput> GetCrossTenantPermissionForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCrossTenantPermissionDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetCrossTenantPermissionsToExcel(GetAllCrossTenantPermissionsForExcelInput input);

		
    }
}