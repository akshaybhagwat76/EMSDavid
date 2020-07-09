using System.Collections.Generic;
using Ems.Telematics.Dtos;
using Ems.Dto;

namespace Ems.Telematics.Exporting
{
    public interface IUsageMetricsExcelExporter
    {
        FileDto ExportToFile(List<GetUsageMetricForViewDto> usageMetrics);
    }
}