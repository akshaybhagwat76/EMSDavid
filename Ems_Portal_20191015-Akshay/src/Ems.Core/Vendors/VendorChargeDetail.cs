using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Vendors
{
	[Table("VendorChargeDetails")]
    [Audited]
    public class VendorChargeDetail : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string InvoiceDetail { get; set; }
		
		public virtual decimal Quantity { get; set; }
		
		public virtual decimal UnitPrice { get; set; }
		
		public virtual decimal Tax { get; set; }
		
		public virtual decimal SubTotal { get; set; }
		

    }
}