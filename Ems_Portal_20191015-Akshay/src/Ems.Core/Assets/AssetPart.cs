using Ems.Assets;
using Ems.Assets;
using Ems.Assets;
using Ems.Telematics;
using Ems.Storage;
using Ems.Assets;
using Ems.Quotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Assets
{
	[Table("AssetParts")]
    public class AssetPart : FullAuditedEntity // , IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
		[Required]
		public virtual string Name { get; set; }
		
		public virtual string Description { get; set; }
		
		public virtual string SerialNumber { get; set; }
		
		public virtual DateTime? InstallDate { get; set; }
		
		public virtual string Code { get; set; }
		
		public virtual bool Installed { get; set; }
		
		public virtual int? AssetPartTypeId { get; set; }
		
        [ForeignKey("AssetPartTypeId")]
		public AssetPartType AssetPartTypeFk { get; set; }
		
		public virtual int? ParentId { get; set; }
		
        [ForeignKey("ParentId")]
		public AssetPart ParentFk { get; set; }
		
		public virtual int? AssetPartStatusId { get; set; }
		
        [ForeignKey("AssetPartStatusId")]
		public AssetPartStatus AssetPartStatusFk { get; set; }
		
		public virtual int? UsageMetricId { get; set; }
		
        [ForeignKey("UsageMetricId")]
		public UsageMetric UsageMetricFk { get; set; }
		
		public virtual int? AttachmentId { get; set; }
		
        [ForeignKey("AttachmentId")]
		public Attachment AttachmentFk { get; set; }
		
		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? ItemTypeId { get; set; }
		
        [ForeignKey("ItemTypeId")]
		public ItemType ItemTypeFk { get; set; }
		
    }
}