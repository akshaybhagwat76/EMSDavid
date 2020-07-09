using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IWorkOrderPrioritiesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderPriorityForViewDto>> GetAll(GetAllWorkOrderPrioritiesInput input);

        Task<GetWorkOrderPriorityForViewDto> GetWorkOrderPriorityForView(int id);

		Task<GetWorkOrderPriorityForEditOutput> GetWorkOrderPriorityForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWorkOrderPriorityDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetWorkOrderPrioritiesToExcel(GetAllWorkOrderPrioritiesForExcelInput input);

		
    }
}