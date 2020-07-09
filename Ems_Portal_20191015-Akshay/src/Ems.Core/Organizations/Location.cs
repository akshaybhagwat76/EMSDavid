using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using Ems.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ems.Organizations
{
    [Table("Locations")]
    [Audited]
    public class Location : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string LocationName { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }
    }
}
