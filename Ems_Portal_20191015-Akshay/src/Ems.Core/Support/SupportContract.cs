using Ems.Vendors;
using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Support
{
	[Table("SupportContracts")]
    [Audited]
    public class SupportContract : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Title { get; set; }
		
		public virtual string Reference { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		public virtual DateTime StartDate { get; set; }
		
		public virtual DateTime EndDate { get; set; }
		
		public virtual bool IsRFQTemplate { get; set; }
		
		public virtual bool IsAcknowledged { get; set; }
		
		public virtual string AcknowledgedBy { get; set; }
		
		public virtual DateTime AcknowledgedAt { get; set; }
		

		public virtual int? VendorId { get; set; }
		
        [ForeignKey("VendorId")]
		public Vendor VendorFk { get; set; }
		
		public virtual int? AssetOwnerId { get; set; }
		
        [ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
		
    }
}