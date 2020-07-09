using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface ICustomerInvoiceStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerInvoiceStatusForViewDto>> GetAll(GetAllCustomerInvoiceStatusesInput input);

        Task<GetCustomerInvoiceStatusForViewDto> GetCustomerInvoiceStatusForView(int id);

		Task<GetCustomerInvoiceStatusForEditOutput> GetCustomerInvoiceStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerInvoiceStatusDto input);

		Task Delete(EntityDto input);

		
    }
}