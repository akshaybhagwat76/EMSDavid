using Ems.Customers;
using Ems.Assets;
using Ems.Vendors;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Organizations
{
	[Table("Addresses")]
    [Audited]
    public class Address : FullAuditedEntity   //, IMayHaveTenant
    {
		public int? TenantId { get; set; }
			

		[Required]
		public virtual string AddressEntryName { get; set; }
		
		public virtual bool IsHeadOffice { get; set; }
		
		[Required]
		public virtual string AddressLine1 { get; set; }
		
		public virtual string AddressLine2 { get; set; }
		
		public virtual string PostalCode { get; set; }
		
		public virtual string City { get; set; }
		
		public virtual string State { get; set; }
		
		public virtual string Country { get; set; }
		
		public virtual string AddressLoc8GUID { get; set; }
		
		public virtual bool IsDefaultForBilling { get; set; }
		
		public virtual bool IsDefaultForShipping { get; set; }
		

		public virtual int? CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
		public virtual int? AssetOwnerId { get; set; }
		
        [ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
		
		public virtual int? VendorId { get; set; }
		
        [ForeignKey("VendorId")]
		public Vendor VendorFk { get; set; }
		
    }
}