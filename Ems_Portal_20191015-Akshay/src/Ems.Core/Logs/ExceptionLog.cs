using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ems.Logs
{
    [Table("ExceptionLog")]
    [Audited]
    public class ExceptionLog : FullAuditedEntity
    {
        public int? TenantId { get; set; }

        public string Message { get; set; }

        public string InnerException { get; set; }

        public string StackTrace { get; set; }
    }
}
