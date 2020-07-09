using Ems.Support;
using Ems.Support;
using Ems.Vendors;
using Ems.Support;
using Ems.Support;
using Ems.Authorization.Users;
using Ems.Customers;
using Ems.Assets;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;

namespace Ems.Support
{
    [Table("WorkOrders")]
    [Audited]
    public class WorkOrder : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Loc8GUID { get; set; }

        [Required]
        public virtual string Subject { get; set; }

        public virtual string Description { get; set; }

        public virtual string Location { get; set; }

        public virtual DateTime? StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public virtual string Remarks { get; set; }

        public virtual string Attachments { get; set; }

        public virtual int WorkOrderPriorityId { get; set; }

        [ForeignKey("WorkOrderPriorityId")]
        public WorkOrderPriority WorkOrderPriorityFk { get; set; }

        public virtual int WorkOrderTypeId { get; set; }

        [ForeignKey("WorkOrderTypeId")]
        public WorkOrderType WorkOrderTypeFk { get; set; }

        public virtual int VendorId { get; set; }

        [ForeignKey("VendorId")]
        public Vendor VendorFk { get; set; }

        public virtual int? IncidentId { get; set; }

        [ForeignKey("IncidentId")]
        public Incident IncidentFk { get; set; }

        public virtual int? SupportItemId { get; set; }

        [ForeignKey("SupportItemId")]
        public SupportItem SupportItemFk { get; set; }

        public virtual long UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }

        public virtual int? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer CustomerFk { get; set; }

        public virtual int? AssetOwnershipId { get; set; }

        [ForeignKey("AssetOwnershipId")]
        public AssetOwnership AssetOwnershipFk { get; set; }

        public virtual int WorkOrderStatusId { get; set; }

        [ForeignKey("WorkOrderStatusId")]
        public WorkOrderStatus WorkOrderStatusFk { get; set; }

    }
}