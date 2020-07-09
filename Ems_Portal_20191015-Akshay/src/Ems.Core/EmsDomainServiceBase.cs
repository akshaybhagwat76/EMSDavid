using Abp.Domain.Services;

namespace Ems
{
    public abstract class EmsDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */

        protected EmsDomainServiceBase()
        {
            LocalizationSourceName = EmsConsts.LocalizationSourceName;
        }
    }
}
