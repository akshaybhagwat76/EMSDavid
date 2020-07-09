using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Finance
{
	[Table("AgedReceivablesPeriods")]
    public class AgedReceivablesPeriod : AuditedEntity // , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime Period { get; set; }
		
		public virtual decimal Current { get; set; }
		
		public virtual decimal Over30 { get; set; }
		
		public virtual decimal Over60 { get; set; }
		
		public virtual decimal Over90 { get; set; }
		
		public virtual decimal Over120 { get; set; }
		

    }
}