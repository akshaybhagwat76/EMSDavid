using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;

namespace Ems.Billing
{
    public interface ICustomerInvoiceDetailsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerInvoiceDetailForViewDto>> GetAll(GetAllCustomerInvoiceDetailsInput input);

        Task<GetCustomerInvoiceDetailForViewDto> GetCustomerInvoiceDetailForView(int id);

		Task<GetCustomerInvoiceDetailForEditOutput> GetCustomerInvoiceDetailForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerInvoiceDetailDto input);

		Task Delete(EntityDto input);

		
		Task<PagedResultDto<CustomerInvoiceDetailCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<CustomerInvoiceLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input);
    }
}