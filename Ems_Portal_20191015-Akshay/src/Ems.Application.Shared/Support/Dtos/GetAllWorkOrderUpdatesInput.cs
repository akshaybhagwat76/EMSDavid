using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderUpdatesInput : PagedAndSortedResultRequestDto
    {
        public int WorkOrderId { get; set; }

		public string Filter { get; set; }

        public string WorkOrderSubjectFilter { get; set; }

        public string ItemTypeTypeFilter { get; set; }
       
        //public DateTime? MaxUpdatedAtFilter { get; set; }

        //public DateTime? MinUpdatedAtFilter { get; set; }

        //public long? MaxUpdatedByUserIdFilter { get; set; }

        //public long? MinUpdatedByUserIdFilter { get; set; }

        //public string UomUnitOfMeasurementFilter { get; set; }
    }
}