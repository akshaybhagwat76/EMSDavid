namespace Ems.Storage.Dtos
{
    public class GetAttachmentForViewDto
    {
		public AttachmentDto Attachment { get; set; }

		public string AssetReference { get; set;}

		public string IncidentDescription { get; set;}

		public string LeaseAgreementReference { get; set;}

		public string QuotationTitle { get; set;}

		public string SupportContractTitle { get; set;}

		public string WorkOrderSubject { get; set;}

		public string CustomerInvoiceDescription { get; set;}

        public string UploadedByName { get; set; }
    }
}