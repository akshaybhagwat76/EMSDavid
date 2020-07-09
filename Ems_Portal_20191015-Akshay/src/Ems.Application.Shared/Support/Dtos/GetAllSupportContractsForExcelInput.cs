using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllSupportContractsForExcelInput
    {
		public string Filter { get; set; }

		public string TitleFilter { get; set; }

		public string ReferenceFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public DateTime? MaxStartDateFilter { get; set; }
		public DateTime? MinStartDateFilter { get; set; }

		public DateTime? MaxEndDateFilter { get; set; }
		public DateTime? MinEndDateFilter { get; set; }

		//public string AttachmentsFilter { get; set; }

		public int IsRFQTemplateFilter { get; set; }

		public int IsAcknowledgedFilter { get; set; }

		public string AcknowledgedByFilter { get; set; }

		public DateTime? MaxAcknowledgedAtFilter { get; set; }
		public DateTime? MinAcknowledgedAtFilter { get; set; }


		 public string VendorNameFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 
    }
}