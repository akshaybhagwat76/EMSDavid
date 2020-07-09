using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;
using Ems.Quotations;

namespace Ems.Support
{
    [Table("WorkOrderUpdates")]
    [Audited]
    public class WorkOrderUpdate : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual int WorkOrderId { get; set; }

        [ForeignKey("WorkOrderId")]
        public WorkOrder WorkOrderFk { get; set; }

        public virtual decimal Number { get; set; }

        public virtual int? ItemTypeId { get; set; }

        [ForeignKey("ItemTypeId")]
        public ItemType ItemTypeFk { get; set; }

        public virtual int WorkOrderActionId { get; set; }

        [ForeignKey("WorkOrderActionId")]
        public WorkOrderAction WorkOrderActionFk { get; set; }

        public virtual string Comments { get; set; }


        //public virtual int? UomId { get; set; }

        //[ForeignKey("UomId")]
        //public Uom UomFk { get; set; }

        //public virtual DateTime UpdatedAt { get; set; }

        //[Required]
        //public virtual string Update { get; set; }

        //public virtual bool Complete { get; set; }

        //public virtual long UpdatedByUserId { get; set; }
    }
}