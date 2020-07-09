using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Quotations.Dtos
{
    public class GetQuotationStatusForEditOutput
    {
		public CreateOrEditQuotationStatusDto QuotationStatus { get; set; }


    }
}