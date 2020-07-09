using Abp.Application.Services.Dto;
using System;

namespace Ems.Quotations.Dtos
{
    public class GetAllRfqsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string TitleFilter { get; set; }

		public DateTime? MaxRequestDateFilter { get; set; }
		public DateTime? MinRequestDateFilter { get; set; }

		public DateTime? MaxRequiredByFilter { get; set; }
		public DateTime? MinRequiredByFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public string RequirementsFilter { get; set; }


		 public string RfqTypeTypeFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 		 public string AssetClassClassFilter { get; set; }

		 		 public string IncidentDescriptionFilter { get; set; }

		 		 public string VendorNameFilter { get; set; }

		 		 public string UserNameFilter { get; set; }

		 
    }
}