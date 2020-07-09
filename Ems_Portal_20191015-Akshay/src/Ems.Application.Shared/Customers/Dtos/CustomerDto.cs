
using System;
using Abp.Application.Services.Dto;

namespace Ems.Customers.Dtos
{
    public class CustomerDto : EntityDto
    {
		public string Reference { get; set; }

		public string Name { get; set; }

		public string Identifier { get; set; }

		public string LogoUrl { get; set; }

		public string Website { get; set; }

		public string CustomerLoc8UUID { get; set; }


		 public int CustomerTypeId { get; set; }

		 		 public int? CurrencyId { get; set; }

		 
    }
}