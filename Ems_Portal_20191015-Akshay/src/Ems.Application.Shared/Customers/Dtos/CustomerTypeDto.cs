
using System;
using Abp.Application.Services.Dto;

namespace Ems.Customers.Dtos
{
    public class CustomerTypeDto : EntityDto
    {
		public string Type { get; set; }

		public string Description { get; set; }



    }
}