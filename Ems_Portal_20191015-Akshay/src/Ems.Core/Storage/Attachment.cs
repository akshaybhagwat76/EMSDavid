using Ems.Assets;
using Ems.Support;
using Ems.Assets;
using Ems.Quotations;
using Ems.Support;
using Ems.Support;
using Ems.Billing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Ems.Storage
{
	[Table("Attachments")]
    public class Attachment : FullAuditedEntity //, IMayHaveTenant
    {
		public int? TenantId { get; set; }
			

		[Required]
		public virtual string Filename { get; set; }
		
		public virtual string Description { get; set; }
		
		public virtual DateTime UploadedAt { get; set; }
		
		public virtual long? UploadedBy { get; set; }
		
		public virtual string BlobFolder { get; set; }
		
		public virtual string BlobId { get; set; }
		

		public virtual int? AssetId { get; set; }
		
        [ForeignKey("AssetId")]
		public Asset AssetFk { get; set; }
		
		public virtual int? IncidentId { get; set; }
		
        [ForeignKey("IncidentId")]
		public Incident IncidentFk { get; set; }
		
		public virtual int? LeaseAgreementId { get; set; }
		
        [ForeignKey("LeaseAgreementId")]
		public LeaseAgreement LeaseAgreementFk { get; set; }
		
		public virtual int? QuotationId { get; set; }
		
        [ForeignKey("QuotationId")]
		public Quotation QuotationFk { get; set; }
		
		public virtual int? SupportContractId { get; set; }
		
        [ForeignKey("SupportContractId")]
		public SupportContract SupportContractFk { get; set; }
		
		public virtual int? WorkOrderId { get; set; }
		
        [ForeignKey("WorkOrderId")]
		public WorkOrder WorkOrderFk { get; set; }
		
		public virtual int? CustomerInvoiceId { get; set; }
		
        [ForeignKey("CustomerInvoiceId")]
		public CustomerInvoice CustomerInvoiceFk { get; set; }
		
    }
}