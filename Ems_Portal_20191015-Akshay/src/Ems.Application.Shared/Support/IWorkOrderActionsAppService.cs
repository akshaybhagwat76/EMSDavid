using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IWorkOrderActionsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderActionForViewDto>> GetAll(GetAllWorkOrderActionsInput input);

        Task<GetWorkOrderActionForViewDto> GetWorkOrderActionForView(int id);

		Task<GetWorkOrderActionForEditOutput> GetWorkOrderActionForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWorkOrderActionDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetWorkOrderActionsToExcel(GetAllWorkOrderActionsForExcelInput input);

		
    }
}