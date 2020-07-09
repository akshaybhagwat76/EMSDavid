using Ems.Quotations;
using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Assets
{
	[Table("InventoryItems")]
    public class InventoryItem : Entity , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Name { get; set; }
		
		public virtual string Reference { get; set; }
		
		public virtual int QtyInWarehouse { get; set; }
		
		public virtual int RestockLimit { get; set; }
		
		public virtual int? QtyOnOrder { get; set; }
		

		public virtual int? ItemTypeId { get; set; }
		
        [ForeignKey("ItemTypeId")]
		public ItemType ItemTypeFk { get; set; }
		
		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
    }
}