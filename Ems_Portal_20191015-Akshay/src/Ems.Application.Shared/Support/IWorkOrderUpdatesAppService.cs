using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IWorkOrderUpdatesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderUpdateForViewDto>> GetAll(GetAllWorkOrderUpdatesInput input);

        Task<GetWorkOrderUpdateForViewDto> GetWorkOrderUpdateForView(int id);

		Task<GetWorkOrderUpdateForEditOutput> GetWorkOrderUpdateForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWorkOrderUpdateDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetWorkOrderUpdatesToExcel(GetAllWorkOrderUpdatesForExcelInput input);

		
		Task<PagedResultDto<WorkOrderUpdateWorkOrderLookupTableDto>> GetAllWorkOrderForLookupTable(GetAllForLookupTableInput input);
		
    }
}