using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
	[Table("WorkOrderStatuses")]
    public class WorkOrderStatus : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Status { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		

    }
}