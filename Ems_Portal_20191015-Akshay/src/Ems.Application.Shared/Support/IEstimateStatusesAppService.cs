using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Support.Dtos;
using Ems.Dto;

namespace Ems.Support
{
    public interface IEstimateStatusesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetEstimateStatusForViewDto>> GetAll(GetAllEstimateStatusesInput input);

        Task<GetEstimateStatusForViewDto> GetEstimateStatusForView(int id);

		Task<GetEstimateStatusForEditOutput> GetEstimateStatusForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditEstimateStatusDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetEstimateStatusesToExcel(GetAllEstimateStatusesForExcelInput input);

		
    }
}