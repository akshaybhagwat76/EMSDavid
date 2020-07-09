using Ems.Customers;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Customers
{
	[Table("Customers")]
    [Audited]
    public class Customer : FullAuditedEntity //, IMayHaveTenant
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
		
		public virtual string CustomerLoc8UUID { get; set; }
		

		public virtual int CustomerTypeId { get; set; }
		
        [ForeignKey("CustomerTypeId")]
		public CustomerType CustomerTypeFk { get; set; }
		
		public virtual int? CurrencyId { get; set; }
		
        [ForeignKey("CurrencyId")]
		public Currency CurrencyFk { get; set; }
		
    }
}