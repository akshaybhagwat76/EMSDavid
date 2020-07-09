using System.Collections.Generic;
using Ems.Authorization.Dtos;
using Ems.Dto;

namespace Ems.Authorization.Exporting
{
    public interface ICrossTenantPermissionsExcelExporter
    {
        FileDto ExportToFile(List<GetCrossTenantPermissionForViewDto> crossTenantPermissions);
    }
}