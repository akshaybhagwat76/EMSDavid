using Ems.Support;
using Ems.Metrics;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;

namespace Ems.Quotations
{
    [Table("QuotationDetails")]
    [Audited]
    public class QuotationDetail : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string Description { get; set; }

        public virtual decimal Quantity { get; set; }

        public virtual decimal UnitPrice { get; set; }

        public virtual decimal? Cost { get; set; }

        public virtual decimal Tax { get; set; }

        public virtual decimal Charge { get; set; }

        public virtual decimal Discount { get; set; }

        public virtual decimal MarkUp { get; set; }

        public virtual bool IsChargeable { get; set; }

        public virtual bool IsAdHoc { get; set; }

        public virtual bool IsStandbyReplacementUnit { get; set; }

        public virtual bool IsOptionalItem { get; set; }

        public virtual string Remark { get; set; }

        public virtual string Loc8GUID { get; set; }

       

        public virtual int? ItemTypeId { get; set; }

        [ForeignKey("ItemTypeId")]
        public ItemType ItemTypeFk { get; set; }

        public virtual int QuotationId { get; set; }

        [ForeignKey("QuotationId")]
        public Quotation QuotationFk { get; set; }

        public virtual int? UomId { get; set; }

        [ForeignKey("UomId")]
        public Uom UomFk { get; set; }


        //public virtual int? EstimateId { get; set; }

        //[ForeignKey("EstimateId")]
        //public Estimate EstimateFk { get; set; }

        //public virtual int? SupportTypeId { get; set; }

        //[ForeignKey("SupportTypeId")]
        //public SupportType SupportTypeFk { get; set; }

        //public virtual int? AssetId { get; set; }

        //[ForeignKey("AssetId")]
        //public Asset AssetFk { get; set; }

        //public virtual int? AssetClassId { get; set; }

        //[ForeignKey("AssetClassId")]
        //public AssetClass AssetClassFk { get; set; }

        //public virtual int? SupportItemId { get; set; }

        //[ForeignKey("SupportItemId")]
        //public SupportItem SupportItemFk { get; set; }

        //public virtual int? WorkOrderId { get; set; }

        //[ForeignKey("WorkOrderId")]
        //public WorkOrder WorkOrderFk { get; set; }
    }
}