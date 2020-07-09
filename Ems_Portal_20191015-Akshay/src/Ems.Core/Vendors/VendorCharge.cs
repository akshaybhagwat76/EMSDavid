using Ems.Vendors;
using Ems.Support;
using Ems.Support;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Vendors
{
	[Table("VendorCharges")]
    [Audited]
    public class VendorCharge : FullAuditedEntity// , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Reference { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		public virtual DateTime? DateIssued { get; set; }
		
		public virtual DateTime? DateDue { get; set; }
		
		public virtual decimal? TotalTax { get; set; }
		
		public virtual decimal? TotalPrice { get; set; }
		

		public virtual int? VendorId { get; set; }
		
        [ForeignKey("VendorId")]
		public Vendor VendorFk { get; set; }
		
		public virtual int? SupportContractId { get; set; }
		
        [ForeignKey("SupportContractId")]
		public SupportContract SupportContractFk { get; set; }
		
		public virtual int? WorkOrderId { get; set; }
		
        [ForeignKey("WorkOrderId")]
		public WorkOrder WorkOrderFk { get; set; }
		
		public virtual int? VendorChargeStatusId { get; set; }
		
        [ForeignKey("VendorChargeStatusId")]
		public VendorChargeStatus VendorChargeStatusFk { get; set; }
		
    }
}