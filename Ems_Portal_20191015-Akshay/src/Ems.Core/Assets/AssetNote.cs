using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using Ems.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ems.Assets
{
    [Table("AssetNotes")]
    [Audited]
    public class AssetNote : FullAuditedEntity
    {
        public int? TenantId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Notes { get; set; }

        public virtual int? AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset AssetFk { get; set; }

        public virtual long? UserId { get; set; }

        [ForeignKey("UserId")]
        public User UserFk { get; set; }
    }
}
