using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Auditing;
using Abp.Domain.Entities.Auditing;

namespace Ems.Telematics
{
    [Table("UsageMetricRecords")]
    [Audited]
    public class UsageMetricRecord : FullAuditedEntity //, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public virtual string Reference { get; set; }

        public virtual DateTime? StartTime { get; set; }

        public virtual DateTime? EndTime { get; set; }

        public virtual decimal? UnitsConsumed { get; set; }

        public virtual int UsageMetricId { get; set; }

        [ForeignKey("UsageMetricId")]
        public UsageMetric UsageMetricFk { get; set; }

    }
}