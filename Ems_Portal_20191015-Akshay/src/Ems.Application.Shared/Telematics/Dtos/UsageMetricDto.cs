
using System;
using Abp.Application.Services.Dto;

namespace Ems.Telematics.Dtos
{
    public class UsageMetricDto : EntityDto
    {
        public string Metric { get; set; }

        public string Description { get; set; }

        public int? LeaseItemId { get; set; }

        public int AssetId { get; set; }

        public int? UomId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}