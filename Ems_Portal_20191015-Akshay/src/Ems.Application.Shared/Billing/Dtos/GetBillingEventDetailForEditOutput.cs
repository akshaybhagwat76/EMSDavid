using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetBillingEventDetailForEditOutput
    {
		public CreateOrEditBillingEventDetailDto BillingEventDetail { get; set; }

		public string BillingRuleName { get; set;}

		public string LeaseItemItem { get; set;}

		public string BillingEventPurpose { get; set;}


    }
}