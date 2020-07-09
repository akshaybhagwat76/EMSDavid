using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderLoc8GUIDsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string Loc8GUIDFilter { get; set; }

		public string SubjectFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public string LocationFilter { get; set; }

		public DateTime? MaxStartDateFilter { get; set; }
		public DateTime? MinStartDateFilter { get; set; }

		public string EndDateFilter { get; set; }

		public DateTime? MaxDateRaisedFilter { get; set; }
		public DateTime? MinDateRaisedFilter { get; set; }

		public string RemarkFilter { get; set; }


		 public string WorkOrderTypeTypeFilter { get; set; }

		 		 public string VendorNameFilter { get; set; }

		 		 public string SupportContractTitleFilter { get; set; }

		 		 public string IncidentDescriptionFilter { get; set; }

		 		 public string SupportItemDescriptionFilter { get; set; }

		 		 public string UserNameFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 		 public string RfqTitleFilter { get; set; }

		 
    }
}