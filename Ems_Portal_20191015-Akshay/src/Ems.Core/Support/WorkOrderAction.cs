using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Ems.Support
{
    [Table("WorkOrderActions")]
    public class WorkOrderAction : FullAuditedEntity
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string Action { get; set; }

        [Required]
        public virtual string Description { get; set; }
    }
}
