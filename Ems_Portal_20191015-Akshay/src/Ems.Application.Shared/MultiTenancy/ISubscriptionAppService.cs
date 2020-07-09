using System.Threading.Tasks;
using Abp.Application.Services;

namespace Ems.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
    }
}
