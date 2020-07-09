using System.Threading.Tasks;
using Abp.Application.Services;
using Ems.Install.Dto;

namespace Ems.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}