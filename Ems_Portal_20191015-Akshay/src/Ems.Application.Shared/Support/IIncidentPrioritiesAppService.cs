using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IIncidentPrioritiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIncidentPriorityForViewDto>> GetAll(GetAllIncidentPrioritiesInput input);

        Task<GetIncidentPriorityForViewDto> GetIncidentPriorityForView(int id);

		Task<GetIncidentPriorityForEditOutput> GetIncidentPriorityForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIncidentPriorityDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetIncidentPrioritiesToExcel(GetAllIncidentPrioritiesForExcelInput input);

		
    }
}