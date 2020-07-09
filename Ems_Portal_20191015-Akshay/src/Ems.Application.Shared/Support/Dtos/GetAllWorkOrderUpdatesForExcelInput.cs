namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderUpdatesForExcelInput
    {
        public string Filter { get; set; }

        public string WorkOrderSubjectFilter { get; set; }

        public string ItemTypeTypeFilter { get; set; }

        //public DateTime? MaxUpdatedAtFilter { get; set; }
        //public DateTime? MinUpdatedAtFilter { get; set; }

        //public string UpdateFilter { get; set; } 

        //public long? MaxUpdatedByUserIdFilter { get; set; }
        //public long? MinUpdatedByUserIdFilter { get; set; }
    }
}