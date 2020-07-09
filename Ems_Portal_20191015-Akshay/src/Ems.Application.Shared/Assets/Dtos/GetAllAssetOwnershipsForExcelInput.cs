using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetOwnershipsForExcelInput
    {
		public string Filter { get; set; }

		public DateTime? MaxStartDateFilter { get; set; }
		public DateTime? MinStartDateFilter { get; set; }

		public DateTime? MaxFinishDateFilter { get; set; }
		public DateTime? MinFinishDateFilter { get; set; }

		public decimal? MaxPercentageOwnershipFilter { get; set; }
		public decimal? MinPercentageOwnershipFilter { get; set; }


		 public string AssetReferenceFilter { get; set; }

		 		 public string AssetOwnerNameFilter { get; set; }

		 
    }
}