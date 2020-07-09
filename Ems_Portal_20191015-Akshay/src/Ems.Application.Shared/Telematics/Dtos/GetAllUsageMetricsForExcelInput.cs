
namespace Ems.Telematics.Dtos
{
    public class GetAllUsageMetricsForExcelInput
    {
        public string Filter { get; set; }

        public string MetricFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public string LeaseItemItemFilter { get; set; }

        public string AssetReferenceFilter { get; set; }

        public string UomUnitOfMeasurementFilter { get; set; }
    }
}