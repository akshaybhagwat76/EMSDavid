
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class XeroInvoiceDto : EntityDto
    {
		public bool XeroInvoiceCreated { get; set; }

		public string ApiResponse { get; set; }

		public bool Failed { get; set; }

		public string Exception { get; set; }

		public string XeroReference { get; set; }


		 public int? CustomerInvoiceId { get; set; }

		 
    }
}