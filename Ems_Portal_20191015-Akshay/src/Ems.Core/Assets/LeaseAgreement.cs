using Ems.Organizations;
using Ems.Assets;
using Ems.Customers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Assets
{
	[Table("LeaseAgreements")]
    [Audited]
    public class LeaseAgreement : FullAuditedEntity //, IMayHaveTenant
    {
		public int? TenantId { get; set; }
			
    	public virtual string Reference { get; set; }
		
		[Required]
		public virtual string Description { get; set; }
		
		[Required]
		public virtual string Title { get; set; }
		
		public virtual string Terms { get; set; }
		
		public virtual DateTime? StartDate { get; set; }
		
		public virtual DateTime? EndDate { get; set; }

		public virtual int? ContactId { get; set; }
		
        [ForeignKey("ContactId")]
		public Contact ContactFk { get; set; }
		
		public virtual int? AssetOwnerId { get; set; }
		
        [ForeignKey("AssetOwnerId")]
		public AssetOwner AssetOwnerFk { get; set; }
		
		public virtual int? CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
    }
}