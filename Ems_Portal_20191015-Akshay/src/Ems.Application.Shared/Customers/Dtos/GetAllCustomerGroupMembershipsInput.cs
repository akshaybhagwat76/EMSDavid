using Abp.Application.Services.Dto;
using System;

namespace Ems.Customers.Dtos
{
    public class GetAllCustomerGroupMembershipsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public DateTime? MaxDateJoinedFilter { get; set; }
		public DateTime? MinDateJoinedFilter { get; set; }

		public DateTime? MaxDateLeftFilter { get; set; }
		public DateTime? MinDateLeftFilter { get; set; }


		 public string CustomerGroupNameFilter { get; set; }

		 		 public string CustomerNameFilter { get; set; }

		 
    }
}