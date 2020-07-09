using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetCustomerInvoiceStatusForEditOutput
    {
		public CreateOrEditCustomerInvoiceStatusDto CustomerInvoiceStatus { get; set; }


    }
}