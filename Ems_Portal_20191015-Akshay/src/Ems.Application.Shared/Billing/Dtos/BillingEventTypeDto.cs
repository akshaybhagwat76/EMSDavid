
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class BillingEventTypeDto : EntityDto
    {
		public string Type { get; set; }

		public string Description { get; set; }



    }
}