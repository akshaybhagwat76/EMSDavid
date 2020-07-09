using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrdersInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string Loc8GUIDFilter { get; set; }

        public string SubjectFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public string LocationFilter { get; set; }

        public DateTime? MaxStartDateFilter { get; set; }
        public DateTime? MinStartDateFilter { get; set; }

        public DateTime? MaxEndDateFilter { get; set; }
        public DateTime? MinEndDateFilter { get; set; }

        public string RemarksFilter { get; set; }

        public string WorkOrderPriorityPriorityFilter { get; set; }

        public string WorkOrderTypeTypeFilter { get; set; }

        public string VendorNameFilter { get; set; }

        public string IncidentDescriptionFilter { get; set; }

        public string SupportItemDescriptionFilter { get; set; }

        public string UserNameFilter { get; set; }

        public string CustomerNameFilter { get; set; }

        public string AssetOwnershipAssetFkFilter { get; set; }

        public string WorkOrderStatusStatusFilter { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsPreventative { get; set; }
    }
}