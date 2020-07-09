using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Storage.Dtos
{
    public class GetAttachmentForEditOutput
    {
		public CreateOrEditAttachmentDto Attachment { get; set; }

		public string AssetReference { get; set;}

		public string IncidentDescription { get; set;}

		public string LeaseAgreementReference { get; set;}

		public string QuotationTitle { get; set;}

		public string SupportContractTitle { get; set;}

		public string WorkOrderSubject { get; set;}

		public string CustomerInvoiceDescription { get; set;}


    }
}