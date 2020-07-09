using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Authorization.Permissions.Dto;

namespace Ems.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
