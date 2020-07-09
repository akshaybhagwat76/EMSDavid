
using System;
using Abp.Application.Services.Dto;

namespace Ems.Quotations.Dtos
{
    public class QuotationStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}