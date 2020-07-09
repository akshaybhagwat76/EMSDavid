using Ems.Customers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace Ems.Customers
{
	[Table("CustomerGroupMemberships")]
    [Audited]
    public class CustomerGroupMembership : FullAuditedEntity //, IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual DateTime? DateJoined { get; set; }
		
		public virtual DateTime DateLeft { get; set; }
		

		public virtual int? CustomerGroupId { get; set; }
		
        [ForeignKey("CustomerGroupId")]
		public CustomerGroup CustomerGroupFk { get; set; }
		
		public virtual int? CustomerId { get; set; }
		
        [ForeignKey("CustomerId")]
		public Customer CustomerFk { get; set; }
		
    }
}