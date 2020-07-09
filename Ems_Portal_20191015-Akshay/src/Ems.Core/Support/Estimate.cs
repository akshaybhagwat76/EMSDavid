using Ems.Quotations;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;
using Ems.Customers;

namespace Ems.Support
{
    [Table("Estimates")]
    [Audited]
    public class Estimate : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string Reference { get; set; }

        [Required]
        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual decimal? TotalTax { get; set; }

        public virtual decimal? TotalPrice { get; set; }

        public virtual decimal? TotalDiscount { get; set; }

        public virtual decimal? TotalCharge { get; set; }

        public virtual decimal? Version { get; set; }

        public virtual string Remark { get; set; }

        public virtual int? RequoteRefId { get; set; }

        public virtual string QuotationLoc8GUID { get; set; }

        public virtual int? AcknowledgedBy { get; set; }

        public virtual DateTime? AcknowledgedAt { get; set; }


        public virtual int? QuotationId { get; set; }

        [ForeignKey("QuotationId")]
        public Quotation QuotationFk { get; set; }

        public virtual int? WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrderFk { get; set; }

        public virtual int EstimateStatusId { get; set; }

        [ForeignKey("EstimateStatusId")]
        public EstimateStatus EstimateStatusFk { get; set; }

        public virtual int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer CustomerFk { get; set; }
    }
}