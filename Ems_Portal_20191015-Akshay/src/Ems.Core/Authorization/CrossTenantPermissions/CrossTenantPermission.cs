using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Authorization
{
	[Table("CrossTenantPermissions")]
    public class CrossTenantPermission : FullAuditedEntity //, IMayHaveTenant
    {
		public int? TenantId { get; set; }

		public virtual int TenantRefId { get; set; }
		
		[Required]
		public virtual string EntityType { get; set; }
		
		public virtual string Tenants { get; set; }
		
		[Required]
		public virtual string TenantType { get; set; }
		

    }
}