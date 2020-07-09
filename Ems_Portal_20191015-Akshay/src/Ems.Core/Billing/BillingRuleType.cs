using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Billing
{
	[Table("BillingRuleTypes")]
    public class BillingRuleType : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Type { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		

    }
}