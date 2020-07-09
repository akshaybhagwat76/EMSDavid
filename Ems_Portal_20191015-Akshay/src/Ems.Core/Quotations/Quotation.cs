using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;
using Ems.Assets;

namespace Ems.Quotations
{
    [Table("Quotations")]
    [Audited]
    public class Quotation : FullAuditedEntity //, IMayHaveTenant
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

        public virtual bool IsFinal { get; set; }

        public virtual string Remark { get; set; }

        public virtual int? RequoteRefId { get; set; }

        public virtual string QuotationLoc8GUID { get; set; }

        public virtual string AcknowledgedBy { get; set; }

        public virtual DateTime? AcknowledgedAt { get; set; }


        public virtual int SupportContractId { get; set; }

        [ForeignKey("SupportContractId")]
        public SupportContract SupportContractFk { get; set; }

        public virtual int? QuotationStatusId { get; set; }

        [ForeignKey("QuotationStatusId")]
        public QuotationStatus QuotationStatusFk { get; set; }

        public virtual int? WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrderFk { get; set; }


        public virtual int? SupportItemId { get; set; }

        [ForeignKey("SupportItemId")]
        public SupportItem SupportItemFk { get; set; }

        public virtual int? SupportTypeId { get; set; }

        [ForeignKey("SupportTypeId")]
        public SupportType SupportTypeFk { get; set; }

        public virtual int? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset AssetFk { get; set; }

        public virtual int? AssetClassId { get; set; }

        [ForeignKey("AssetClassId")]
        public AssetClass AssetClassFk { get; set; }
    }
}