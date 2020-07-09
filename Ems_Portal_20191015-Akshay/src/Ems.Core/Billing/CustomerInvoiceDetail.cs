using Ems.Assets;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;
using Ems.Quotations;
using Ems.Metrics;
using Ems.Support;

namespace Ems.Billing
{
    [Table("CustomerInvoiceDetails")]
    [Audited]
    public class CustomerInvoiceDetail : FullAuditedEntity   //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Description { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual decimal UnitPrice { get; set; }

        public virtual decimal? Gross { get; set; }

        public virtual decimal? Tax { get; set; }

        public virtual decimal Net { get; set; }

        public virtual decimal? Discount { get; set; }

        public virtual decimal? Charge { get; set; }

        public virtual int? BillingRuleRefId { get; set; }

        public virtual int CustomerInvoiceId { get; set; }

        [ForeignKey("CustomerInvoiceId")]
        public CustomerInvoice CustomerInvoiceFk { get; set; }

        public virtual int? ItemTypeId { get; set; }

        [ForeignKey("ItemTypeId")]
        public ItemType ItemTypeFk { get; set; }

        public virtual int? UomId { get; set; }

        [ForeignKey("UomId")]
        public Uom UomFk { get; set; }

        public virtual int? WorkOrderActionId { get; set; }

        [ForeignKey("WorkOrderActionId")]
        public WorkOrderAction WorkOrderActionFk { get; set; }

        public virtual int? LeaseItemId { get; set; }

        [ForeignKey("LeaseItemId")]
        public LeaseItem LeaseItemFk { get; set; }
    }
}