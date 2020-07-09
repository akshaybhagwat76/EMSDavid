using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Support
{
	[Table("IncidentPriorities")]
    public class IncidentPriority : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Priority { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		public virtual int PriorityLevel { get; set; }
		

    }
}