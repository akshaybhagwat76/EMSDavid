using Microsoft.Extensions.Configuration;

namespace Ems.Configuration
{
    public interface IAppConfigurationAccessor
    {
        IConfigurationRoot Configuration { get; }
    }
}
