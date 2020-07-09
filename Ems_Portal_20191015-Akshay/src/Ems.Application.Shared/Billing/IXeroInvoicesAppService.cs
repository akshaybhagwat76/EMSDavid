using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;


namespace Ems.Billing
{
    public interface IXeroInvoicesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetXeroInvoiceForViewDto>> GetAll(GetAllXeroInvoicesInput input);

        Task<GetXeroInvoiceForViewDto> GetXeroInvoiceForView(int id);

		Task<GetXeroInvoiceForEditOutput> GetXeroInvoiceForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditXeroInvoiceDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetXeroInvoicesToExcel(GetAllXeroInvoicesForExcelInput input);

		
		Task<PagedResultDto<XeroInvoiceCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input);
		
    }
}