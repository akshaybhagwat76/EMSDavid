
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Storage.Dtos
{
    public class CreateOrEditAttachmentDto : EntityDto<int?>
    {

		[Required]
		public string Filename { get; set; }
		
		
		public string Description { get; set; }
		
		
		public DateTime UploadedAt { get; set; }
		
		
		public long? UploadedBy { get; set; }
		
		
		public string BlobFolder { get; set; }
		
		
		public string BlobId { get; set; }
		
		
		 public int? AssetId { get; set; }
		 
		 		 public int? IncidentId { get; set; }
		 
		 		 public int? LeaseAgreementId { get; set; }
		 
		 		 public int? QuotationId { get; set; }
		 
		 		 public int? SupportContractId { get; set; }
		 
		 		 public int? WorkOrderId { get; set; }
		 
		 		 public int? CustomerInvoiceId { get; set; }
		 
		 
    }
}