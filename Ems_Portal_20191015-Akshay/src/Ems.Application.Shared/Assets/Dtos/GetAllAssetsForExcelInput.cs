using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllAssetsForExcelInput
    {
		public string Filter { get; set; }

		public string ReferenceFilter { get; set; }

		public string VehicleRegistrationNoFilter { get; set; }

		public int IsExternalAssetFilter { get; set; }

		public string LocationFilter { get; set; }

		public string SerialNumberFilter { get; set; }

		public string EngineNoFilter { get; set; }

		public string ChassisNoFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public string PurchaseOrderNoFilter { get; set; }

		public DateTime? MaxPurchaseDateFilter { get; set; }
		public DateTime? MinPurchaseDateFilter { get; set; }

		public decimal? MaxPurchaseCostFilter { get; set; }
		public decimal? MinPurchaseCostFilter { get; set; }

		public string AssetLoc8GUIDFilter { get; set; }

		//public string AttachmentsFilter { get; set; }


		 public string AssetClassClassFilter { get; set; }

		 		 public string AssetStatusStatusFilter { get; set; }

		 
    }
}