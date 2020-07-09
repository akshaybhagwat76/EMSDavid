using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
	[Table("WorkOrderPriorities")]
    public class WorkOrderPriority : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Priority { get; set; }
		
		public virtual int PriorityLevel { get; set; }
		

    }
}