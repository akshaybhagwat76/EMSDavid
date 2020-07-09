using Ems.Billing;
using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Billing
{
	[Table("BillingEventDetails")]
    [Audited]
    public class BillingEventDetail : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual bool RuleExecutedSuccessfully { get; set; }
		
		public virtual string Exception { get; set; }
		

		public virtual int? BillingRuleId { get; set; }
		
        [ForeignKey("BillingRuleId")]
		public BillingRule BillingRuleFk { get; set; }
		
		public virtual int? LeaseItemId { get; set; }
		
        [ForeignKey("LeaseItemId")]
		public LeaseItem LeaseItemFk { get; set; }
		
		public virtual int? BillingEventId { get; set; }
		
        [ForeignKey("BillingEventId")]
		public BillingEvent BillingEventFk { get; set; }
		
    }
}