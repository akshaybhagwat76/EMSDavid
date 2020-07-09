
using System;
using Abp.Application.Services.Dto;

namespace Ems.Customers.Dtos
{
    public class CustomerGroupDto : EntityDto
    {
		public string Name { get; set; }

		public string Description { get; set; }



    }
}