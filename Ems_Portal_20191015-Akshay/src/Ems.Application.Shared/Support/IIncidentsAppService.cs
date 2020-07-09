using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IIncidentsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIncidentForViewDto>> GetAll(GetAllIncidentsInput input);

        Task<GetIncidentForViewDto> GetIncidentForView(int id, PagedAndSortedResultRequestDto input);

		Task<GetIncidentForEditOutput> GetIncidentForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIncidentDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetIncidentsToExcel(GetAllIncidentsForExcelInput input);

		
		Task<PagedResultDto<IncidentIncidentPriorityLookupTableDto>> GetAllIncidentPriorityForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<IncidentIncidentStatusLookupTableDto>> GetAllIncidentStatusForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<IncidentCustomerLookupTableDto>> GetAllCustomerForLookupTable(GetAllCustomersForLookupTableInput input);
		
		Task<PagedResultDto<IncidentAssetLookupTableDto>> GetAllAssetForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<IncidentSupportItemLookupTableDto>> GetAllSupportItemForLookupTable(GetAllSupportItemsForLookupTableInput input);

        Task<PagedResultDto<IncidentIncidentTypeLookupTableDto>> GetAllIncidentTypeForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<IncidentUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
    }
}