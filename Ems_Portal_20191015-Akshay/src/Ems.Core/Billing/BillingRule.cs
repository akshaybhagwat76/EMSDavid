using Ems.Billing;
using Ems.Telematics;
using Ems.Assets;
using Ems.Vendors;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Billing
{
	[Table("BillingRules")]
    [Audited]
    public class BillingRule : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Name { get; set; }
		
		public virtual bool IsParent { get; set; }
		
		public virtual int? ParentBillingRuleRefId { get; set; }
		
		public virtual decimal? ChargePerUnit { get; set; }
		
		public virtual string DefaultInvoiceDescription { get; set; }
		

		public virtual int BillingRuleTypeId { get; set; }
		
        [ForeignKey("BillingRuleTypeId")]
		public BillingRuleType BillingRuleTypeFk { get; set; }
		
		public virtual int UsageMetricId { get; set; }
		
        [ForeignKey("UsageMetricId")]
		public UsageMetric UsageMetricFk { get; set; }
		
		public virtual int? LeaseAgreementId { get; set; }
		
        [ForeignKey("LeaseAgreementId")]
		public LeaseAgreement LeaseAgreementFk { get; set; }
		
		public virtual int? VendorId { get; set; }
		
        [ForeignKey("VendorId")]
		public Vendor VendorFk { get; set; }
		
		public virtual int? LeaseItemId { get; set; }
		
        [ForeignKey("LeaseItemId")]
		public LeaseItem LeaseItemFk { get; set; }
		
		public virtual int? CurrencyId { get; set; }
		
        [ForeignKey("CurrencyId")]
		public Currency CurrencyFk { get; set; }
		
    }
}