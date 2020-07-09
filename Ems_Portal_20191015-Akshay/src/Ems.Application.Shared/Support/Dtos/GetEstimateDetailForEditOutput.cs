namespace Ems.Support.Dtos
{
    public class GetEstimateDetailForEditOutput
    {
        public CreateOrEditEstimateDetailDto EstimateDetail { get; set; }

        public string ItemTypeType { get; set; }

        public string EstimateTitle { get; set; }

        public string UomUnitOfMeasurement { get; set; }

        public string ActionWorkOrderAction { get; set; }
    }
}
