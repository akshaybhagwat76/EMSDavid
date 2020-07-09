using System.Threading.Tasks;

namespace Ems.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}