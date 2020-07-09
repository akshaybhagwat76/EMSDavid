using Ems.Customers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;
using Ems.Assets;
using Ems.Support;

namespace Ems.Billing
{
	[Table("CustomerInvoices")]
    [Audited]
    public class CustomerInvoice : FullAuditedEntity   //, IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		public virtual string CustomerReference { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		public virtual DateTime? DateIssued { get; set; }
		
		public virtual DateTime? DateDue { get; set; }
		
		public virtual decimal? TotalTax { get; set; }
		
		public virtual decimal TotalPrice { get; set; }
		
		public virtual decimal TotalNet { get; set; }
		
		public virtual decimal? TotalDiscount { get; set; }
		
		public virtual decimal TotalCharge { get; set; }
		
		public virtual string InvoiceRecipient { get; set; }
		
		public virtual string Remarks { get; set; }

		public virtual int CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
		public virtual int CurrencyId { get; set; }
		
        [ForeignKey("CurrencyId")]
		public Currency CurrencyFk { get; set; }
		
		public virtual int BillingRuleId { get; set; }
		
        [ForeignKey("BillingRuleId")]
		public BillingRule BillingRuleFk { get; set; }
		
		public virtual int BillingEventId { get; set; }
		
        [ForeignKey("BillingEventId")]
		public BillingEvent BillingEventFk { get; set; }
		
		public virtual int? InvoiceStatusId { get; set; }
		
        [ForeignKey("InvoiceStatusId")]
		public CustomerInvoiceStatus InvoiceStatusFk { get; set; }

        public virtual int? EstimateId { get; set; }

        [ForeignKey("EstimateId")]
        public Estimate EstimateFk { get; set; }

        public virtual int? WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrderFk { get; set; }
    }
}