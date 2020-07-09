
using System;
using Abp.Application.Services.Dto;

namespace Ems.Telematics.Dtos
{
    public class UsageMetricRecordDto : EntityDto
    {
        public string Reference { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public decimal? UnitsConsumed { get; set; }

        public int UsageMetricId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}