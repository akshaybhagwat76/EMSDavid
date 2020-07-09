using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Assets.Dtos;
using Ems.Dto;

namespace Ems.Assets
{
    public interface ILeaseItemsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetLeaseItemForViewDto>> GetAll(GetAllLeaseItemsInput input);

        Task<GetLeaseItemForViewDto> GetLeaseItemForView(int id);

		Task<GetLeaseItemForEditOutput> GetLeaseItemForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditLeaseItemDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetLeaseItemsToExcel(GetAllLeaseItemsForExcelInput input);

		
		Task<PagedResultDto<LeaseItemAssetClassLookupTableDto>> GetAllAssetClassForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<LeaseItemAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<LeaseItemLeaseAgreementLookupTableDto>> GetAllLeaseAgreementForLookupTable(GetAllForLookupTableInput input);
		
    }
}