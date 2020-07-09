
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class CustomerInvoiceStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}