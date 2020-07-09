using System.Collections.Generic;
using Ems.Telematics.Dtos;
using Ems.Dto;

namespace Ems.Telematics.Exporting
{
    public interface IUsageMetricRecordsExcelExporter
    {
        FileDto ExportToFile(List<GetUsageMetricRecordForViewDto> usageMetricRecords);
    }
}