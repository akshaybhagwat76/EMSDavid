using Ems.Support;
using Ems.Customers;
using Ems.Assets;
using Ems.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Support
{
	[Table("Incidents")]
    [Audited]
    public class Incident : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Description { get; set; }
		
		public virtual DateTime IncidentDate { get; set; }
		
		[Required]
		public virtual string Location { get; set; }
		
		public virtual string Remarks { get; set; }

		public virtual DateTime? ResolvedAt { get; set; }

		public virtual int? IncidentPriorityId { get; set; }
		
        [ForeignKey("IncidentPriorityId")]
		public IncidentPriority IncidentPriorityFk { get; set; }
		
		public virtual int? IncidentStatusId { get; set; }
		
        [ForeignKey("IncidentStatusId")]
		public IncidentStatus IncidentStatusFk { get; set; }
		
		public virtual int? CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? SupportItemId { get; set; }
		
        [ForeignKey("SupportItemId")]
		public SupportItem SupportItemFk { get; set; }
		
		public virtual int IncidentTypeId { get; set; }
		
        [ForeignKey("IncidentTypeId")]
		public IncidentType IncidentTypeFk { get; set; }
		
		public virtual long? UserId { get; set; }
		
        [ForeignKey("UserId")]
		public User UserFk { get; set; }
		
    }
}