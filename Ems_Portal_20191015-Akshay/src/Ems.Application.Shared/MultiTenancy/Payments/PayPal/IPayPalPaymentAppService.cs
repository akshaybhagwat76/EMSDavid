using System.Threading.Tasks;
using Abp.Application.Services;
using Ems.MultiTenancy.Payments.PayPal.Dto;

namespace Ems.MultiTenancy.Payments.PayPal
{
    public interface IPayPalPaymentAppService : IApplicationService
    {
        Task ConfirmPayment(long paymentId, string paypalOrderId);

        PayPalConfigurationDto GetConfiguration();
    }
}
