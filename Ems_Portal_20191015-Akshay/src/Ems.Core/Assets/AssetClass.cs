using Ems.Assets;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Assets
{
	[Table("AssetClasses")]
    public class AssetClass : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		public virtual string Manufacturer { get; set; }
		
		public virtual string Model { get; set; }
		
		public virtual string Specification { get; set; }
		
		[Required]
		public virtual string Class { get; set; }
		

		public virtual int AssetTypeId { get; set; }
		
        [ForeignKey("AssetTypeId")]
		public AssetType AssetTypeFk { get; set; }
		
    }
}