using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Billing
{
	[Table("XeroInvoices")]
    public class XeroInvoice : AuditedEntity , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual bool XeroInvoiceCreated { get; set; }
		
		public virtual string ApiResponse { get; set; }
		
		public virtual bool Failed { get; set; }
		
		public virtual string Exception { get; set; }
		
		public virtual string XeroReference { get; set; }
		

		public virtual int? CustomerInvoiceId { get; set; }
		
        [ForeignKey("CustomerInvoiceId")]
		public CustomerInvoice CustomerInvoiceFk { get; set; }
		
    }
}