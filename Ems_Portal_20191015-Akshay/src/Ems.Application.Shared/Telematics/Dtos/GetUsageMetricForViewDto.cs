using System;

namespace Ems.Telematics.Dtos
{
    public class GetUsageMetricForViewDto
    {
		public UsageMetricDto UsageMetric { get; set; }

		public string LeaseItemItem { get; set;}

        public string AssetReference { get; set; }

        public string UomUnitOfMeasurement { get; set; }

        public bool NeedRecordUpdate { get; set; }
    }
}