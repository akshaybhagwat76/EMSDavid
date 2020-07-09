using Ems.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Auditing;
using Ems.Metrics;

namespace Ems.Telematics
{
    [Table("UsageMetrics")]
    [Audited]
    public class UsageMetric : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public virtual string Metric { get; set; }

        public virtual string Description { get; set; }

        public virtual int? LeaseItemId { get; set; }

        [ForeignKey("LeaseItemId")]
        public LeaseItem LeaseItemFk { get; set; }

        public virtual int AssetId { get; set; }

        [ForeignKey("AssetId")]
        public Asset AssetFk { get; set; }

        public virtual int? UomId { get; set; }

        [ForeignKey("UomId")]
        public Uom UomFk { get; set; }
    }
}