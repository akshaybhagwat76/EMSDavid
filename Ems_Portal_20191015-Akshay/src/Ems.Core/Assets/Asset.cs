using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Assets
{
	[Table("Assets")]
    [Audited]
    public class Asset : FullAuditedEntity //, IMayHaveTenant
    {
		public int? TenantId { get; set; }

		[Required]
		public virtual string Reference { get; set; }
		
		public virtual string VehicleRegistrationNo { get; set; }
		
		public virtual bool IsExternalAsset { get; set; }
		
		public virtual string Location { get; set; }
		
		public virtual string SerialNumber { get; set; }
		
		public virtual string EngineNo { get; set; }
		
		public virtual string ChassisNo { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		public virtual string PurchaseOrderNo { get; set; }
		
		public virtual DateTime? PurchaseDate { get; set; }
		
		public virtual decimal? PurchaseCost { get; set; }
		
		public virtual string AssetLoc8GUID { get; set; }

		public virtual int? AssetClassId { get; set; }
		
        [ForeignKey("AssetClassId")]
		public AssetClass AssetClassFk { get; set; }
		
		public virtual int? AssetStatusId { get; set; }
		
        [ForeignKey("AssetStatusId")]
		public AssetStatus AssetStatusFk { get; set; }
		
    }
}