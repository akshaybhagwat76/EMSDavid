using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Billing.Dtos
{
    public class GetBillingEventForEditOutput
    {
		public CreateOrEditBillingEventDto BillingEvent { get; set; }

		public string LeaseAgreementTitle { get; set;}

		public string VendorChargeReference { get; set;}

		public string BillingEventTypeType { get; set;}


    }
}