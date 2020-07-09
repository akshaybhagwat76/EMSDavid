using Ems.Assets;
using Ems.Metrics;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Support
{
	[Table("SupportItems")]
    [Audited]
    public class SupportItem : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Description { get; set; }
		
		public virtual decimal UnitPrice { get; set; }
		
		public virtual decimal? Frequency { get; set; }
		
		public virtual bool IsAdHoc { get; set; }
		
		public virtual bool IsChargeable { get; set; }
		
		public virtual bool IsStandbyReplacementUnit { get; set; }
		
		public virtual int AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? AssetClassId { get; set; }
		
        [ForeignKey("AssetClassId")]
		public AssetClass AssetClassFk { get; set; }
		
		public virtual int UomId { get; set; }
		
        [ForeignKey("UomId")]
		public Uom UomFk { get; set; }
		
		public virtual int? SupportContractId { get; set; }
		
        [ForeignKey("SupportContractId")]
		public SupportContract SupportContractFk { get; set; }
		
		public virtual int? ConsumableTypeId { get; set; }
		
        [ForeignKey("ConsumableTypeId")]
		public ConsumableType ConsumableTypeFk { get; set; }
		
		public virtual int? SupportTypeId { get; set; }
		
        [ForeignKey("SupportTypeId")]
		public SupportType SupportTypeFk { get; set; }
		
    }
}