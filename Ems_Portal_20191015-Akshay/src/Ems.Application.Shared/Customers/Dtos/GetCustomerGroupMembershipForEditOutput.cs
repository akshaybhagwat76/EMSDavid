using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class GetCustomerGroupMembershipForEditOutput
    {
		public CreateOrEditCustomerGroupMembershipDto CustomerGroupMembership { get; set; }

		public string CustomerGroupName { get; set;}

		public string CustomerName { get; set;}


    }
}