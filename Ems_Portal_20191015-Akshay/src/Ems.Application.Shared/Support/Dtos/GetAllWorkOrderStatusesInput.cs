using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderStatusesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string StatusFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}