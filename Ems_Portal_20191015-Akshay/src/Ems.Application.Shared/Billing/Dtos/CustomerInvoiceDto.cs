
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class CustomerInvoiceDto : EntityDto
    {
        public string CustomerReference { get; set; }

        public string Description { get; set; }

        public DateTime? DateIssued { get; set; }

        public DateTime? DateDue { get; set; }

        public decimal? TotalTax { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal TotalNet { get; set; }

        public decimal? TotalDiscount { get; set; }

        public decimal TotalCharge { get; set; }

        public string InvoiceRecipient { get; set; }

        public string Remarks { get; set; }


        public int CustomerId { get; set; }

        public int CurrencyId { get; set; }

        public int BillingRuleId { get; set; }

        public int BillingEventId { get; set; }

        public int? InvoiceStatusId { get; set; }

        public int? EstimateId { get; set; }

        public int? WorkOrderId { get; set; }
    }
}