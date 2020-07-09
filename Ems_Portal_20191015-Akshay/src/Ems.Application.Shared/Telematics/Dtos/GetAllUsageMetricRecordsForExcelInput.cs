using Abp.Application.Services.Dto;
using System;

namespace Ems.Telematics.Dtos
{
    public class GetAllUsageMetricRecordsForExcelInput
    {
        public string Filter { get; set; }

        public string ReferenceFilter { get; set; }

        public DateTime? MaxStartTimeFilter { get; set; }
        public DateTime? MinStartTimeFilter { get; set; }

        public DateTime? MaxEndTimeFilter { get; set; }
        public DateTime? MinEndTimeFilter { get; set; }

        public decimal? MaxUnitsConsumedFilter { get; set; }
        public decimal? MinUnitsConsumedFilter { get; set; }

        public string UsageMetricMetricFilter { get; set; }
    }
}