using Ems.Organizations;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Vendors
{
	[Table("Vendors")]
    [Audited]
    public class Vendor : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Reference { get; set; }
		
		[Required]
		public virtual string Name { get; set; }
		
		[Required]
		public virtual string Identifier { get; set; }
		
		public virtual string LogoUrl { get; set; }
		
		public virtual string Website { get; set; }
		
		public virtual string VendorLoc8GUID { get; set; }
		

		public virtual int? SsicCodeId { get; set; }
		
        [ForeignKey("SsicCodeId")]
		public SsicCode SsicCodeFk { get; set; }
		
		public virtual int? CurrencyId { get; set; }
		
        [ForeignKey("CurrencyId")]
		public Currency CurrencyFk { get; set; }
		
    }
}