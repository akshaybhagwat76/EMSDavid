using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetPartsForExcelInput
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public string SerialNumberFilter { get; set; }

		public DateTime? MaxInstallDateFilter { get; set; }
		public DateTime? MinInstallDateFilter { get; set; }

		public int InstalledFilter { get; set; }


		 public string AssetPartTypeTypeFilter { get; set; }

		 		 public string AssetPartNameFilter { get; set; }

		 		 public string AssetPartStatusStatusFilter { get; set; }

		 		 public string UsageMetricMetricFilter { get; set; }

		 		 public string AttachmentFilenameFilter { get; set; }

		 		 public string AssetReferenceFilter { get; set; }

		 		 public string ItemTypeTypeFilter { get; set; }

		 
    }
}