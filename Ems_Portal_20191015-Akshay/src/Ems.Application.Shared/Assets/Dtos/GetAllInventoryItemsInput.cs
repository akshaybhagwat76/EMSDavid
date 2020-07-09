using Abp.Application.Services.Dto;
using System;

namespace Ems.Assets.Dtos
{
    public class GetAllInventoryItemsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public string ReferenceFilter { get; set; }

		public int? MaxQtyInWarehouseFilter { get; set; }
		public int? MinQtyInWarehouseFilter { get; set; }

		public int? MaxRestockLimitFilter { get; set; }
		public int? MinRestockLimitFilter { get; set; }

		public int? MaxQtyOnOrderFilter { get; set; }
		public int? MinQtyOnOrderFilter { get; set; }


		 public string ItemTypeTypeFilter { get; set; }

		 		 public string AssetReferenceFilter { get; set; }

		 
    }
}