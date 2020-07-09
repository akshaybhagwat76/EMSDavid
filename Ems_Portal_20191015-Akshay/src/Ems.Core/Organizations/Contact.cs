using Ems.Authorization.Users;
using Ems.Vendors;
using Ems.Assets;
using Ems.Customers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Organizations
{
    [Table("Contacts")]
    [Audited]
    public class Contact : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual bool HeadOfficeContact { get; set; }

        public virtual string ContactName { get; set; }

        public virtual string PhoneOffice { get; set; }

        public virtual string PhoneMobile { get; set; }

        public virtual string Fax { get; set; }

        public virtual string Address { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string Position { get; set; }

        public virtual string Department { get; set; }

        public virtual string ContactLoc8GUID { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

        public virtual int? VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor VendorFk { get; set; }

        public virtual int? AssetOwnerId { get; set; }

        [ForeignKey("AssetOwnerId")]
        public AssetOwner AssetOwnerFk { get; set; }

        public virtual int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer CustomerFk { get; set; }
    }
}