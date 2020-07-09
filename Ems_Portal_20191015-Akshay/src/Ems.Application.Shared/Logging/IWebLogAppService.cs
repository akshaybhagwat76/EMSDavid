using Abp.Application.Services;
using Ems.Dto;
using Ems.Logging.Dto;

namespace Ems.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
