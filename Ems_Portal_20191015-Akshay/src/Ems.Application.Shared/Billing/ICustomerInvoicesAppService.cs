using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Billing.Dtos;
using Ems.Dto;
using Ems.XeroModel;

namespace Ems.Billing
{
    public interface ICustomerInvoicesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetCustomerInvoiceForViewDto>> GetAll(GetAllCustomerInvoicesInput input);

        Task<GetCustomerInvoiceForViewDto> GetCustomerInvoiceForView(int id, PagedAndSortedResultRequestDto input);

		Task<GetCustomerInvoiceForEditOutput> GetCustomerInvoiceForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditCustomerInvoiceDto input);

        Task<GetCustomerInvoiceForViewDto> GetCustomerInvoiceForPDF(int id);

        Task<XeroResponse> XeroCommunication(int id, string flag);

        Task<string> RefereshXeroInvoices();

        Task<bool> GetCallback(string code, string state);

        Task Delete(EntityDto input);

		Task<FileDto> GetCustomerInvoicesToExcel(GetAllCustomerInvoicesForExcelInput input);

        //void UpdateCustomerInvoicePrices(int customerInvoiceId);

        //void CloneAllEstimateDetails(int customerInvoiceId, int estimateId);

        Task<PagedResultDto<CustomerInvoiceCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForInvoiceLookupTableInput input);

        Task<PagedResultDto<CustomerInvoiceCurrencyLookupTableDto>> GetAllCurrencyForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<CustomerInvoiceBillingRuleLookupTableDto>> GetAllBillingRuleForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<CustomerInvoiceBillingEventLookupTableDto>> GetAllBillingEventForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<CustomerInvoiceInvoiceStatusLookupTableDto>> GetAllInvoiceStatusForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<CustomerInvoiceWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<CustomerInvoiceEstimateLookupTableDto>> GetAllEstimateForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        Task<CustomerInvoiceWorkOrderFkListDto> GetWorkOrderFkData(int workOrderId);

        CustomerInvoiceEstimateFkListDto GetEstimateFkData(int estimateId);

        Task<GetCustomerInvoiceEstimateForViewDto> GetCustomerInvoiceEstimateForView(int customerInvoiceId, PagedAndSortedResultRequestDto input);

        Task<GetCustomerInvoiceWorkOrderForViewDto> GetCustomerInvoiceWorkOrderForView(int customerInvoiceId, PagedAndSortedResultRequestDto input);
    }
}