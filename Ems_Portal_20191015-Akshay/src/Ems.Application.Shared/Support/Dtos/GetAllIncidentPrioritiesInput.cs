using Abp.Application.Services.Dto;
using System;

namespace Ems.Support.Dtos
{
    public class GetAllIncidentPrioritiesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }



    }
}