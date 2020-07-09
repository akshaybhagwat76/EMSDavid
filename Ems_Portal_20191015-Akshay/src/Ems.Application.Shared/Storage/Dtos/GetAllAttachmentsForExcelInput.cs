using Abp.Application.Services.Dto;
using System;

namespace Ems.Storage.Dtos
{
    public class GetAllAttachmentsForExcelInput
    {
		public string Filter { get; set; }

		public string FilenameFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public DateTime? MaxUploadedAtFilter { get; set; }
		public DateTime? MinUploadedAtFilter { get; set; }

		public long? MaxUploadedByFilter { get; set; }
		public long? MinUploadedByFilter { get; set; }

		public string BlobFolderFilter { get; set; }

		public string BlobIdFilter { get; set; }


		 public string AssetReferenceFilter { get; set; }

		 		 public string IncidentDescriptionFilter { get; set; }

		 		 public string LeaseAgreementReferenceFilter { get; set; }

		 		 public string QuotationTitleFilter { get; set; }

		 		 public string SupportContractTitleFilter { get; set; }

		 		 public string WorkOrderSubjectFilter { get; set; }

		 		 public string CustomerInvoiceDescriptionFilter { get; set; }

		 
    }
}