using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetBillingRuleForEditOutput
    {
		public CreateOrEditBillingRuleDto BillingRule { get; set; }

		public string BillingRuleTypeType { get; set;}

		public string UsageMetricMetric { get; set;}

		public string LeaseAgreementTitle { get; set;}

		public string VendorName { get; set;}

		public string LeaseItemItem { get; set;}

		public string CurrencyCode { get; set;}


    }
}