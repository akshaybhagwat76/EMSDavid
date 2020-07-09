using Abp.Application.Services.Dto;
using System;

namespace Ems.Billing.Dtos
{
    public class GetAllCustomerInvoicesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string CustomerReferenceFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public DateTime? MaxDateIssuedFilter { get; set; }
        public DateTime? MinDateIssuedFilter { get; set; }

        public DateTime? MaxDateDueFilter { get; set; }
        public DateTime? MinDateDueFilter { get; set; }

        public decimal? MaxTotalTaxFilter { get; set; }
        public decimal? MinTotalTaxFilter { get; set; }

        public decimal? MaxTotalPriceFilter { get; set; }
        public decimal? MinTotalPriceFilter { get; set; }

        public decimal? MaxTotalNetFilter { get; set; }
        public decimal? MinTotalNetFilter { get; set; }

        public decimal? MaxTotalDiscountFilter { get; set; }
        public decimal? MinTotalDiscountFilter { get; set; }

        public decimal? MaxTotalChargeFilter { get; set; }
        public decimal? MinTotalChargeFilter { get; set; }

        public string InvoiceRecipientFilter { get; set; }

        public string RemarksFilter { get; set; }


        public string CustomerNameFilter { get; set; }
        public string WorkOrderSubjectFilter { get; set; }
        public string EstimateTitleFilter { get; set; }

        public string CurrencyCodeFilter { get; set; }

        public string BillingRuleNameFilter { get; set; }

        public string BillingEventPurposeFilter { get; set; }

        public string InvoiceStatusStatusFilter { get; set; }


    }
}