using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface ILeaseAgreementsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetLeaseAgreementForViewDto>> GetAll(GetAllLeaseAgreementsInput input);

        Task<GetLeaseAgreementForViewDto> GetLeaseAgreementForView(int id);

		Task<GetLeaseAgreementForEditOutput> GetLeaseAgreementForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditLeaseAgreementDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetLeaseAgreementsToExcel(GetAllLeaseAgreementsForExcelInput input);

		
		Task<PagedResultDto<LeaseAgreementContactLookupTableDto>> GetAllContactForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<LeaseAgreementAssetOwnerLookupTableDto>> GetAllAssetOwnerForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);
		
		Task<PagedResultDto<LeaseAgreementCustomerLookupTableDto>> GetAllCustomerForLookupTable(Support.Dtos.GetAllUsingIdForLookupTableInput input);

        LeaseAgreementCustomerAndAssetOwnerDto GetCustomerAndAssetOwnerInfo(int contactId);
    }
}