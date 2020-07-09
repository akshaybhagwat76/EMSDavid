using Abp.Application.Services.Dto;
using System;

namespace Ems.Metrics.Dtos
{
    public class GetAllUomsInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}