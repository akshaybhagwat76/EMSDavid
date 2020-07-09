using Ems.Assets;
using Ems.Vendors;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Billing
{
	[Table("BillingEvents")]
    [Audited]
    public class BillingEvent : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime BillingEventDate { get; set; }
		
		public virtual string TriggeredBy { get; set; }
		
		public virtual string Purpose { get; set; }
		
		public virtual bool WasInvoiceGenerated { get; set; }
		

		public virtual int? LeaseAgreementId { get; set; }
		
        [ForeignKey("LeaseAgreementId")]
		public LeaseAgreement LeaseAgreementFk { get; set; }
		
		public virtual int? VendorChargeId { get; set; }
		
        [ForeignKey("VendorChargeId")]
		public VendorCharge VendorChargeFk { get; set; }
		
		public virtual int BillingEventTypeId { get; set; }
		
        [ForeignKey("BillingEventTypeId")]
		public BillingEventType BillingEventTypeFk { get; set; }
		
    }
}