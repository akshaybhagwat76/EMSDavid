using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Ems.Organizations.Dtos;

namespace Ems.Quotations.Dtos
{
    public class GetQuotationForEditOutput
    {
		public CreateOrEditQuotationDto Quotation { get; set; }

		public string SupportContractTitle { get; set;}

		public string QuotationStatusStatus { get; set;}

		public string WorkOrderSubject { get; set;}

        public string AssetReference { get; set; }

        public string AssetClassClass { get; set; }

        public string SupportTypeType { get; set; }

        public string SupportItemDescription { get; set; }

        public AddressDto AO_Address { get; set; }

    }
}