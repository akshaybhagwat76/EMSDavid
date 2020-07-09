
using System;
using Abp.Application.Services.Dto;

namespace Ems.Support.Dtos
{
    public class SupportItemDto : EntityDto
    {
		public string Description { get; set; }

		public decimal UnitPrice { get; set; }

		public decimal? Frequency { get; set; }

		public bool IsAdHoc { get; set; }

		public bool IsChargeable { get; set; }

		public bool IsStandbyReplacementUnit { get; set; }

		


		 public int AssetId { get; set; }

		 		 public int? AssetClassId { get; set; }

		 		 public int UomId { get; set; }

		 		 public int? SupportContractId { get; set; }

		 		 public int? ConsumableTypeId { get; set; }

		 		 public int? SupportTypeId { get; set; }

		 
    }
}