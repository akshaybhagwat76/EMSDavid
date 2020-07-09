using System.Threading.Tasks;
using Abp.Application.Services;
using Ems.Sessions.Dto;

namespace Ems.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}
