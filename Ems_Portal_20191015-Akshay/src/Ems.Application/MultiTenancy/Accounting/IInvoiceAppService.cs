using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Ems.MultiTenancy.Accounting.Dto;

namespace Ems.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
