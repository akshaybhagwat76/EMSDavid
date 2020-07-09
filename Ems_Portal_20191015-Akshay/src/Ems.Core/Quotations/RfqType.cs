using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Quotations
{
	[Table("RfqTypes")]
    public class RfqType : FullAuditedEntity   //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Type { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		

    }
}