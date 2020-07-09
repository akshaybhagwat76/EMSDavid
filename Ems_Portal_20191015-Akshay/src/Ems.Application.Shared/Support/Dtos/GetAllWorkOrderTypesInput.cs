using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllWorkOrderTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string TypeFilter { get; set; }

		public string DescriptionFilter { get; set; }



    }
}