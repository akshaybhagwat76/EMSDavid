using System.Threading.Tasks;
using Ems.Sessions.Dto;

namespace Ems.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
