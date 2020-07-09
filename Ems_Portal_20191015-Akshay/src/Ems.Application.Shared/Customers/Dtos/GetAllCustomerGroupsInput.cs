using Abp.Application.Services.Dto;
using System;

namespace Ems.Customers.Dtos
{
    public class GetAllCustomerGroupsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}