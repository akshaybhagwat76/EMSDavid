using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetXeroInvoiceForEditOutput
    {
		public CreateOrEditXeroInvoiceDto XeroInvoice { get; set; }

		public string CustomerInvoiceCustomerReference { get; set;}


    }
}