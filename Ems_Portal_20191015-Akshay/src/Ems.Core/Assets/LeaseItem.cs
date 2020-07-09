using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Assets
{
	[Table("LeaseItems")]
    [Audited]
    public class LeaseItem : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime? DateAllocated { get; set; }
		
		public virtual decimal? AllocationPercentage { get; set; }
		
		public virtual string Terms { get; set; }
		
		public virtual decimal? UnitRentalRate { get; set; }
		
		public virtual decimal? UnitDepositRate { get; set; }
		
		public virtual DateTime? StartDate { get; set; }
		
		public virtual DateTime? EndDate { get; set; }

		public virtual int? RentalUomRefId { get; set; }
		
		public virtual decimal? DepositUomRefId { get; set; }
		
		[Required]
		public virtual string Item { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		

		public virtual int? AssetClassId { get; set; }
		
        [ForeignKey("AssetClassId")]
		public AssetClass AssetClassFk { get; set; }
		
		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? LeaseAgreementId { get; set; }
		
        [ForeignKey("LeaseAgreementId")]
		public LeaseAgreement LeaseAgreementFk { get; set; }
		
    }
}