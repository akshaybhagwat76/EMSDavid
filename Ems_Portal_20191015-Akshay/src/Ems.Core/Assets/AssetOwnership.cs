using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Assets
{
	[Table("AssetOwnerships")]
    [Audited]
    public class AssetOwnership : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime? StartDate { get; set; }
		
		public virtual DateTime? FinishDate { get; set; }
		
		public virtual decimal? PercentageOwnership { get; set; }
		

		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? AssetOwnerId { get; set; }
		
        [ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
		
    }
}