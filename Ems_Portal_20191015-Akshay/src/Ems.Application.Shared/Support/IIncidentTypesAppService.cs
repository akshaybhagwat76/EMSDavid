using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IIncidentTypesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetIncidentTypeForViewDto>> GetAll(GetAllIncidentTypesInput input);

        Task<GetIncidentTypeForViewDto> GetIncidentTypeForView(int id);

		Task<GetIncidentTypeForEditOutput> GetIncidentTypeForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditIncidentTypeDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetIncidentTypesToExcel(GetAllIncidentTypesForExcelInput input);

		
    }
}