using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Ems.Telematics.Dtos;
using Ems.Dto;

namespace Ems.Telematics
{
    public interface IUsageMetricRecordsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetUsageMetricRecordForViewDto>> GetAll(GetAllUsageMetricRecordsInput input);

        Task<GetUsageMetricRecordForViewDto> GetUsageMetricRecordForView(int id);

		Task<GetUsageMetricRecordForEditOutput> GetUsageMetricRecordForEdit(EntityDto input);

		Task CreateOrEdit(CreateOrEditUsageMetricRecordDto input);

		Task Delete(EntityDto input);

		Task<FileDto> GetUsageMetricRecordsToExcel(GetAllUsageMetricRecordsForExcelInput input);

		
		Task<PagedResultDto<UsageMetricRecordUsageMetricLookupTableDto>> GetAllUsageMetricForLookupTable(GetAllForLookupTableInput input);

        Task<PagedResultDto<GetUsageMetricRecordForViewDto>> GetAllByUsageMetric(GetAllRecordsByUsageMetricInput input);

        Task<GetUsageMetricRecordForViewDto> GetByUsageMetric(int usageMetricId);
    }
}