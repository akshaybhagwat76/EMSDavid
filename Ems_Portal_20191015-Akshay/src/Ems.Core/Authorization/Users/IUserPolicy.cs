using System.Threading.Tasks;
using Abp.Domain.Policies;

namespace Ems.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}
