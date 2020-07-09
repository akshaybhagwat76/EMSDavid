using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IWorkOrderStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetWorkOrderStatusForViewDto>> GetAll(GetAllWorkOrderStatusesInput input);

        Task<GetWorkOrderStatusForViewDto> GetWorkOrderStatusForView(int id);

		Task<GetWorkOrderStatusForEditOutput> GetWorkOrderStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditWorkOrderStatusDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetWorkOrderStatusesToExcel(GetAllWorkOrderStatusesForExcelInput input);

		
    }
}