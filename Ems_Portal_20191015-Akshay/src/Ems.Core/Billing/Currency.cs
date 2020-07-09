using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Billing
{
	[Table("Currencies")]
    public class Currency : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Code { get; set; }
		
		[Required]
		public virtual string Name { get; set; }
		
		public virtual string Symbol { get; set; }
		
		[Required]
		public virtual string Country { get; set; }
		
		[Required]
		public virtual string BaseCountry { get; set; }
		

    }
}