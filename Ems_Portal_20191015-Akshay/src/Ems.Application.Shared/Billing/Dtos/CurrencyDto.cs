
using System;
using Abp.Application.Services.Dto;

namespace Ems.Billing.Dtos
{
    public class CurrencyDto : EntityDto
    {
		public string Code { get; set; }

		public string Name { get; set; }

		public string Symbol { get; set; }

		public string Country { get; set; }

		public string BaseCountry { get; set; }



    }
}