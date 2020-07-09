using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using Ems.Metrics;
using Ems.Quotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ems.Support
{
    [Table("EstimateDetails")]
    [Audited]
    public class EstimateDetail : FullAuditedEntity
    {
        public int? TenantId { get; set; }

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

        public virtual int? UomId { get; set; }

        [ForeignKey("UomId")]
        public Uom UomFk { get; set; }

        public virtual int? WorkOrderActionId { get; set; }

        [ForeignKey("WorkOrderActionId")]
        public WorkOrderAction WorkOrderActionFk { get; set; }

        public virtual int EstimateId { get; set; }

        [ForeignKey("EstimateId")]
        public Estimate EstimateFk { get; set; }
    }
}
