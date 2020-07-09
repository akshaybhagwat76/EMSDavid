using System.Threading.Tasks;
using Abp.Application.Services;
using Ems.Configuration.Tenants.Dto;

namespace Ems.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
