using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Ems.Storage.Dtos
{
    public class GetSomeAttachmentsInput : PagedAndSortedResultRequestDto
    {
        public string RelatedEntity { get; set; }

        public int ReferenceId { get; set; }
        /*
	    public int AssetRefId { get; set; }

		public int IncidentRefId { get; set; }

		public int LeaseAgreementRefId { get; set; }

		public int QuotationRefId { get; set; }

		public int SupportContractRefId { get; set; }

		public int WorkOrderRefId { get; set; }
        */
    }
}