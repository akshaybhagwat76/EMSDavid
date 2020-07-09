using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetCustomerInvoiceForEditOutput
    {
		public CreateOrEditCustomerInvoiceDto CustomerInvoice { get; set; }

		public string CustomerName { get; set;}

        public string WorkOrderSubject { get; set; }

        public string EstimateTitle { get; set; }

        public string CurrencyCode { get; set;}

		public string BillingRuleName { get; set;}

		public string BillingEventPurpose { get; set;}

		public string InvoiceStatusStatus { get; set;}


    }
}