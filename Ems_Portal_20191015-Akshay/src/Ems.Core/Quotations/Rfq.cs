using Ems.Quotations;
using Ems.Assets;
using Ems.Customers;
using Ems.Support;
using Ems.Vendors;
using Ems.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Quotations
{
	[Table("Rfqs")]
    [Audited]
    public class Rfq : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Title { get; set; }
		
		public virtual DateTime RequestDate { get; set; }
		
		public virtual DateTime RequiredBy { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		[Required]
		public virtual string Requirements { get; set; }
		

		public virtual int RfqTypeId { get; set; }
		
        [ForeignKey("RfqTypeId")]
		public RfqType RfqTypeFk { get; set; }
		
		public virtual int? AssetOwnerId { get; set; }
		
        [ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
		
		public virtual int? CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
		public virtual int? AssetClassId { get; set; }
		
        [ForeignKey("AssetClassId")]
		public AssetClass AssetClassFk { get; set; }
		
		public virtual int? IncidentId { get; set; }
		
        [ForeignKey("IncidentId")]
		public Incident IncidentFk { get; set; }
		
		public virtual int VendorId { get; set; }
		
        [ForeignKey("VendorId")]
		public Vendor VendorFk { get; set; }
		
		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
    }
}