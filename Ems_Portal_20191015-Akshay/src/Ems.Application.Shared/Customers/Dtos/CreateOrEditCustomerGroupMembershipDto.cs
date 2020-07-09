
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Ems.Customers.Dtos
{
    public class CreateOrEditCustomerGroupMembershipDto : EntityDto<int?>
    {

		public DateTime? DateJoined { get; set; }
		
		
		public DateTime DateLeft { get; set; }
		
		
		 public int? CustomerGroupId { get; set; }
		 
		 		 public int? CustomerId { get; set; }
		 
		 
    }
}