using Ems.Authorization.Users;
using Ems.Support;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Support
{
	[Table("IncidentUpdates")]
    [Audited]
    public class IncidentUpdate : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime Updated { get; set; }
		
		public virtual string Update { get; set; }
		

		public virtual long UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
		public virtual int IncidentId { get; set; }
		
        [ForeignKey("IncidentId")]
		public Incident IncidentFk { get; set; }
		
    }
}