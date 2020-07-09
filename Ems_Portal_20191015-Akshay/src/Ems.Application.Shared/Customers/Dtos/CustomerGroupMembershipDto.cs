
using System;
using Abp.Application.Services.Dto;

namespace Ems.Customers.Dtos
{
    public class CustomerGroupMembershipDto : EntityDto
    {
		public DateTime? DateJoined { get; set; }

		public DateTime DateLeft { get; set; }


		 public int? CustomerGroupId { get; set; }

		 		 public int? CustomerId { get; set; }

		 
    }
}