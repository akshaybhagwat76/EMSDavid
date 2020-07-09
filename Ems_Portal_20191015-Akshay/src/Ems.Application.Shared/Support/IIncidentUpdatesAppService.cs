using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IIncidentUpdatesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIncidentUpdateForViewDto>> GetAll(GetAllIncidentUpdatesInput input);

        Task<GetIncidentUpdateForViewDto> GetIncidentUpdateForView(int id);

		Task<GetIncidentUpdateForEditOutput> GetIncidentUpdateForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIncidentUpdateDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetIncidentUpdatesToExcel(GetAllIncidentUpdatesForExcelInput input);

		
		Task<PagedResultDto<IncidentUpdateUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input);
		
		Task<PagedResultDto<IncidentUpdateIncidentLookupTableDto>> GetAllIncidentForLookupTable(GetAllForLookupTableInput input);
		
    }
}