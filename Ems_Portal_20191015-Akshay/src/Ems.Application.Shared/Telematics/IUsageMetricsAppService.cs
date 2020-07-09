using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Telematics.Dtos;
using Ems.Dto;

namespace Ems.Telematics
{
    public interface IUsageMetricsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUsageMetricForViewDto>> GetAll(GetAllUsageMetricsInput input);

        Task<GetUsageMetricForViewDto> GetUsageMetricForView(int id);

		Task<GetUsageMetricForEditOutput> GetUsageMetricForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUsageMetricDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetUsageMetricsToExcel(GetAllUsageMetricsForExcelInput input);

		
		Task<PagedResultDto<UsageMetricLeaseItemLookupTableDto>> GetAllLeaseItemForLookupTable(GetAllForLookupTableInput input);
		
    }
}