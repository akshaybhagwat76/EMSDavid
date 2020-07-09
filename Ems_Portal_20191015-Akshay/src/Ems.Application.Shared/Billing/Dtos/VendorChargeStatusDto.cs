
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class VendorChargeStatusDto : EntityDto
    {
		public string Status { get; set; }

		public string Description { get; set; }



    }
}