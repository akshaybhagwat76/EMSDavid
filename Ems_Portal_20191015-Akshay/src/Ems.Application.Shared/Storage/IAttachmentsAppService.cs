using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Storage.Dtos;
using Ems.Dto;

namespace Ems.Storage
{
    public interface IAttachmentsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetAttachmentForViewDto>> GetAll(GetAllAttachmentsInput input);

        Task<GetAttachmentForViewDto> GetAttachmentForView(int id);

		Task<GetAttachmentForEditOutput> GetAttachmentForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditAttachmentDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetAttachmentsToExcel(GetAllAttachmentsForExcelInput input);

		
		Task<PagedResultDto<AttachmentAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentQuotationLookupTableDto>> GetAllQuotationForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentSupportContractLookupTableDto>> GetAllSupportContractForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<AttachmentCustomerInvoiceLookupTableDto>> GetAllCustomerInvoiceForLookupTable(GetAllForLookupTableInput input);
		
    }
}