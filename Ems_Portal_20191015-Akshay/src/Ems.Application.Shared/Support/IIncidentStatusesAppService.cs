using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IIncidentStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIncidentStatusForViewDto>> GetAll(GetAllIncidentStatusesInput input);

        Task<GetIncidentStatusForViewDto> GetIncidentStatusForView(int id);

		Task<GetIncidentStatusForEditOutput> GetIncidentStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIncidentStatusDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetIncidentStatusesToExcel(GetAllIncidentStatusesForExcelInput input);

		
    }
}