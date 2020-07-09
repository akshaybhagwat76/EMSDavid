
using System;
using Abp.Application.Services.Dto;

namespace Ems.Assets.Dtos
{
    public class AssetDto : EntityDto
    {
		public string Reference { get; set; }

		public string VehicleRegistrationNo { get; set; }

		public bool IsExternalAsset { get; set; }

		public string Location { get; set; }

		public string SerialNumber { get; set; }

		public string EngineNo { get; set; }

		public string ChassisNo { get; set; }

		public string Description { get; set; }

		public string PurchaseOrderNo { get; set; }

		public DateTime? PurchaseDate { get; set; }

		public decimal? PurchaseCost { get; set; }

		public string AssetLoc8GUID { get; set; }

		


		 public int? AssetClassId { get; set; }

		 		 public int? AssetStatusId { get; set; }

		 
    }
}